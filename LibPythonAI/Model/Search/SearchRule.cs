using System.Text.Json;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.Search {
    // 検索条件ルールは
    // - 検索条件
    // 検索結果の保存先フォルダ(検索フォルダ)、検索対象フォルダ、検索対象サブフォルダを含むかどうかを保持する
    // IsGlobalSearchがTrueの場合は検索フォルダ以外のどのフォルダを読み込んでも、読み込みのタイミングで検索を行う
    // IsGlobalSearchがFalseの場合は検索フォルダのみ検索を行う
    // このクラスのオブジェクトはLiteDBに保存される

    public class SearchRule {


        // Id
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";

        public string? SearchFolderId { get; set; }

        public string? TargetFolderId { get; set; }

        // 検索対象フォルダ配下を検索するかどうか
        public bool IsIncludeSubFolder { get; set; }

        // 全てのフォルダを検索するかどうか
        public bool IsGlobalSearch { get; set; }

        public string SearchConditionJson { get; set; } = "{}";

        private SearchCondition? _searchCondition;

        public SearchCondition SearchCondition {
            get {
                if (_searchCondition == null) {
                    Dictionary<string, dynamic?> dict = JsonUtil.ParseJson(SearchConditionJson);
                    _searchCondition = SearchCondition.FromDict(dict ?? new());
                }
                return _searchCondition;
            }
            set {
                SearchConditionJson = JsonSerializer.Serialize(value.ToDict(), JsonUtil.JsonSerializerOptions);
            }
        }


        public async Task<ContentFolderWrapper?> GetTargetFolder() {
            if (TargetFolderId == null) {
                return null;
            }
            ContentFolderWrapper? folder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(TargetFolderId);
            return folder;
        }

        public async Task<ContentFolderWrapper?> GetSearchFolder() {
            if (SearchFolderId == null) {
                return null;
            }
            ContentFolderWrapper? folder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(SearchFolderId);
            return folder;
        }

        public void SaveSearchConditionJson() {
            if (_searchCondition != null) {
                SearchConditionJson = JsonSerializer.Serialize(_searchCondition.ToDict(), JsonUtil.JsonSerializerOptions);
            }
        }
        // 保存
        public void Save() {
            // SearchConditionJsonを更新
            SaveSearchConditionJson();
            // APIを呼び出して保存
            PythonExecutor.PythonAIFunctions.UpdateSearchRuleAsync(new SearchRuleRequest(this));

            // Load
            Task.Run(async () => { await LoadItemsAsync(); });

        }

        // 削除
        public void Delete() {
            // APIを呼び出して削除
            PythonExecutor.PythonAIFunctions.DeleteSearchRuleAsync(new SearchRuleRequest(this));
        }

        public async Task<List<ContentItemWrapper>> SearchItems() {
            List<ContentItemWrapper> result = [];
            // GlobalSearchの場合は全フォルダを検索
            if (IsGlobalSearch) {
                return await ContentItemWrapper.SearchAll(SearchCondition);
            }
            var targetFolder = await GetTargetFolder();
            if (targetFolder != null) {
                return await ContentItemWrapper.Search(SearchCondition, targetFolder, IsIncludeSubFolder);
            }
            return result;
        }

        public SearchRule Copy() {
            SearchRule rule = new() {
                Name = Name,
                SearchFolderId = SearchFolderId,
                TargetFolderId = TargetFolderId,
                IsIncludeSubFolder = IsIncludeSubFolder,
                IsGlobalSearch = IsGlobalSearch,
                SearchConditionJson = SearchConditionJson
            };
            return rule;

        }

        public static SearchRule FromDict(Dictionary<string, dynamic?> dict) {
            SearchRule rule = new();
            if (dict.Count == 0) {
                return rule;
            }
            if (dict.TryGetValue("id", out dynamic? id)) { rule.Id = id ?? ""; }
            if (dict.TryGetValue("name", out dynamic? name)) { rule.Name = name ?? ""; }
            if (dict.TryGetValue("search_folder_id", out dynamic? searchFolderId)) { rule.SearchFolderId = searchFolderId ?? null; }
            if (dict.TryGetValue("target_folder_id", out dynamic? targetFolderId)) { rule.TargetFolderId = targetFolderId ?? null; }
            if (dict.TryGetValue("is_include_sub_folder", out dynamic? isIncludeSubFolder)) { rule.IsIncludeSubFolder = isIncludeSubFolder ?? false; }
            if (dict.TryGetValue("is_global_search", out dynamic? isGlobalSearch)) { rule.IsGlobalSearch = isGlobalSearch ?? false; }
            if (dict.TryGetValue("search_condition_json", out dynamic? searchConditionJson)) { rule.SearchConditionJson = searchConditionJson ?? "{}"; }
            return rule;
        }

        private static bool _isInitialized = false;
        private static List<SearchRule> _items = [];
        public static async Task LoadItemsAsync() {
            // 修正: 非同期メソッドで 'await' を使用
            _items = await Task.Run(() => PythonExecutor.PythonAIFunctions.GetSearchRulesAsync());
            if (_items != null) {
                _isInitialized = true;
            }
        }
        public static async Task<List<SearchRule>> GetItems() {
            if (!_isInitialized) {
                await LoadItemsAsync();
            }
            return _items;
        }

        // GetItemByName
        public static async Task<SearchRule?> GetItemByName(string name) {
            var items = await GetItems();
            return items.FirstOrDefault(x => x.Name == name);
        }

        // GetItmByFolder
        public static async Task<SearchRule?> GetItemBySearchFolder(ContentFolderWrapper folder) {
            var items = await GetItems();
            return items.FirstOrDefault(x => x.SearchFolderId == folder.Id);
        }
    }
}

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

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
                SearchConditionJson = JsonSerializer.Serialize(value.ToDict(), jsonSerializerOptions);
            }
        }


        public ContentFolderWrapper? TargetFolder {
            get {
                if (TargetFolderId == null) {
                    return null;
                }
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(TargetFolderId);
                return folder;
            }
            set {
                TargetFolderId = value?.Id;
            }
        }

        public ContentFolderWrapper? SearchFolder {
            get {
                if (SearchFolderId == null) {
                    return null;
                }
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(SearchFolderId);
                return folder;
            }
            set {
                SearchFolderId = value?.Id;
            }
        }

        public void SaveSearchConditionJson() {
            if (_searchCondition != null) {
                SearchConditionJson = JsonSerializer.Serialize(_searchCondition.ToDict(), jsonSerializerOptions);
            }
        }
        // 保存
        public async Task Save() {
            // SearchConditionJsonを更新
            SaveSearchConditionJson();
            // APIを呼び出して保存
            await PythonExecutor.PythonAIFunctions.UpdateSearchRuleAsync(new SearchRuleRequest(this));
        }

        // 削除
        public async Task Delete() {
            // APIを呼び出して削除
            await PythonExecutor.PythonAIFunctions.DeleteSearchRuleAsync(new SearchRuleRequest(this));
        }

        public List<ContentItemWrapper> SearchItems() {
            List<ContentItemWrapper> result = [];
            if (TargetFolder == null) {
                return result;
            }
            if (TargetFolder == null) {
                return result;
            }
            // GlobalSearchの場合は全フォルダを検索
            if (IsGlobalSearch) {
                return ContentItemWrapper.SearchAll(SearchCondition);
            } else {
                return ContentItemWrapper.Search(SearchCondition, TargetFolder, IsIncludeSubFolder);
            }
        }

        public SearchRule Copy() {
            SearchRule rule = new SearchRule() {
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
            SearchRule rule = new SearchRule();
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

        private static List<SearchRule> _items = new(); // 修正: 空のリストを初期化
        public static async Task LoadItemsAsync() {
            // 修正: 非同期メソッドで 'await' を使用
            _items = await Task.Run(() => PythonExecutor.PythonAIFunctions.GetSearchRulesAsync());
        }
        public static List<SearchRule> GetItems() {
            return _items;
        }

        // GetItemByName
        public static SearchRule? GetItemByName(string name) {
            return GetItems().FirstOrDefault(x => x.Name == name);
        }

        // GetItmByFolder
        public static SearchRule? GetItemBySearchFolder(ContentFolderWrapper folder) {
            return GetItems().FirstOrDefault(x => x.SearchFolderId == folder.Id);
        }
    }
}

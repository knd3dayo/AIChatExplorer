using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace LibPythonAI.Model.Search {
    // 検索条件ルールは
    // - 検索条件
    // 検索結果の保存先フォルダ(検索フォルダ)、検索対象フォルダ、検索対象サブフォルダを含むかどうかを保持する
    // IsGlobalSearchがTrueの場合は検索フォルダ以外のどのフォルダを読み込んでも、読み込みのタイミングで検索を行う
    // IsGlobalSearchがFalseの場合は検索フォルダのみ検索を行う
    // このクラスのオブジェクトはLiteDBに保存される
    public class SearchRule {
        public SearchRule(SearchRuleEntity entity) {
            Entity = entity;
        }
        public SearchRuleEntity Entity { get; set; }

        // Id
        public string Id { get => Entity.Id; }

        public SearchCondition SearchCondition {
            get => Entity.SearchCondition;
            set => Entity.SearchCondition = value;
        }

        public ContentFolderWrapper? TargetFolder {
            get {
                if (Entity.TargetFolderId == null) {
                    return null;
                }
                ContentFolderWrapper? folder =  ContentFolderWrapper.GetFolderById(Entity.TargetFolderId);
                return folder;
            }
            set {
                Entity.TargetFolderId = value?.Id;
            }
        }

        public ContentFolderWrapper? SearchFolder {
            get {
                if (Entity.SearchFolderId == null) {
                    return null;
                }
                ContentFolderWrapper? folder =  ContentFolderWrapper.GetFolderById(Entity.SearchFolderId);
                return folder;
            }
            set {
                Entity.SearchFolderId = value?.Id;
            }
        }

        // IsIncludeSubFolder
        public bool IsIncludeSubFolder {
            get => Entity.IsIncludeSubFolder;
            set => Entity.IsIncludeSubFolder = value;
        }
        // IsGlobalSearch
        public bool IsGlobalSearch {
            get => Entity.IsGlobalSearch;
            set => Entity.IsGlobalSearch = value;
        }


        public string Name {
            get => Entity.Name;
            set => Entity.Name = value;
        }

        // 保存
        public void Save() {
            SearchRuleEntity.SaveItems([Entity]);
        }

        // 削除
        public void Delete() {
            SearchRuleEntity.DeleteItems([Entity]);
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
            SearchRule rule = new SearchRule(Entity.Copy());
            return rule;

        }
        // GetItemByName
        public static SearchRule? GetItemByName(string name) {
            using PythonAILibDBContext db = new();
            var item = db.SearchRules.Where(x => x.Name == name).FirstOrDefault();
            if (item == null) {
                return null;
            }
            return new SearchRule(item);
        }

        // GetItmByFolder
        public static SearchRule? GetItemBySearchFolder(ContentFolderWrapper folder) {
            using PythonAILibDBContext db = new();
            var item = db.SearchRules.Where(x => x.SearchFolderId == folder.Id).FirstOrDefault();
            if (item == null) {
                return null;
            }
            return new SearchRule(item);
        }
    }
}

using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace PythonAILib.Model.Search {
    // 検索条件ルールは
    // - 検索条件
    // 検索結果の保存先フォルダ(検索フォルダ)、検索対象フォルダ、検索対象サブフォルダを含むかどうかを保持する
    // IsGlobalSearchがTrueの場合は検索フォルダ以外のどのフォルダを読み込んでも、読み込みのタイミングで検索を行う
    // IsGlobalSearchがFalseの場合は検索フォルダのみ検索を行う
    // このクラスのオブジェクトはLiteDBに保存される
    public class SearchRule {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        public SearchCondition SearchCondition { get; set; }

        public ObjectId SearchFolderId { get; set; } = ObjectId.Empty;

        public ObjectId TargetFolderId { get; set; } = ObjectId.Empty;

        [BsonIgnore]
        public ContentFolderWrapper? TargetFolder {
            get {
                return ContentFolderWrapper.GetFolderById(TargetFolderId);
            }
            set {
                TargetFolderId = value?.Id ?? ObjectId.Empty;
            }
        }

        [BsonIgnore]
        public ContentFolderWrapper? SearchFolder {
            get {
                return ContentFolderWrapper.GetFolderById(SearchFolderId);
            }
            set {
                SearchFolderId = value?.Id ?? ObjectId.Empty;
            }
        }

        public string Name { get; set; } = "";

        // コンストラクタ
        public SearchRule() {
            SearchCondition = new SearchCondition();
        }

        // 保存
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetSearchRuleCollection();

            collection.Upsert(this);
        }

        public List<ContentItemWrapper> SearchItems() {
            List<ContentItemWrapper> result = [];
            if (TargetFolderId == ObjectId.Empty) {
                return result;
            }
            // 検索フォルダの取得
            var TargetFolder = ContentFolderWrapper.GetFolderById(TargetFolderId);
            if (TargetFolder == null) {
                return result;
            }
            return TargetFolder.SearchItems(SearchCondition).ToList();
        }

        public SearchRule Copy() {
            SearchRule rule = new();
            rule.SearchCondition = SearchCondition.Copy();
            rule.SearchFolderId = SearchFolderId;
            rule.TargetFolderId = TargetFolderId;
            rule.Name = Name;
            return rule;

        }
    }
}

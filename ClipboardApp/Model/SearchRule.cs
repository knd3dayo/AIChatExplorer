using ClipboardApp.Factory;
using LiteDB;

namespace ClipboardApp.Model {
    // 検索条件ルールは
    // - 検索条件
    // 検索結果の保存先フォルダ(検索フォルダ)、検索対象フォルダ、検索対象サブフォルダを含むかどうかを保持する
    // IsGlobalSearchがTrueの場合は検索フォルダ以外のどのフォルダを読み込んでも、読み込みのタイミングで検索を行う
    // IsGlobalSearchがFalseの場合は検索フォルダのみ検索を行う
    // このクラスのオブジェクトはLiteDBに保存される
    public class SearchRule {
        public enum SearchType {
            // 標準 or 検索フォルダ
            Normal,
            SearchFolder
        }

        public ObjectId? Id { get; set; }

        public SearchCondition SearchCondition { get; set; }

        public ClipboardFolder? SearchFolder { get; set; }

        public ClipboardFolder? TargetFolder { get; set; }

        public string Name { get; set; } = "";

        // Type
        // 標準 or 検索フォルダ
        public SearchType Type { get; set; }

        // コンストラクタ
        public SearchRule() {
            SearchCondition = new SearchCondition();
            Type = SearchType.Normal;
        }

        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertSearchRule(this);
        }

        public List<ClipboardItem> SearchItems() {
            List<ClipboardItem> result = [];
            if (TargetFolder == null) {
                return result;
            }
            return ClipboardAppFactory.Instance.GetClipboardDBController().SearchItems(TargetFolder, SearchCondition).ToList();
        }

        public SearchRule Copy() {
            SearchRule rule = new();
            rule.SearchCondition = SearchCondition.Copy();
            rule.SearchFolder = SearchFolder;
            rule.TargetFolder = TargetFolder;
            rule.Name = Name;
            rule.Type = Type;
            return rule;

        }
    }
}

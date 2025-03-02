using LibPythonAI.Data;
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
        public SearchRule(SearchRuleEntity entity) {
            Entity = entity;
        }
        public SearchRuleEntity Entity { get; set; }


        public SearchCondition SearchCondition {
            get {
                return Entity.SearchCondition;
            }
            set {
                Entity.SearchCondition = value;
            }
        }
        public ContentFolderWrapper? TargetFolder {
            get {
                if (Entity.TargetFolder == null) {
                    return null;
                }
                ContentFolderWrapper? folder = new ContentFolderWrapper(Entity.TargetFolder);
                return folder;

            }
            set {
                Entity.TargetFolder = value?.Entity;
            }
        }

        public ContentFolderWrapper? SearchFolder {
            get {
                if (Entity.SearchFolder == null) {
                    return null;
                }
                ContentFolderWrapper? folder = new ContentFolderWrapper(Entity.SearchFolder);
                return folder;
            }
            set {
                Entity.SearchFolder = value?.Entity;
            }
        }

        public string Name {
            get {
                return Entity.Name;
            }
            set {
                Entity.Name = value;
            }
        }


        // 保存
        public void Save() {
            using PythonAILibDBContext db = new();
            db.SearchRules.Update(Entity);
            db.SaveChanges();
        }

        public List<ContentItemWrapper> SearchItems() {
            List<ContentItemWrapper> result = [];
            if (Entity.TargetFolder == null) {
                return result;
            }
            if (TargetFolder == null) {
                return result;
            }
            return TargetFolder.SearchItems(SearchCondition).ToList();
        }

        public SearchRule Copy() {
            SearchRule rule = new SearchRule(Entity.Copy());
            return rule;

        }
    }
}

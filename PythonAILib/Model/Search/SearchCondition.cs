using LiteDB;

namespace PythonAILib.Model.Search {
    public class SearchCondition {
        private bool _ExcludeSourceApplicationName = false;
        private bool _ExcludeSourceApplicationTitle = false;

        // ObjectId
        public ObjectId? Id { get; set; }
        // 検索条件の名前
        public string Name { get; set; } = "";

        // デフォルトコンストラクタ
        public SearchCondition() {
        }

        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Tags { get; set; } = "";

        public string SourceApplicationName { get; set; } = ""
;       public string SourceApplicationTitle {  get; set; } = "";

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = DateTime.Now;
        public bool EnableStartTime { get; set; } = false;

        public bool EnableEndTime { get; set; } = false;

        public bool ExcludeDescription { get; set; } = false;
        public bool ExcludeContent { get; set; } = false;
        public bool ExcludeTags { get; set; } = false;

        public bool ExcludeSourceApplicationName { get; set; } = false;
        public bool ExcludeSourceApplicationTitle { get; set; } = false;

        public SearchCondition Copy() {
            SearchCondition searchCondition = new();
            searchCondition.CopyFrom(this);
            return searchCondition;
        }
        public void CopyFrom(SearchCondition searchCondition) {
            Description = searchCondition.Description;
            Content = searchCondition.Content;
            Tags = searchCondition.Tags;
            SourceApplicationName = searchCondition.SourceApplicationName;
            SourceApplicationTitle = searchCondition.SourceApplicationTitle;
            StartTime = searchCondition.StartTime;
            EndTime = searchCondition.EndTime;
            EnableStartTime = searchCondition.EnableStartTime;
            EnableEndTime = searchCondition.EnableEndTime;
            ExcludeDescription = searchCondition.ExcludeDescription;
            ExcludeContent = searchCondition.ExcludeContent;
            ExcludeTags = searchCondition.ExcludeTags;
            ExcludeSourceApplicationName = searchCondition.ExcludeSourceApplicationName;
            ExcludeSourceApplicationTitle = searchCondition.ExcludeSourceApplicationTitle;

        }

        public void Clear() {
            Description = "";
            Content = "";
            Tags = "";
            SourceApplicationName = "";
            SourceApplicationTitle = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            EnableStartTime = false;
            EnableEndTime = false;
            ExcludeDescription = false;
            ExcludeContent = false;
            ExcludeTags = false;
            ExcludeSourceApplicationName = false;
            ExcludeSourceApplicationTitle = false;

        }
        public bool IsEmpty() {
            return
                string.IsNullOrEmpty(Description)
                && string.IsNullOrEmpty(Content)
                && string.IsNullOrEmpty(Tags)
                && string.IsNullOrEmpty(SourceApplicationName)
                && string.IsNullOrEmpty(SourceApplicationTitle)
                && EnableEndTime == false
                && EnableEndTime == false;
        }
        public string ToStringSearchCondition() {
            string description = "";
            if (string.IsNullOrEmpty(Description) == false) {
                if (ExcludeDescription) {
                    description += " -Description: " + Description;
                } else {
                    description += " Description: " + Description;
                }
            }
            if (string.IsNullOrEmpty(Content) == false) {
                if (ExcludeContent) {
                    description += " -Content: " + Content;
                } else {
                    description += " Content: " + Content;
                }
            }
            if (string.IsNullOrEmpty(Tags) == false) {
                if (ExcludeTags) {
                    description += " -Tags: " + Tags;
                } else {
                    description += " Tags: " + Tags;
                }
            }
            if (string.IsNullOrEmpty(SourceApplicationName) == false) {
                if (ExcludeSourceApplicationName) {
                    description += " -SourceApplicationName: " + SourceApplicationName;
                } else {
                    description += " SourceApplicationName: " + SourceApplicationName;
                }
            }
            if (string.IsNullOrEmpty(SourceApplicationTitle) == false) {
                if (ExcludeSourceApplicationTitle) {
                    description += " -SourceApplicationTitle: " + SourceApplicationTitle;
                } else {
                    description += " SourceApplicationTitle: " + SourceApplicationTitle;
                }
            }
            if (EnableStartTime) {
                description += " StartTime: " + StartTime;
            }
            if (EnableEndTime) {
                description += " EndTime: " + EndTime;
            }
            return description;
        }
        // 検索対象フォルダ配下を検索するかどうか
        public bool IsIncludeSubFolder { get; set; }


    }

}

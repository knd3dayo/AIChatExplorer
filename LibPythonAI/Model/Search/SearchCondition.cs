using LiteDB;

namespace PythonAILib.Model.Search {
    public class SearchCondition {


        // 検索条件の名前 
        public string Name { get; set; } = "";

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

        // ToDict()
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "description", Description },
                { "content", Content },
                { "tags", Tags },
                { "source_application_name", SourceApplicationName },
                { "source_application_title", SourceApplicationTitle },
                { "start_time", StartTime },
                { "end_time", EndTime },
                { "enable_start_time", EnableStartTime },
                { "enable_end_time", EnableEndTime },
                { "exclude_description", ExcludeDescription },
                { "exclude_content", ExcludeContent },
                { "exclude_tags", ExcludeTags },
                { "exclude_source_application_name", ExcludeSourceApplicationName },
                { "exclude_source_application_title", ExcludeSourceApplicationTitle },
            };
            return dict;
        }

        public static SearchCondition FromDict(Dictionary<string, object> dict) {
            SearchCondition searchCondition = new();
            searchCondition.Description = (string)dict["description"];
            searchCondition.Content = (string)dict["content"];
            searchCondition.Tags = (string)dict["tags"];
            searchCondition.SourceApplicationName = (string)dict["source_application_name"];
            searchCondition.SourceApplicationTitle = (string)dict["source_application_title"];
            searchCondition.StartTime = (DateTime)dict["start_time"];
            searchCondition.EndTime = (DateTime)dict["end_time"];
            searchCondition.EnableStartTime = (bool)dict["enable_start_time"];
            searchCondition.EnableEndTime = (bool)dict["enable_end_time"];
            searchCondition.ExcludeDescription = (bool)dict["exclude_description"];
            searchCondition.ExcludeContent = (bool)dict["exclude_content"];
            searchCondition.ExcludeTags = (bool)dict["exclude_tags"];
            searchCondition.ExcludeSourceApplicationName = (bool)dict["exclude_source_application_name"];
            searchCondition.ExcludeSourceApplicationTitle = (bool)dict["exclude_source_application_title"];
            return searchCondition;
        }

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

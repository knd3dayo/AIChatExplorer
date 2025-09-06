namespace LibMain.Model.Search {
    public class SearchCondition {


        // 検索条件の名前 
        public string Name { get; set; } = "";

        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Tags { get; set; } = "";

        public string SourceApplicationName { get; set; } = "";
        public string SourceApplicationTitle { get; set; } = "";

        public DateTime StartTime { get; set; } = DateTime.Now;

        public DateTime EndTime { get; set; } = DateTime.Now;
        public bool EnableStartTime { get; set; } = false;

        public bool EnableEndTime { get; set; } = false;

        public bool ExcludeDescription { get; set; } = false;
        public bool ExcludeContent { get; set; } = false;
        public bool ExcludeTags { get; set; } = false;

        public bool ExcludeSourceApplicationName { get; set; } = false;
        public bool ExcludeSourceApplicationTitle { get; set; } = false;

        // target_folder_id
        public string TargetFolderId { get; set; } = "";

        // ターゲットフォルダ配下のアイテムを検索するかどうか
        public bool IncludeSubFolders { get; set; } = false;


        // ToDict()
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "description", Description },
                { "content", Content },
                { "tags", Tags },
                { "source_application_name", SourceApplicationName },
                { "source_application_title", SourceApplicationTitle },
                { "start_time_str", StartTime.ToString("o") },
                { "end_time_str", EndTime.ToString("o") },
                { "enable_start_time", EnableStartTime },
                { "enable_end_time", EnableEndTime },
                { "exclude_description", ExcludeDescription },
                { "exclude_content", ExcludeContent },
                { "exclude_tags", ExcludeTags },
                { "exclude_source_application_name", ExcludeSourceApplicationName },
                { "exclude_source_application_title", ExcludeSourceApplicationTitle },
                { "target_folder_id", TargetFolderId },
                { "include_sub_folders", IncludeSubFolders }
            };
            return dict;
        }

        public static SearchCondition FromDict(Dictionary<string, dynamic?> dict) {
            SearchCondition searchCondition = new();
            if (dict.Count == 0) {
                return searchCondition;
            }
            if (dict.TryGetValue("description", out dynamic? description)) { searchCondition.Description = description ?? ""; }
            if (dict.TryGetValue("content", out dynamic? content)) { searchCondition.Content = content ?? ""; }
            if (dict.TryGetValue("tags", out dynamic? tags)) { searchCondition.Tags = tags ?? ""; }
            if (dict.TryGetValue("source_application_name", out dynamic? sourceApplicationName)) { searchCondition.SourceApplicationName = sourceApplicationName ?? ""; }
            if (dict.TryGetValue("source_application_title", out dynamic? sourceApplicationTitle)) { searchCondition.SourceApplicationTitle = sourceApplicationTitle ?? ""; }
            if (dict.TryGetValue("start_time_str", out dynamic? startTime)) {
                // UTC文字列をDateTimeに変換
                DateTime utcDateTime = DateTime.Parse(startTime, null, System.Globalization.DateTimeStyles.RoundtripKind); 
                searchCondition.StartTime = utcDateTime; 
            }
            if (dict.TryGetValue("end_time_str", out dynamic? endTime)) {
                // UTC文字列をDateTimeに変換
                DateTime utcDateTime = DateTime.Parse(endTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
                searchCondition.EndTime = utcDateTime;
            }
            if (dict.TryGetValue("enable_start_time", out dynamic? enableStartTime)) { searchCondition.EnableStartTime = enableStartTime ?? false; }
            if (dict.TryGetValue("enable_end_time", out dynamic? enableEndTime)) { searchCondition.EnableEndTime = enableEndTime ?? false; }
            if (dict.TryGetValue("exclude_description", out dynamic? excludeDescription)) { searchCondition.ExcludeDescription = excludeDescription ?? false; }
            if (dict.TryGetValue("exclude_content", out dynamic? excludeContent)) { searchCondition.ExcludeContent = excludeContent ?? false; }
            if (dict.TryGetValue("exclude_tags", out dynamic? excludeTags)) { searchCondition.ExcludeTags = excludeTags ?? false; }
            if (dict.TryGetValue("exclude_source_application_name", out dynamic? excludeSourceApplicationName)) { searchCondition.ExcludeSourceApplicationName = excludeSourceApplicationName ?? false; }
            if (dict.TryGetValue("exclude_source_application_title", out dynamic? excludeSourceApplicationTitle)) { searchCondition.ExcludeSourceApplicationTitle = excludeSourceApplicationTitle ?? false; }

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

    }

}

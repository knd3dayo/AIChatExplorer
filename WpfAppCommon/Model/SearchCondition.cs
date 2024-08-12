using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfAppCommon.Model {
    public class SearchCondition : ObservableObject {
        private string _description = "";
        private string _content = "";
        private string _tags = "";
        private string _sourceApplicationName = "";
        private string _sourceApplicationTitle = "";
        private DateTime _startTime = DateTime.Now;
        private DateTime _endTime = DateTime.Now;
        private bool _enableStartTime = false;
        private bool _enableEndTime = false;
        private bool _ExcludeDescription = false;
        private bool _ExcludeContent = false;
        private bool _ExcludeTags = false;
        private bool _ExcludeSourceApplicationName = false;
        private bool _ExcludeSourceApplicationTitle = false;

        // ObjectId
        public ObjectId? Id { get; set; }
        // 検索条件の名前
        public string Name { get; set; } = "";

        // デフォルトコンストラクタ
        public SearchCondition() {
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Content {
            get { return _content; }
            set {
                _content = value;
                OnPropertyChanged("Content");
            }
        }
        public string Tags {
            get { return _tags; }
            set {
                _tags = value;
                OnPropertyChanged("Tags");
            }
        }
        public string SourceApplicationName {
            get { return _sourceApplicationName; }
            set {
                _sourceApplicationName = value;
                OnPropertyChanged("SourceApplicationName");
            }
        }
        public string SourceApplicationTitle {
            get { return _sourceApplicationTitle; }
            set {
                _sourceApplicationTitle = value;
                OnPropertyChanged("SourceApplicationTitle");
            }
        }

        public DateTime StartTime {
            get { return _startTime; }
            set {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime {
            get { return _endTime; }
            set {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }
        public bool EnableStartTime {
            get { return _enableStartTime; }
            set {
                _enableStartTime = value;
                OnPropertyChanged("EnableStartTime");
            }
        }
        public bool EnableEndTime {
            get { return _enableEndTime; }
            set {
                _enableEndTime = value;
                OnPropertyChanged("EnableEndTime");
            }
        }

        public bool ExcludeDescription {
            get { return _ExcludeDescription; }
            set {
                _ExcludeDescription = value;
                OnPropertyChanged("ExcludeDescription");
            }
        }
        public bool ExcludeContent {
            get { return _ExcludeContent; }
            set {
                _ExcludeContent = value;
                OnPropertyChanged("ExcludeContent");
            }
        }
        public bool ExcludeTags {
            get { return _ExcludeTags; }
            set {
                _ExcludeTags = value;
                OnPropertyChanged("ExcludeTags");
            }
        }
        public bool ExcludeSourceApplicationName {
            get { return _ExcludeSourceApplicationName; }
            set {
                _ExcludeSourceApplicationName = value;
                OnPropertyChanged("ExcludeSourceApplicationName");
            }
        }
        public bool ExcludeSourceApplicationTitle {
            get { return _ExcludeSourceApplicationTitle; }
            set {
                _ExcludeSourceApplicationTitle = value;
                OnPropertyChanged("ExcludeSourceApplicationTitle");
            }
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

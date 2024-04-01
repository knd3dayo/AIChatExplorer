using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    public class SearchCondition: ObservableObject
    {
        private string _description = "";
        private string _content = "";
        private string _tags = "";
        private string _sourceApplicationName = "";
        private string _sourceApplicationTitle = "";
        private DateTime _startTime = DateTime.Now;
        private DateTime _endTime = DateTime.Now;
        private bool _enableStartTime = false;
        private bool _enableEndTime = false;


        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }
        public string Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                OnPropertyChanged("Tags");
            }
        }
        public string SourceApplicationName
        {
            get { return _sourceApplicationName; }
            set
            {
                _sourceApplicationName = value;
                OnPropertyChanged("SourceApplicationName");
            }
        }
        public string SourceApplicationTitle
        {
            get { return _sourceApplicationTitle; }
            set
            {
                _sourceApplicationTitle = value;
                OnPropertyChanged("SourceApplicationTitle");
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }
        public bool EnableStartTime
        {
            get { return _enableStartTime; }
            set
            {
                _enableStartTime = value;
                OnPropertyChanged("EnableStartTime");
            }
        }
        public bool EnableEndTime
        {
            get { return _enableEndTime; }
            set
            {
                _enableEndTime = value;
                OnPropertyChanged("EnableEndTime");
            }
        }


        public void CopyFrom(SearchCondition searchCondition)
        {
            Description = searchCondition.Description;
            Content = searchCondition.Content;
            Tags = searchCondition.Tags;
            SourceApplicationName = searchCondition.SourceApplicationName;
            SourceApplicationTitle = searchCondition.SourceApplicationTitle;
            StartTime = searchCondition.StartTime;
            EndTime = searchCondition.EndTime;
            EnableStartTime = searchCondition.EnableStartTime;
            EnableEndTime = searchCondition.EnableEndTime;
        }

        public void Clear()
        {
            Description = "";
            Content = "";
            Tags = "";
            SourceApplicationName = "";
            SourceApplicationTitle = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            EnableStartTime = false;
            EnableEndTime = false;
        }
        public bool IsEmpty()
        {
            return 
                string.IsNullOrEmpty(Description)
                && string.IsNullOrEmpty(Content)
                && string.IsNullOrEmpty(Tags) 
                && string.IsNullOrEmpty(SourceApplicationName) 
                && string.IsNullOrEmpty(SourceApplicationTitle) 
                && EnableEndTime == false
                && EnableEndTime == false;
        }
        public string ToStringSearchCondition()
        {
            string description = "";
            if (string.IsNullOrEmpty(Description) == false)
            {
                description += " Description: " + Description;
            }
            if (string.IsNullOrEmpty(Content) == false)
            {
                description += " Content: " + Content;
            }
            if (string.IsNullOrEmpty(Tags) == false)
            {
                description += " Tags: " + Tags;
            }
            if (string.IsNullOrEmpty(SourceApplicationName) == false)
            {
                description += " SourceApplicationName: " + SourceApplicationName;
            }
            if (string.IsNullOrEmpty(SourceApplicationTitle) == false)
            {
                description += " SourceApplicationTitle: " + SourceApplicationTitle;
            }
            if (EnableStartTime)
            {
                description += " StartTime: " + StartTime;
            }
            if (EnableEndTime)
            {
                description += " EndTime: " + EndTime;
            }
            return description;
        }
    }

}

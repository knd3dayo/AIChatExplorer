
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfAppCommon.Model
{
    public class TagItem : ObservableObject
    {
        public ObjectId? Id { get; set; }

        private string _tag = "";
        public string Tag
        {
            get
            {
                return _tag;
            }
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
        private bool _isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
                OnPropertyChanged("IsDeleted");
            }
        }
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteTag(Tag);
            IsDeleted = true;
        }
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().InsertTag(Tag);
            IsDeleted = false;
        }
    }

}

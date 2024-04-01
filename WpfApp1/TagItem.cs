
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    public class TagItem: ObservableObject
    {
        public int Id { get; set; }

        private string _tag = "";
        public string Tag {
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
        public Boolean IsChecked { 
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
        public Boolean IsDeleted
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
    }
   
}

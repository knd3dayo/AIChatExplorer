using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;

namespace ClipboardApp.View.TagView {
    public class TagItemViewModel : MyWindowViewModel {

        public string Tag { get; set; }

        private bool _isChecked = false;
        public bool IsChecked {
            get {
                return _isChecked;
            }
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private bool _isCommonTag;
        public bool IsCommonTag {
            get {
                return _isCommonTag;
            }
            set {
                _isCommonTag = value;
                OnPropertyChanged(nameof(IsCommonTag));
            }
        }

        private TagItem _tagItem;
        public TagItem TagItem {
            get {
                return _tagItem;
            }
        }

        public TagItemViewModel(TagItem tagItem) {
            _tagItem = tagItem;
            Tag = tagItem.Tag;
            IsChecked = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon;
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

        public static void AddCommonTags(ObservableCollection<TagItemViewModel> tagList) {
            // 共通タグを追加
            foreach (var item in tagList) {
                if (item.IsChecked) {
                    // LiteDBにタグを追加  
                    TagItem tagItem = new() { Tag = item.Tag };
                    ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
                }
            }
        }
    }
}

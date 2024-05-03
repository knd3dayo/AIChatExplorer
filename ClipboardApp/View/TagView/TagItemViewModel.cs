using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;

namespace ClipboardApp.View.TagView {
    public class TagItemViewModel : MyWindowViewModel {

        public string Tag { get; set; }
        public bool IsChecked { get; set; }

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

using PythonAILib.Model.Tag;

namespace LibUIPythonAI.ViewModel.Tag {
    public class TagItemViewModel : ChatViewModelBase {

        // コンストラクタ
        public TagItemViewModel(TagItem tagItem) {
            TagItem = tagItem;
            Tag = tagItem.Tag;
            IsChecked = false;
        }

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
        public TagItem TagItem { get; set; }

    }
}

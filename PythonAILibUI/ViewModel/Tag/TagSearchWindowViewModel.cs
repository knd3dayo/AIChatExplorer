using System.Windows;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Tag {
    public class TagSearchWindowViewModel : QAChatViewModelBase {
        private Action<string, bool> _afterUpdate = (tag, exclude) => { };

        public TagSearchWindowViewModel(Action<string, bool> afterUpdate) {
            _afterUpdate = afterUpdate;
        }
        private bool _excludeTag = false;
        public bool ExcludeTag {
            get {
                return _excludeTag;
            }
            set {
                _excludeTag = value;
                OnPropertyChanged(nameof(ExcludeTag));
            }
        }

        private string _tagName = "";
        public string TagName {
            get {
                return _tagName;
            }
            set {
                _tagName = value;
                OnPropertyChanged(nameof(TagName));
            }
        }

        // クリアボタンの処理
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            TagName = "";
            ExcludeTag = false;
        });

        // 検索ボタンの処理
        public SimpleDelegateCommand<Window> SearchCommand => new((window) => {
            _afterUpdate(TagName, ExcludeTag);
            window.Close();
        });

    }
}

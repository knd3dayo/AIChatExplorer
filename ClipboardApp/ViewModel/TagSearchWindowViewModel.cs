using System.Windows;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public class TagSearchWindowViewModel : MyWindowViewModel {
        private Action<string, bool> _afterUpdate = (tag, exclude) => { };

        public void Initialize(Action<string, bool> afterUpdate) {
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
        // 閉じるボタンの処理
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });

        // 検索ボタンの処理
        public SimpleDelegateCommand<Window> SearchCommand => new((window) => {
            _afterUpdate(TagName, ExcludeTag);
            window.Close();
        });

    }
}

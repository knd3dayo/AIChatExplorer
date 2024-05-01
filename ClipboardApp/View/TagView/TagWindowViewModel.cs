using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.TagView {
    public class TagWindowViewModel : MyWindowViewModel {

        public ObservableCollection<TagItemViewModel> TagList { get; set; } = [];

        private ClipboardItem? _clipboardItem;

        Action? _afterUpdate;

        public void Initialize(ClipboardItem? clipboardItem, Action afterUpdate) {
            _clipboardItem = clipboardItem;
            _afterUpdate = afterUpdate;
            if (clipboardItem == null) {
                return;
            }
            foreach (var item in TagList) {
                var tagString = item.Tag;
                item.IsChecked = clipboardItem.Tags.Contains(tagString);
            }

        }
        public TagWindowViewModel() {
            foreach (var item in TagController.GetTagList()) {
                TagList.Add(new TagItemViewModel(item));
            }
        }

        //新規タグのテキスト
        private string _newTag = "";
        public string NewTag {
            get {
                return _newTag;
            }
            set {
                _newTag = value;
                OnPropertyChanged(nameof(NewTag));
            }
        }

        // タグを追加したときの処理
        public SimpleDelegateCommand AddTagCommand => new SimpleDelegateCommand(AddTagCommandExecute);

        private void AddTagCommandExecute(object parameter) {
            if (parameter is not string) {
                Tools.Error("パラメーターがありません");
                return;
            }
            string tag = (string)parameter;
            if (string.IsNullOrEmpty(tag)) {
                Tools.Error("タグが空です");
            }
            //tagが既に存在するかチェック
            foreach (var item in TagList) {
                if (item.Tag == tag) {
                    Tools.Error("タグが既に存在します");
                    return;
                }
            }

            TagItem tagItem = new TagItem { Tag = tag };
            ClipboardAppFactory.Instance.GetClipboardDBController().InsertTag(tagItem);
            TagList.Add(new TagItemViewModel(tagItem));
            NewTag = "";
        }

        // タグを削除したときの処理
        public SimpleDelegateCommand DeleteTagCommand => new SimpleDelegateCommand(
            (parameter) => {
                // IsCheckedがTrueのものを削除
                foreach (var item in TagList) {
                    if (item.IsChecked) {
                        // LiteDBから削除
                        item.TagItem.Delete();

                        // TagListから削除
                        TagList.Remove(item);
                    }
                }
                // LiteDBから再読み込み
                TagList.Clear();
                foreach (var item in TagController.GetTagList()) {
                    TagList.Add(new TagItemViewModel(item));
                }
            });

        // OKボタンを押したときの処理
        public SimpleDelegateCommand OkCommand => new((parameter) => {
            if (_clipboardItem != null) {
                // TagListのチェックを反映
                foreach (var item in TagList) {
                    if (item.IsChecked) {
                        _clipboardItem.Tags.Add(item.Tag);
                    } else {
                        _clipboardItem.Tags.Remove(item.Tag);
                    }
                }
                // DBに反映
                _clipboardItem.Save();

            }
            // 更新後の処理を実行
            _afterUpdate?.Invoke();

            // ウィンドウを閉じる
            if (parameter is not System.Windows.Window window) {
                return;
            }
            window.Close();

        });
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CancelCommand => new(CancelCommandExecute);
        private void CancelCommandExecute(object obj) {
            if (obj is not System.Windows.Window window) {
                return;
            }
            // ウィンドウを閉じる
            window.Close();
        }
        // 検索ウィンドウを開く
        public SimpleDelegateCommand OpenSearchWindowCommand => new((parameter) => {
            var searchWindow = new TagSearchWindow();
            searchWindow.Show();
        });
    }
}

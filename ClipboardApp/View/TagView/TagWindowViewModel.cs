using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.Factory;
using ClipboardApp.Factory.Default;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.TagView
{
    public class TagWindowViewModel : ObservableObject {
        public ObservableCollection<TagItem> TagList { get; set; } = new ObservableCollection<TagItem>();

        private ClipboardItem? _clipboardItem;
        public ClipboardItem? ClipboardItem {
            get {
                return _clipboardItem;
            }
            set {
                _clipboardItem = value;
                if (value != null) {
                    foreach (var item in TagList) {
                        var tagString = item.Tag;
                        item.IsChecked = value.Tags.Contains(tagString);
                    }
                }
            }
        }
        Action? _afterUpdate;

        public void Initialize(ClipboardItem clipboardItem, Action afterUpdate) {
            ClipboardItem = clipboardItem;
            _afterUpdate = afterUpdate;
        }
        public TagWindowViewModel() {
            foreach (var item in TagController.GetTagList()) {
                TagList.Add(item);
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
                OnPropertyChanged("NewTag");
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
            TagList.Add(tagItem);
            ClipboardAppFactory.Instance.GetClipboardDBController().InsertTag(tag);
            NewTag = "";
        }

        // タグを削除したときの処理
        public SimpleDelegateCommand DeleteTagCommand => new SimpleDelegateCommand(
            (parameter) => {
                // IsCheckedがTrueのものを削除
                foreach (var item in TagList) {
                    if (item.IsChecked) {
                        // LiteDBから削除
                        item.Delete();
                        // TagListから削除
                    }
                }
                // LiteDBから再読み込み
                TagList.Clear();
                foreach (var item in TagController.GetTagList()) {
                    TagList.Add(item);
                }
            });

        // OKボタンを押したときの処理
        public SimpleDelegateCommand OkCommand => new((parameter) => {
            if (ClipboardItem == null) {
                // クリップボードアイテムがnullの場合は何もしない
                // ウィンドウを閉じる
                return;
            }
            // ClipboardItemのタグをクリア
            ClipboardItem.Tags.Clear();
            // TagListのチェックを反映
            foreach (var item in TagList) {
                if (item.IsChecked) {
                    ClipboardItem.Tags.Add(item.Tag);
                } else {
                    ClipboardItem.Tags.Remove(item.Tag);
                }
            }
            // DBに反映
            ClipboardItem.Save();

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
    }
}

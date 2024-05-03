using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.TagView {
    public class TagWindowViewModel : MyWindowViewModel {

        public ObservableCollection<TagItemViewModel> TagList { get; set; } = [];

        private ClipboardItemViewModel? itemViewModel;

        Action? _afterUpdate;

        public void Initialize(ClipboardItemViewModel? itemViewModel, Action afterUpdate) {
            this.itemViewModel = itemViewModel;
            _afterUpdate = afterUpdate;
            ReloadTagList();

        }
        public TagWindowViewModel() {
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
        public SimpleDelegateCommand AddTagCommand => new (AddTagCommandExecute);

        private void AddTagCommandExecute(object parameter) {
            if (parameter is not string) {
                Tools.Error("パラメーターがありません");
                return;
            }
            string tag = (string)parameter;
            if (string.IsNullOrEmpty(tag)) {
                Tools.Error("タグが空です");
                return;
            }
            //tagが既に存在するかチェック
            foreach (var item in TagList) {
                if (item.Tag == tag) {
                    Tools.Error("タグが既に存在します");
                    return;
                }
            }

            TagItem tagItem = new () { Tag = tag };
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
            TagList.Add(new TagItemViewModel(tagItem));
            NewTag = "";
            // LiteDBから再読み込み
            ReloadTagList();

        }
        // LiteDBから再読み込み
        public void ReloadTagList() {
            TagList.Clear();
            foreach (var item in TagItem.GetTagList()) {
                TagList.Add(new TagItemViewModel(item));
                if (itemViewModel != null) {
                    var tagString = item.Tag;
                    TagList.Last().IsChecked = itemViewModel.Tags.Contains(tagString);
                }
            }
        }

        // タグを削除したときの処理
        public SimpleDelegateCommand DeleteTagCommand => new ((parameter) => {
                // IsCheckedがTrueのものを削除
                foreach (var item in TagList) {
                    if (item.IsChecked) {
                        // LiteDBから削除
                        item.TagItem.Delete();
                    }
                }
                // LiteDBから再読み込み
                ReloadTagList();
            });

        // OKボタンを押したときの処理
        public SimpleDelegateCommand OkCommand => new((parameter) => {
            if (itemViewModel != null) {
                // TagListのチェックを反映
                foreach (var item in TagList) {
                    if (item.IsChecked) {
                        itemViewModel.Tags.Add(item.Tag);
                    } else {
                        itemViewModel.Tags.Remove(item.Tag);
                    }
                }
                // DBに反映
                itemViewModel.Save();

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
            var searchWindowViewModel = (TagSearchWindowViewModel)searchWindow.DataContext;
            searchWindowViewModel.Initialize((tag, exclude) => {
                // タグを検索
                TagList.Clear();
                foreach (var item in TagItem.FilterTag(tag, exclude)) {
                    TagList.Add(new TagItemViewModel(item));
                    if (itemViewModel != null) {
                        var tagString = item.Tag;
                        TagList.Last().IsChecked = itemViewModel.Tags.Contains(tagString);
                    }
                }

            });
            searchWindow.Show();
        });
    }
}

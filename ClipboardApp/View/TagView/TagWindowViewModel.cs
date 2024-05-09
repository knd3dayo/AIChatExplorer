using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
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
            // itemViewModelがnullでない場合、IsShowCommonTagをFalseにする
            IsShowCommonTag = itemViewModel == null;

            ReloadTagList();

        }
        // 共通タグを表示するかどうか
        private bool _isShowCommonTag = false;
        public bool IsShowCommonTag {
            get {
                return _isShowCommonTag;
            }
            set {
                _isShowCommonTag = value;
                ReloadTagList();
                OnPropertyChanged(nameof(IsShowCommonTag));
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
        public SimpleDelegateCommand AddTagCommand => new(AddTagCommandExecute);

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

            TagItem tagItem = new() { Tag = tag };
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
            TagList.Add(new TagItemViewModel(tagItem));
            NewTag = "";
            // LiteDBから再読み込み
            ReloadTagList();

        }
        // LiteDBから再読み込み
        public void ReloadTagList() {
            TagList.Clear();
            IEnumerable<TagItem> commonTagList = ClipboardAppFactory.Instance.GetClipboardDBController().GetTagList();

            // ItemViewModelがnullの場合またはIsShowCommonTagがTrueの場合、commonTagListを表示
            if (IsShowCommonTag) {
                foreach (var item in commonTagList) {
                    TagItemViewModel tagItemViewModel = new(item);
                    tagItemViewModel.IsCommonTag = true;
                    TagList.Add(new TagItemViewModel(item));
                }
            }

            if (itemViewModel == null) {
                return;
            }
            // ItemViewModelがnullでない場合、ItemViewModelのタグを表示
            foreach (var tag in itemViewModel.Tags) {
                // tagがTagListに含まれている場合はそれを取得
                TagItemViewModel? itemViewModel = TagList.FirstOrDefault(x => x.Tag == tag);
                if (itemViewModel != null) {
                    itemViewModel.IsChecked = true;
                }
                if (itemViewModel == null) {
                    TagItem item = new TagItem() { Tag = tag };
                    itemViewModel = new TagItemViewModel(item);
                    itemViewModel.IsChecked = true;
                    TagList.Add(itemViewModel);
                }
            }
        }

        // 選択したタグを削除する。
        public SimpleDelegateCommand DeleteSelectedTagCommand => new((parameter) => {
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

        // すべて選択
        public SimpleDelegateCommand SelectAllCommand => new((parameter) => {
            foreach (var item in TagList) {
                item.IsChecked = true;
            }
            OnPropertyChanged(nameof(TagList));
        });
        // すべて選択解除
        public SimpleDelegateCommand UnselectAllCommand => new((parameter) => {
            foreach (var item in TagList) {
                item.IsChecked = false;
            }
            OnPropertyChanged(nameof(TagList));
        });
        // 共通タグへ追加
        public SimpleDelegateCommand AddCommonTagCommand => new((parameter) => {
            // 共通タグを追加
            foreach (var item in TagList) {
                if (item.IsChecked) {
                    // LiteDBにタグを追加  
                    TagItem tagItem = new() { Tag = item.Tag };
                    ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
                }
            }
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

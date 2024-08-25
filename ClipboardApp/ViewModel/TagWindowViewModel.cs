using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.TagView;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    public class TagWindowViewModel : MyWindowViewModel {

        public ObservableCollection<TagItemViewModel> TagList { get; set; } = [];

        public List<TagItemViewModel> SelectedTagList { get; set; } = [];

        private ClipboardItemViewModel? ClipboardItemViewModel { get; set; }

        private Action? AfterUpdate { get; set; }

        public void Initialize(ClipboardItemViewModel? itemViewModel, Action afterUpdate) {
            ClipboardItemViewModel = itemViewModel;
            AfterUpdate = afterUpdate;

            ReloadTagList();

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
        public SimpleDelegateCommand<string> AddTagCommand => new((tag) => {
            if (string.IsNullOrEmpty(tag)) {
                LogWrapper.Error(StringResources.TagIsEmpty);
                return;
            }
            //tagが既に存在するかチェック
            foreach (var item in TagList) {
                if (item.Tag == tag) {
                    LogWrapper.Error(StringResources.TagAlreadyExists);
                    return;
                }
            }

            TagItem tagItem = new() { Tag = tag };
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(tagItem);
            TagList.Add(new TagItemViewModel(tagItem));
            NewTag = "";
            // LiteDBから再読み込み
            ReloadTagList();

        });

        // LiteDBから再読み込み
        public void ReloadTagList() {
            TagList.Clear();
            IEnumerable<TagItem> tagItems = ClipboardAppFactory.Instance.GetClipboardDBController().GetTagList();
            foreach (var item in tagItems) {
                TagItemViewModel tagItemViewModel = new(item);
                if (ClipboardItemViewModel != null) {
                    var tagString = item.Tag;
                    tagItemViewModel.IsChecked = ClipboardItemViewModel.Tags.Contains(tagString);
                }
                TagList.Add(tagItemViewModel);
            }
            OnPropertyChanged(nameof(TagList));
        }

        // 選択したタグを削除する。
        public SimpleDelegateCommand<object> DeleteSelectedTagCommand => new((parameter) => {
            // 選択中のアイテムを削除

            foreach (var item in SelectedTagList) {
                // LiteDBから削除
                item.TagItem.Delete();
            }
            // LiteDBから再読み込み
            ReloadTagList();
        });


        // OpenAIExecutionModeSelectionChangeCommand
        public SimpleDelegateCommand<RoutedEventArgs> SelectionChangeCommand => new((routedEventArgs) => {
            ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
            // 選択中のアイテムを取得
            SelectedTagList.Clear();
            foreach (TagItemViewModel item in listBox.SelectedItems) {
                SelectedTagList.Add(item);
            }

        });

        // すべて選択
        public SimpleDelegateCommand<object> SelectAllCommand => new((parameter) => {
            foreach (var item in TagList) {
                item.IsChecked = true;
            }
            OnPropertyChanged(nameof(TagList));
        });
        // すべて選択解除
        public SimpleDelegateCommand<object> UnselectAllCommand => new((parameter) => {
            foreach (var item in TagList) {
                item.IsChecked = false;
            }
            OnPropertyChanged(nameof(TagList));
        });

        public SimpleDelegateCommand<Window> OkCommand => new((window) => {
            ClipboardItemViewModel?.UpdateTagList(TagList);
            // 更新後の処理を実行
            AfterUpdate?.Invoke();

            // ウィンドウを閉じる
            window.Close();

        });

        // 検索ウィンドウを開く
        public SimpleDelegateCommand<object> OpenSearchWindowCommand => new((parameter) => {
            TagSearchWindow.OpenTagSearchWindow((tag, exclude) => {
                // タグを検索
                TagList.Clear();
                foreach (var item in TagItem.FilterTag(tag, exclude)) {
                    TagList.Add(new TagItemViewModel(item));
                    if (ClipboardItemViewModel != null) {
                        var tagString = item.Tag;
                        TagList.Last().IsChecked = ClipboardItemViewModel.Tags.Contains(tagString);
                    }
                }

            });
        });
    }
}

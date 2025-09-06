using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using LibPythonAI.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.View.Tag;

namespace LibUIMain.ViewModel.Tag {
    public class TagWindowViewModel : CommonViewModelBase {

        public ObservableCollection<TagItemViewModel> TagList { get; set; } = [];

        public List<TagItemViewModel> SelectedTagList { get; set; } = [];

        private ContentItem? ContentItem { get; set; }

        private Action? AfterUpdate { get; set; }

        public TagWindowViewModel(ContentItem? contentItem, Action afterUpdate) {
            ContentItem = contentItem;
            AfterUpdate = afterUpdate;

            _ = ReloadTagListAsync(); // CS4014エラーを防ぐために、非同期メソッドの呼び出しを明示的に無視します。
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
        public SimpleDelegateCommand<string> AddTagCommand => new(async (tag) => {
            if (string.IsNullOrEmpty(tag)) {
                LogWrapper.Error(CommonStringResources.Instance.TagIsEmpty);
                return;
            }
            //tagが既に存在するかチェック
            foreach (var item in TagList) {
                if (item.Tag == tag) {
                    LogWrapper.Error(CommonStringResources.Instance.TagAlreadyExists);
                    return;
                }
            }

            TagItem tagItem = new(tag) { Tag = tag };
            await tagItem.SaveAsync();

            NewTag = "";
            // LiteDBから再読み込み
            await ReloadTagListAsync();

        });

        // LiteDBから再読み込み
        public async Task ReloadTagListAsync() {
            TagList.Clear();
            List<TagItem> tagItems = await TagItem.GetTagItemsAsync();

            foreach (var item in tagItems) {
                TagItemViewModel tagItemViewModel = new(item);
                if (ContentItem != null) {
                    var tagString = item.Tag;
                    tagItemViewModel.IsChecked = ContentItem.Tags.Contains(tagString);
                }
                TagList.Add(tagItemViewModel);
            }
            OnPropertyChanged(nameof(TagList));
        }

        // 選択したタグを削除する。
        public SimpleDelegateCommand<object> DeleteSelectedTagCommand => new(async (parameter) => {
            // 選択中のアイテムを削除

            foreach (var item in SelectedTagList) {
                // LiteDBから削除
                await item.TagItem.DeleteAsync();
            }
            // LiteDBから再読み込み
            await ReloadTagListAsync();
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
            // TagListのうちIsCheckedがTrueのものを取得
            HashSet<string> tags = new();
            foreach (var item in TagList) {
                if (item.IsChecked) {
                    tags.Add(item.Tag);
                }
            }
            if (ContentItem != null) {
                ContentItem.Tags.Clear();
                ContentItem.Tags.UnionWith(tags);
                ContentItem.SaveAsync();
            }

            // 更新後の処理を実行
            AfterUpdate?.Invoke();

            // ウィンドウを閉じる
            window.Close();

        });

        // 検索ウィンドウを開く
        public SimpleDelegateCommand<object> OpenSearchWindowCommand => new((parameter) => {
            TagSearchWindow.OpenTagSearchWindow(async (tag, exclude) => {
                // タグを検索  
                TagList.Clear();
                var tagItems = await TagItem.FilterTagAsync(tag, exclude);
                foreach (var item in tagItems) {
                    TagList.Add(new TagItemViewModel(item));
                    if (ContentItem != null) {
                        var tagString = item.Tag;
                        TagList.Last().IsChecked = ContentItem.Tags.Contains(tagString);
                    }
                }
            });
        });
    }
}

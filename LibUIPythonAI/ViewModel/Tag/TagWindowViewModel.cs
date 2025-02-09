using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.Content;
using PythonAILib.Model.Tag;
using LibUIPythonAI.View.Tag;
using WpfAppCommon.Utils;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.Tag {
    public class TagWindowViewModel : ChatViewModelBase {

        public ObservableCollection<TagItemViewModel> TagList { get; set; } = [];

        public List<TagItemViewModel> SelectedTagList { get; set; } = [];

        private ContentItem? ContentItem { get; set; }

        private Action? AfterUpdate { get; set; }

        public TagWindowViewModel(ContentItem? contentItem, Action afterUpdate) {
            ContentItem = contentItem;
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
            tagItem.Save();

            TagList.Add(new TagItemViewModel(tagItem));
            NewTag = "";
            // LiteDBから再読み込み
            ReloadTagList();

        });

        // LiteDBから再読み込み
        public void ReloadTagList() {
            TagList.Clear();
            IEnumerable<TagItem> tagItems = TagItem.GetTagList();
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
            // TagListのうちIsCheckedがTrueのものを取得
            HashSet<string> tags = new();
            foreach (var item in TagList) {
                if (item.IsChecked) {
                    tags.Add(item.Tag);
                }
            }
            if (ContentItem != null) {
                ContentItem.Tags = tags;
                ContentItem.Save();
            }

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
                    if (ContentItem != null) {
                        var tagString = item.Tag;
                        TagList.Last().IsChecked = ContentItem.Tags.Contains(tagString);
                    }
                }

            });
        });
    }
}

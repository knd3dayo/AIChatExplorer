using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.View.TagView;
using PythonAILib.Model.Abstract;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    public class EditItemWindowViewModel : ClipboardAppViewModelBase {


        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.Tags ?? []);

                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        private ClipboardFolderViewModel? _folderViewModel;
        public ClipboardFolderViewModel? FolderViewModel {
            get {
                return _folderViewModel;
            }
            set {
                _folderViewModel = value;
                OnPropertyChanged(nameof(FolderViewModel));
            }
        }

        public override void OnActivatedAction() {
            if (FolderViewModel == null) {
                return;
            }
            // StatusText.Readyにフォルダ名を設定
            Tools.StatusText.ReadyText = $"{StringResources.Folder}:[{FolderViewModel.FolderName}]";
            // StatusText.Textにフォルダ名を設定
            Tools.StatusText.Text = $"{StringResources.Folder}:[{FolderViewModel.FolderName}]";
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            set {
                if (value == null) {
                    return;
                }
                title = value;
                OnPropertyChanged(nameof(Title));

            }
        }

        //Tagを文字列に変換したもの
        private string _tagsString = "";
        public string TagsString {
            get {
                return _tagsString;
            }
            set {
                _tagsString = value;
                OnPropertyChanged(nameof(TagsString));
            }
        }

        // 更新後の処理
        private Action _afterUpdate = () => { };

        // IsDrawerOpen
        private bool isDrawerOpen = false;
        public bool IsDrawerOpen {
            get {
                return isDrawerOpen;
            }
            set {
                isDrawerOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }
        // IsMenuDrawerOpen
        private bool isMenuDrawerOpen = false;
        public bool IsMenuDrawerOpen {
            get {
                return isMenuDrawerOpen;
            }
            set {
                isMenuDrawerOpen = value;
                OnPropertyChanged(nameof(IsMenuDrawerOpen));
            }
        }
        // SelectedFile
        private ClipboardItemFile? selectedFile;
        public ClipboardItemFile? SelectedFile {
            get {
                return selectedFile;
            }
            set {
                selectedFile = value;
                OnPropertyChanged(nameof(SelectedFile));
                OnPropertyChanged(nameof(SelectedFileImageVisibility));
            }
        }
        // 選択したファイルの画像のVisible. SelectedFileがNull、Imageがnullの場合はCollapsed
        public Visibility SelectedFileImageVisibility {
            get {
                if (SelectedFile == null || SelectedFile.BitmapImage == null) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        // SelectedImage
        private ImageSource? selectedImage;
        public ImageSource? SelectedImage {
            get {
                return selectedImage;
            }
            set {
                selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }
        public int SelectedImageIndex { get; set; } = 0;

        // SelectedIssueItem
        public IssueItemBase? SelectedIssueItem { get; set; }

        public EditItemWindowViewModel(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {

            FolderViewModel = folderViewModel;
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new(folderViewModel.ClipboardItemFolder.Id);
                ItemViewModel = new ClipboardItemViewModel(folderViewModel, clipboardItem);
                Title = "新規アイテム";
            } else {
                Title = itemViewModel.ClipboardItem.Description;
                ItemViewModel = itemViewModel;
            }
            // ClipboardItemFileがある場合はSelectedFileに設定
            if (ItemViewModel.ClipboardItem.ClipboardItemFiles.Count > 0) {
                SelectedFile = (ClipboardItemFile)ItemViewModel.ClipboardItem.ClipboardItemFiles[0];
            }
            _afterUpdate = afterUpdate;

        }

        // TextWrapping
        public TextWrapping TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping;
            }
        }
        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand<object> AddTagButtonCommand => new((obj) => {

            if (ItemViewModel == null) {
                LogWrapper.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow.OpenTagWindow(ItemViewModel, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.Tags);
            });
        });

        // QAChatButtonCommand
        public SimpleDelegateCommand<object> QAChatButtonCommand => new((obj) => {
            // QAChatControlのDrawerを開く
            ItemViewModel?.OpenOpenAIChatWindowCommand.Execute();
        });

        // Saveコマンド
        public SimpleDelegateCommand<object> SaveCommand => new((obj) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // フォルダに自動処理が設定されている場合は実行
            ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ItemViewModel.ClipboardItem.FolderObjectId);
            ClipboardItem? item = folder?.ApplyAutoProcess(ItemViewModel.ClipboardItem);
            // ClipboardItemを更新
            if (item != null) {
                item.Save();
            } else {
                // 自動処理に失敗した場合はLogWrapper.Info("自動処理に失敗しました");
                LogWrapper.Info("自動処理に失敗しました");
            }
            // 更新後の処理を実行
            _afterUpdate.Invoke();

        });
        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {

            // SaveCommandを実行
            SaveCommand.Execute(null);
            // ウィンドウを閉じる
            window.Close();
        });

        // Issuesの削除
        public SimpleDelegateCommand<object> DeleteIssueCommand => new((parameter) => {
            if (SelectedIssueItem == null) {
                return;
            }
            ItemViewModel?.DeleteIssueCommand.Execute(SelectedIssueItem);
        });

        // IssuesのSelectionChangedイベント発生時の処理
        public SimpleDelegateCommand<RoutedEventArgs> IssueItemSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                IssueItemBase item = (IssueItemBase)dataGrid.SelectedItem;
                SelectedIssueItem = item;
            }

        });



    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.TagView;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    /// <summary>
    /// クリップボードアイテム編集ウィンドウのViewModel
    /// </summary>
    class EditItemWindowViewModel : MyWindowViewModel {

        private readonly TextSelector TextSelector = new();

        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.Tags ?? []);

                OnPropertyChanged(nameof(ItemViewModel));
                OnPropertyChanged(nameof(ImageTabVisibility));
                OnPropertyChanged(nameof(FileTabVisibility));
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
            Tools.StatusText.ReadyText = $"フォルダ名:[{FolderViewModel.FolderName}]";
            // StatusText.Textにフォルダ名を設定
            Tools.StatusText.Text = $"フォルダ名:[{FolderViewModel.FolderName}]";
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

        // イメージタブの表示可否
        public Visibility ImageTabVisibility {
            get {
                return ItemViewModel?.ContentType == ClipboardContentTypes.Image ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // ファイルタブの表示可否
        public Visibility FileTabVisibility {
            get {
                return ItemViewModel?.ContentType == ClipboardContentTypes.Files ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // 更新後の処理
        private Action _afterUpdate = () => {};

        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; } = new();

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

        public void Initialize(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action afterUpdate) {

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
                SelectedFile = ItemViewModel.ClipboardItem.ClipboardItemFiles[0];
            }
            // ClipboardItemImageがある場合はSelectedImageに設定
            if (ItemViewModel.Images.Count > 0) {
                SelectedImage = ItemViewModel.Images[0];
            }
            _afterUpdate = afterUpdate;

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

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);

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

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });

    }
}

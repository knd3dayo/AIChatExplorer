using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SelectVectorDBView;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Folder;
using ClipboardApp.ViewModel.Search;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using QAChat;
using QAChat.Control;
using WpfAppCommon.Utils;


namespace ClipboardApp
{
    public partial class MainWindowViewModel : ClipboardAppViewModelBase {
        public MainWindowViewModel() {
            Init();

        }
        private void Init() {

            ActiveInstance = this;

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            ClipboardAppPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);
            QAChatManager.Init(configParams);
            // フォルダの初期化
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder);
            InitClipboardFolders();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            IBackupController backupController = ClipboardAppFactory.Instance.GetBackupController();
            backupController.BackupNow();
        }

        private void InitClipboardFolders() {
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder);

            ClipboardItemFolders.Add(RootFolderViewModel);
            ClipboardItemFolders.Add(new SearchFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            ClipboardItemFolders.Add(new ChatFolderViewModel(this, ClipboardFolder.ChatRootFolder));

            ClipboardItemFolders.Add(ImageCheckRootFolderViewModel);

            OnPropertyChanged(nameof(ClipboardItemFolders));
        }

        public static MainWindowViewModel? ActiveInstance { get; set; }

        /// <summary>
        /// ウィンドウがアクティブになった時の処理
        /// </summary>
        public override void OnActivatedAction() {

            ActiveInstance = this;

        }
        // RootFolderのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 画像チェックフォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ImageCheckFolderViewModel ImageCheckRootFolderViewModel { get; private set; }

        public void ReLoadRootFolders() {
            foreach (var folder in ClipboardItemFolders) {
                folder.LoadFolderCommand.Execute();
            }
        }

        // ClipboardController
        public static ClipboardController ClipboardController { get; } = new();

        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        // Progress Indicatorの表示フラグ
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitor ? StringResources.StopClipboardWatch : StringResources.StartClipboardWatch;
            }
        }
        // クリップボード監視を開始、終了するフラグ
        public bool IsClipboardMonitor { get; set; } = false;

        // Windows通知監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string WindowsNotificationMonitorButtonText {
            get {
                return IsWindowsNotificationMonitor ? StringResources.StopNotificationWatch : StringResources.StartNotificationWatch;
            }
        }
        // Windows通知監視が開始、終了するフラグ
        public bool IsWindowsNotificationMonitor { get; set; } = false;

        // ClipboardFolder

        public ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = [];

        // Cutフラグ
        private bool _CutFlag = false;
        public bool CutFlag {
            get {
                return _CutFlag;
            }
            set {
                _CutFlag = value;
                OnPropertyChanged(nameof(CutFlag));
            }
        }

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ClipboardItemViewModel> _selectedItems = [];
        public ObservableCollection<ClipboardItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem;
        public ClipboardItemViewModel? SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        // 選択中のフォルダ
        private ClipboardFolderViewModel? _selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        /// <summary>
        /// コピーされたアイテム
        /// </summary>
        // Ctrl + C or X が押された時のClipboardItem
        private ObservableCollection<ClipboardItemViewModel> _copiedItems = [];
        public ObservableCollection<ClipboardItemViewModel> CopiedItems {
            get {
                return _copiedItems;
            }
            set {
                _copiedItems = value;
                // MainWindowModelのCopiedItemsにもセット
                OnPropertyChanged(nameof(CopiedItems));
            }
        }

        /// <summary>
        /// コピーされたアイテムのフォルダ
        /// </summary>
        // Ctrl + C or X  が押された時のClipboardItemFolder
        private ClipboardFolderViewModel? _copiedItemFolder;
        public ClipboardFolderViewModel? CopiedItemFolder {
            get {
                return _copiedItemFolder;
            }
            set {
                _copiedItemFolder = value;
                OnPropertyChanged(nameof(CopiedItemFolder));
            }
        }

        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(TextWrapping));
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public static Visibility PreviewModeVisibility {
            get {
                return ClipboardAppConfig.Instance.PreviewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return ClipboardAppConfig.Instance.PreviewMode;
            }
            set {
                ClipboardAppConfig.Instance.PreviewMode = value;
                // Save
                ClipboardAppConfig.Instance.Save();

                OnPropertyChanged(nameof(PreviewMode));
                OnPropertyChanged(nameof(PreviewModeVisibility));
                // アプリケーション再起動後に反映されるようにメッセージを表示
                MessageBox.Show(StringResources.DisplayModeWillChangeWhenYouRestartTheApplication, StringResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        public TextSelector TextSelector { get; } = new();

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


        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.Instance.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static QAChatStartupProps CreateQAChatStartupProps(ClipboardItem clipboardItem) {


            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(clipboardItem) {
                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    if (ActiveInstance == null) {
                        LogWrapper.Error("MainWindowViewModelがNullです");
                        return;
                    }
                    SelectVectorDBWindow.OpenSelectVectorDBWindow(ActiveInstance.RootFolderViewModel, (selectedItems) => {
                        foreach (var item in selectedItems) {
                            vectorDBItems.Add(item);
                        }
                    });

                },
                // ペーストアクション
                AddContentItemCommandAction = (action) => {
                    // MainWindowViewModel.ActiveInstanceがnullの場合は何もしない
                    if (ActiveInstance == null) {
                        return;
                    }
                    List<ClipboardItem> result = [];
                    PasteFromClipboardCommandExecute(ActiveInstance, false, (newItems) => {
                        // newItemsをContentItemBaseに変換
                        List<ContentItem> contentItemBases = [.. newItems];
                        action(contentItemBases);
                    });
                },
                // 選択中のアイテムを開くアクション
                OpenSelectedItemCommand = (item) => {
                    // MainWindowViewModel.ActiveInstanceがnullの場合は何もしない
                    if (MainWindowViewModel.ActiveInstance == null) {
                        return;
                    }
                    clipboardItem = (ClipboardItem)item;
                    // item からClipboardFolderViewModelを取得
                    ClipboardFolderViewModel folderViewModel = new(MainWindowViewModel.ActiveInstance, clipboardItem.GetFolder());
                    // item からClipboardItemViewModelを取得
                    ClipboardItemViewModel itemViewModel = new(folderViewModel, clipboardItem);
                    EditItemWindow.OpenEditItemWindow(folderViewModel, itemViewModel, () => { });

                },
                // Saveアクション
                SaveCommand = (item) => {
                    clipboardItem = (ClipboardItem)item;
                    // ClipboardItemを保存
                    clipboardItem.Save();

                    //ChatHistoryItemがある場合は保存
                    // チャット履歴用のItemの設定
                    // チャット履歴を保存する。チャット履歴に同一階層のフォルダを作成して、Itemをコピーする。
                    ClipboardFolder chatFolder = ClipboardFolder.GetAnotherTreeFolder(clipboardItem.GetFolder(), ClipboardFolder.ChatRootFolder, true);
                    ClipboardItem chatHistoryItem = new(chatFolder.Id);

                    clipboardItem.CopyTo(chatHistoryItem);
                    chatHistoryItem.Save();
                }
            };

            return props;
        }

    }

}

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.ViewModel;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.Control;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using ClipboardApp.Factory;
using ClipboardApp.Factory.Default;
using ClipboardApp.Model;
using QAChat;


namespace ClipboardApp {
    public partial class MainWindowViewModel : MyWindowViewModel {
        public MainWindowViewModel() {
            // 旧バージョンのRootFolderの移行
            ClipboardFolder.MigrateRootFolder();
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder);
            Init();

        }
        private void Init() {

            ActiveInstance = this;

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            PythonAILibManager.Init(
                ClipboardAppConfig.ActualLang,
                ClipboardAppConfig.PythonDllPath,
                ClipboardAppConfig.PythonVenvPath,
                ClipboardAppFactory.Instance.GetClipboardDBController(),
                LogWrapper.Info,
                LogWrapper.Warn,
                LogWrapper.Error
            );

            // フォルダの初期化
            InitClipboardFolders();

            // PromptItemの初期化
            PromptItem.InitSystemPromptItems();

            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();

            // DBのバックアップの取得
            IBackupController backupController = ClipboardAppFactory.Instance.GetBackupController();
            backupController.BackupNow();
        }

        private void InitClipboardFolders() {

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
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }
        // 画像チェックフォルダのClipboardViewModel
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
                return ClipboardAppConfig.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Save();
                OnPropertyChanged(nameof(TextWrapping));
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public static Visibility PreviewModeVisibility {
            get {
                return ClipboardAppConfig.PreviewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return ClipboardAppConfig.PreviewMode;
            }
            set {
                ClipboardAppConfig.PreviewMode = value;
                // Save
                ClipboardAppConfig.Save();

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
                return ClipboardAppConfig.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static QAChatStartupProps CreateQAChatStartupProps(ClipboardItem clipboardItem) {


            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(clipboardItem) {
                // ベクトルDBアイテムを開くアクション
                OpenVectorDBItemAction = (vectorDBItem) => {
                    VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                    EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
                },
                // ベクトルDBアイテムを選択するアクション
                SelectVectorDBItemsAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                        vectorDBItems.Add(selectedItem);
                    });
                },
                // フォルダ選択アクション
                SelectFolderAction = (vectorDBItems) => {
                    if (MainWindowViewModel.ActiveInstance == null) {
                        LogWrapper.Error("MainWindowViewModelがNullです");
                        return;
                    }
                    FolderSelectWindow.OpenFolderSelectWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, (folderViewModel) => {
                        vectorDBItems.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
                    });

                },
                // ペーストアクション
                PasteFromClipboardCommandAction = (action) => {
                    // MainWindowViewModel.ActiveInstanceがnullの場合は何もしない
                    if (MainWindowViewModel.ActiveInstance == null) {
                        return;
                    }
                    List<ClipboardItem> result = [];
                    MainWindowViewModel.PasteFromClipboardCommandExecute(MainWindowViewModel.ActiveInstance, false, (newItems) => {
                        // newItemsをContentItemBaseに変換
                        List<ContentItemBase> contentItemBases = [.. newItems];
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
                    // MainWindowViewModel.ActiveInstanceがnullの場合は何もしない
                    if (MainWindowViewModel.ActiveInstance == null) {
                        return;
                    }
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

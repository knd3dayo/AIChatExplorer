using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Utils;
using ClipboardApp.View.ExportImportView;
using ClipboardApp.ViewModel.ClipboardItemView;
using Microsoft.WindowsAPICodePack.Dialogs;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {
    public abstract class ClipboardFolderBase(ClipboardFolder clipboardItemFolder) : ClipboardAppViewModelBase {

        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public virtual int DefaultNextLevel { get; } = 5;

        #region abstract

        // フォルダ作成コマンドの実装
        public abstract void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate);

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate);

        public abstract void CreateItemCommandExecute();

        public abstract ClipboardItemViewModel CreateItemViewModel(ClipboardItem item);

        public abstract void OpenItemCommandExecute(ClipboardItemViewModel item);

        public abstract void PasteClipboardItemCommandExecute(MainWindowViewModel.CutFlagEnum CutFlag, IEnumerable<object> items, ClipboardFolderViewModel toFolder);

        public abstract void MergeItemCommandExecute(ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems);

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public abstract ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder);

        public abstract ObservableCollection<MenuItem> FolderMenuItems { get; }
        #endregion


        // コンストラクタ

        public ClipboardFolder ClipboardItemFolder { get; } = clipboardItemFolder;

        // Description
        public string Description {
            get {
                return ClipboardItemFolder.Description;
            }
            set {
                ClipboardItemFolder.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // Items
        public ObservableCollection<ClipboardItemViewModel> Items { get; } = [];

        // 子フォルダ
        public ObservableCollection<ClipboardFolderViewModel> Children { get; set; } = [];

        public string FolderName {
            get {
                return ClipboardItemFolder.FolderName;
            }
            set {
                ClipboardItemFolder.FolderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }
        public string FolderPath {
            get {
                return ClipboardItemFolder.FolderPath;
            }
        }

        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                return ClipboardItemFolder.IsRootFolder == false;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか xamlで使う
        public bool IsEditVisible {
            get {
                // RootFolderは編集不可
                return ClipboardItemFolder.IsRootFolder == false;
            }
        }

        public ClipboardFolderViewModel? ParentFolderViewModel { get; set; }

        private void UpdateStatusText() {
            string message = $"{StringResources.Folder}[{FolderName}]";
            // AutoProcessRuleが設定されている場合
            var rules = AutoProcessRuleController.GetAutoProcessRules(ClipboardItemFolder);
            if (rules.Count > 0) {
                message += $" {StringResources.AutoProcessingIsSet}[";
                foreach (AutoProcessRule item in rules) {
                    message += item.RuleName + " ";
                }
                message += "]";
            }

            // folderが検索フォルダの場合
            SearchRule? searchConditionRule = ClipboardFolderUtil.GlobalSearchCondition;
            if (ClipboardItemFolder.FolderType == FolderTypeEnum.Search) {
                searchConditionRule = SearchRuleController.GetSearchRuleByFolder(ClipboardItemFolder);
            }
            SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
            // SearchConditionがNullでなく、 Emptyでもない場合
            if (searchCondition != null && !searchCondition.IsEmpty()) {
                message += $" {StringResources.SearchCondition}[";
                message += searchCondition.ToStringSearchCondition();
                message += "]";
            }
            Tools.StatusText.ReadyText = message;
            Tools.StatusText.Text = message;
        }


        //-----コマンド
        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // フォルダー保存コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> SaveFolderCommand => new((folderViewModel) => {
            ClipboardItemFolder.Save<ClipboardFolder, ClipboardItem>();
        });

        // アイテム削除コマンド
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteItemCommand => new((item) => {
            item.DeleteItemCommand.Execute();
            Items.Remove(item);

        });
        // アイテム保存コマンド
        public SimpleDelegateCommand<ClipboardItemViewModel> AddItemCommand => new((item) => {
            ClipboardItemFolder.AddItem(item.ClipboardItem);
        });


        // 新規フォルダ作成コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> CreateFolderCommand => new((folderViewModel) => {

            CreateFolderCommandExecute(folderViewModel, () => {
                // 親フォルダを保存
                folderViewModel.ClipboardItemFolder.Save<ClipboardFolder, ClipboardItem>();
                folderViewModel.LoadFolderCommand.Execute();

            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((folderViewModel) => {

            EditFolderCommandExecute(folderViewModel, () => {
                //　フォルダを保存
                folderViewModel.ClipboardItemFolder.Save<ClipboardFolder, ClipboardItem>();
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.FolderEdited);
            });
        });

        // フォルダ内のアイテムをJSON形式でバックアップする処理
        public SimpleDelegateCommand<object> BackupItemsFromFolderCommand => new((parameter) => {
            DirectoryInfo directoryInfo = new("export");
            // exportフォルダが存在しない場合は作成
            if (!Directory.Exists("export")) {
                directoryInfo = Directory.CreateDirectory("export");
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.ClipboardItemFolder.Id.ToString() + ".json";

            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFolderPlease,
                InitialDirectory = directoryInfo.FullName,
                // デフォルトのファイル名を設定
                DefaultFileName = fileName,
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                string resultFilePath = dialog.FileName;
                this.ClipboardItemFolder.ExportItemsToJson(resultFilePath);
                // フォルダ内のアイテムを読み込む
                LogWrapper.Info(CommonStringResources.Instance.FolderExported);
            }
        });

        // フォルダ内のアイテムをJSON形式でリストアする処理
        public SimpleDelegateCommand<object> RestoreItemsToFolderCommand => new((parameter) => {
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFolderPlease,
                InitialDirectory = @".",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                string filaPath = dialog.FileName;
                // ファイルを読み込む
                string jsonString = File.ReadAllText(filaPath);
                this.ClipboardItemFolder.ImportItemsFromJson(jsonString);
                // フォルダ内のアイテムを読み込む
                this.LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.FolderImported);
            }
        });

        // ExportImportFolderCommand
        public SimpleDelegateCommand<ClipboardFolderViewModel> ExportImportFolderCommand => new((folderViewModel) => {
            // ExportImportFolderWindowを開く
            ExportImportWindow.OpenExportImportFolderWindow(folderViewModel, () => {
                // ファイルを再読み込み
                folderViewModel.LoadFolderCommand.Execute();
            });
        });

        // ベクトルのリフレッシュ
        public SimpleDelegateCommand<object> RefreshVectorDBCollectionCommand => new((parameter) => {
            ClipboardItemFolder.RefreshVectorDBCollection<ClipboardItem>();
        });

        // --------------------------------------------------------------
        // 2024/04/07 以下の処理はフォルダ更新後の再読み込み対応済み
        // --------------------------------------------------------------

        public SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadChildren(DefaultNextLevel);
            int count = Children.Count;
            LoadItems();
            UpdateStatusText();
        });


        /// <summary>
        /// フォルダ削除コマンド
        /// フォルダを削除した後に、RootFolderをリロードする処理を行う。
        /// </summary>
        /// <param name="parameter"></param>        
        public SimpleDelegateCommand<ClipboardFolderViewModel> DeleteFolderCommand => new((folderViewModel) => {

            if (folderViewModel.ClipboardItemFolder.Id == ClipboardFolderUtil.RootFolder.Id
                || folderViewModel.FolderPath == ClipboardFolderUtil.SEARCH_ROOT_FOLDER_NAME) {
                LogWrapper.Error(StringResources.RootFolderCannotBeDeleted);
                return;
            }

            // フォルダ削除するかどうか確認
            if (MessageBox.Show(StringResources.ConfirmDeleteFolder, StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            // 親フォルダを取得
            ClipboardFolderViewModel? parentFolderViewModel = folderViewModel.ParentFolderViewModel;

            folderViewModel.ClipboardItemFolder.Delete();

            // 親フォルダが存在する場合は、親フォルダを再読み込み
            if (parentFolderViewModel != null) {
                parentFolderViewModel.LoadFolderCommand.Execute();
            }

            LogWrapper.Info(StringResources.FolderDeleted);
        });

        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(ClipboardFolderViewModel folderViewModel) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                foreach (ClipboardItemViewModel item in folderViewModel.Items) {
                    if (item.IsPinned) {
                        continue;
                    }
                    // item.ClipboardItemを削除
                    item.DeleteItemCommand.Execute();
                }

                // フォルダ内のアイテムを読み込む
                folderViewModel.LoadFolderCommand.Execute(null);
                LogWrapper.Info(CommonStringResources.Instance.DeletedItems);
            }
        }

        //クリップボードアイテムを開く
        public SimpleDelegateCommand<ClipboardItemViewModel> OpenItemCommand => new((itemViewModel) => {

            OpenItemCommandExecute(itemViewModel);

        });


        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public virtual async void LoadChildren(int nestLevel = 5) {
            try {
                MainWindowViewModel.ActiveInstance.UpdateIndeterminate(true);
                await Task.Run(() => {
                Children = [];

                // Childrenがクリアされていない場合
                if (Children.Count > 0) {
                    throw new Exception("Children is not cleared");
                }
                foreach (var child in ClipboardItemFolder.GetChildren<ClipboardFolder>()) {
                    if (child == null) {
                        continue;
                    }
                    ClipboardFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                    // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                    if (nestLevel > 0) {
                        childViewModel.LoadChildren(nestLevel - 1);
                    }
                    Children.Add(childViewModel);
                }
            });
            } finally {
                MainWindowViewModel.ActiveInstance.UpdateIndeterminate(false);
            }
            OnPropertyChanged(nameof(Children));

        }
        // LoadItems
        public virtual async void LoadItems() {
            Items.Clear();
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<ClipboardItem> _items = [];
            try {
                MainWindowViewModel.ActiveInstance.UpdateIndeterminate(true);
                await Task.Run(() => {
                    _items = ClipboardItemFolder.Items;
                });
                foreach (ClipboardItem item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            } finally {
                MainWindowViewModel.ActiveInstance.UpdateIndeterminate(false);
            }
        }


    }
}

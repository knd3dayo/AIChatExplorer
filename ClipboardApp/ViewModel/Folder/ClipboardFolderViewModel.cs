using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.View.ExportImportView;
using ClipboardApp.ViewModel.Folder;
using Microsoft.WindowsAPICodePack.Dialogs;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace ClipboardApp.ViewModel {
    public partial class ClipboardFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardAppViewModelBase {

        // ClipboardFolder
        public ClipboardFolder ClipboardItemFolder { get; } = clipboardItemFolder;

        // 親フォルダ
        public ClipboardFolderViewModel? ParentFolderViewModel { get; set; }

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
        public ObservableCollection<ClipboardFolderViewModel> Children { get; } = [];

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
            SearchRule? searchConditionRule = ClipboardFolder.GlobalSearchCondition;
            if (ClipboardItemFolder.FolderType == ClipboardFolder.FolderTypeEnum.Search) {
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
            ClipboardItemFolder.Save();
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
                folderViewModel.ClipboardItemFolder.Save();
                folderViewModel.LoadFolderCommand.Execute();

            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((folderViewModel) => {

            EditFolderCommandExecute(folderViewModel, () => {
                //　フォルダを保存
                folderViewModel.ClipboardItemFolder.Save();
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.FolderEdited);
            });
        });


        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand<object> FolderSelectWindowSelectFolderCommand => new(FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

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
        SimpleDelegateCommand<ClipboardFolderViewModel> ExportImportFolderCommand => new((folderViewModel) => {
            // ExportImportFolderWindowを開く
            ExportImportWindow.OpenExportImportFolderWindow(folderViewModel, () => {
                // ファイルを再読み込み
                ReloadCommandExecute(folderViewModel);
            });
        });

        //フォルダを再読み込みする処理
        public static void ReloadCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {
            clipboardItemFolder.LoadFolderCommand.Execute();
            LogWrapper.Info(CommonStringResources.Instance.Reloaded);
        }

        // --------------------------------------------------------------
        // 2024/04/07 以下の処理はフォルダ更新後の再読み込み対応済み
        // --------------------------------------------------------------

        public SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            MainWindowViewModel.ActiveInstance.IsIndeterminate = true;
            try {
                LoadChildren();
                LoadItems();

                UpdateStatusText();
            } finally {
                MainWindowViewModel.ActiveInstance.IsIndeterminate = false;
            }
        });


        /// <summary>
        /// フォルダ削除コマンド
        /// フォルダを削除した後に、RootFolderをリロードする処理を行う。
        /// </summary>
        /// <param name="parameter"></param>        
        public SimpleDelegateCommand<ClipboardFolderViewModel> DeleteFolderCommand => new((folderViewModel) => {

            if (folderViewModel.ClipboardItemFolder.Id == ClipboardFolder.RootFolder.Id
                || folderViewModel.FolderPath == ClipboardFolder.SEARCH_ROOT_FOLDER_NAME) {
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


    }
}

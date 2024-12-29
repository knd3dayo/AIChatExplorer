using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItem;
using ClipboardApp.View.Folder;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Content;
using Microsoft.WindowsAPICodePack.Dialogs;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Search;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;


namespace ClipboardApp.ViewModel {
    public class ClipboardFolderViewModel(ClipboardFolder clipboardItemFolder) : ContentFolderViewModel(clipboardItemFolder) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItem item) {
            return new ClipboardItemViewModel(this, item);
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public virtual ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            var childFolderViewModel = new ClipboardFolderViewModel(childFolder) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ClipboardFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        public bool IsNameEditable {
            get {
                return Folder.IsRootFolder == false;
            }

        }
        // フォルダ作成コマンドの実装
        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            childFolder.FolderType = FolderTypeEnum.Normal;
            ClipboardFolderViewModel childFolderViewModel = new(childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

        public override void CreateItemCommandExecute() {
            /**
            EditItemWindow.OpenEditItemWindow(this, null, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Added);
            });
            **/
            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, null,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(StringResources.Edited);
                });

            ClipboardAppTabContainer container = new("New Item", editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);


        }
        public override void OpenItemCommandExecute(ContentItemViewModel item) {
            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, item,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(StringResources.Edited);
                });

            ClipboardAppTabContainer container = new(item.ContentItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);


        }

        /// <summary>
        /// Ctrl + V が押された時の処理
        /// コピー中のアイテムを選択中のフォルダにコピー/移動する
        /// 貼り付け後にフォルダ内のアイテムを再読み込む
        /// 
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="item"></param>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>

        public virtual void PasteClipboardItemCommandExecute(MainWindowViewModel.CutFlagEnum CutFlag,
            IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ClipboardItemViewModel itemViewModel) {
                    ContentItem clipboardItem = itemViewModel.ContentItem;
                    if (CutFlag == MainWindowViewModel.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveToFolder(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ClipboardFolder folder = (ClipboardFolder)folderViewModel.Folder;
                    if (CutFlag == MainWindowViewModel.CutFlagEnum.Folder) {
                        // Cutフラグが立っている場合はコピー元のフォルダを削除する
                        folder.MoveTo(toFolder.Folder);
                        // 元のフォルダの親フォルダを再読み込み
                        folderViewModel.ParentFolderViewModel?.LoadFolderCommand.Execute();
                    }
                }

            }
            toFolder.LoadFolderCommand.Execute();

            LogWrapper.Info(StringResources.Pasted);
        }

        public virtual void MergeItemCommandExecute(
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems) {

            if (selectedItems.Count < 2) {
                LogWrapper.Error(StringResources.SelectTwoItemsToMerge);
                return;
            }
            selectedItems[0].MergeItems([.. selectedItems]);

            // フォルダ内のアイテムを再読み込み
            folderViewModel.LoadFolderCommand.Execute();
            LogWrapper.Info(StringResources.Merged);

        }

        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(ContentFolderViewModel folderViewModel) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                foreach (ClipboardItemViewModel item in folderViewModel.Items) {
                    if (item.IsPinned) {
                        continue;
                    }
                    // item.ClipboardItemを削除
                    item.Commands.DeleteItemCommand.Execute();
                }

                // フォルダ内のアイテムを読み込む
                folderViewModel.LoadFolderCommand.Execute(null);
                LogWrapper.Info(CommonStringResources.Instance.DeletedItems);
            }
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public virtual async void LoadChildren(int nestLevel = 5) {
            try {
                UpdateIndeterminate(true);
                // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
                List<ContentFolderViewModel> _children = [];
                await Task.Run(() => {
                    foreach (var child in Folder.GetChildren<ClipboardFolder>()) {
                        if (child == null) {
                            continue;
                        }
                        ClipboardFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                        if (nestLevel > 0) {
                            childViewModel.LoadChildren(nestLevel - 1);
                        }
                        _children.Add(childViewModel);
                    }
                });

                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));

            } finally {
                UpdateIndeterminate(false);
            }
        }
        // LoadItems
        public virtual async void LoadItems() {
            Items.Clear();
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<Model.ClipboardItem> _items = [];
            try {
                UpdateIndeterminate(true);
                await Task.Run(() => {
                    _items = Folder.GetItems<ClipboardItem>();
                });
                foreach (ContentItem item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            } finally {
                UpdateIndeterminate(false);
            }
        }
        public override void UpdateIndeterminate(bool isIndeterminate) {
            MainWindowViewModel.Instance.UpdateIndeterminate(isIndeterminate);
        }

        public override SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadChildren(DefaultNextLevel);
            int count = Children.Count;
            LoadItems();
            UpdateStatusText();
        });

        protected virtual void UpdateStatusText() {
            string message = $"{StringResources.Folder}[{FolderName}]";

            if (Folder is ClipboardFolder clipboardFolder) {
                // AutoProcessRuleが設定されている場合
                var rules = AutoProcessRuleController.GetAutoProcessRules(clipboardFolder);
                if (rules.Count > 0) {
                    message += $" {StringResources.AutoProcessingIsSet}[";
                    foreach (AutoProcessRule item in rules) {
                        message += item.RuleName + " ";
                    }
                    message += "]";
                }

                // folderが検索フォルダの場合
                SearchRule? searchConditionRule = FolderManager.GlobalSearchCondition;
                if (clipboardFolder.FolderType == FolderTypeEnum.Search) {
                    searchConditionRule = SearchRuleController.GetSearchRuleByFolder(clipboardFolder);
                }
                SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
                // SearchConditionがNullでなく、 Emptyでもない場合
                if (searchCondition != null && !searchCondition.IsEmpty()) {
                    message += $" {StringResources.SearchCondition}[";
                    message += searchCondition.ToStringSearchCondition();
                    message += "]";
                }
            }

            Tools.StatusText.ReadyText = message;
            Tools.StatusText.Text = message;
        }


        //-----コマンド
        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------


        // フォルダ内のアイテムをJSON形式でバックアップする処理
        public SimpleDelegateCommand<object> BackupItemsFromFolderCommand => new((parameter) => {
            if (Folder is not ClipboardFolder clipboardFolder) {
                return;
            }
            DirectoryInfo directoryInfo = new("export");
            // exportフォルダが存在しない場合は作成
            if (!Directory.Exists("export")) {
                directoryInfo = Directory.CreateDirectory("export");
            }
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.Folder.Id.ToString() + ".json";

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
                clipboardFolder.ExportItemsToJson(resultFilePath);
                // フォルダ内のアイテムを読み込む
                LogWrapper.Info(CommonStringResources.Instance.FolderExported);
            }
        });

        // フォルダ内のアイテムをJSON形式でリストアする処理
        public SimpleDelegateCommand<object> RestoreItemsToFolderCommand => new((parameter) => {
            if (Folder is not ClipboardFolder clipboardFolder) {
                return;
            }
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
                clipboardFolder.ImportItemsFromJson(jsonString);
                // フォルダ内のアイテムを読み込む
                this.LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.FolderImported);
            }
        });


    }
}

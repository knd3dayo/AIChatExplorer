using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using LibUIPythonAI.View.Item;
using ClipboardApp.ViewModel.Main;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Content;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using LibUIPythonAI.Resource;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using ClipboardApp.Common;


namespace ClipboardApp.ViewModel.Folders.Clipboard {
    public class ClipboardFolderViewModel(ClipboardFolder clipboardItemFolder) : ContentFolderViewModel(clipboardItemFolder) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItem item) {
            return new ClipboardItemViewModel(this, item);
        }

        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel;
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
            ClipboardItem clipboardItem = new(Folder.Id);
            ContentItemViewModel ItemViewModel = new ClipboardItemViewModel(this, clipboardItem);


            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, ItemViewModel,
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

        public virtual void PasteClipboardItemCommandExecute(ClipboardController.CutFlagEnum CutFlag,
            IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ClipboardItemViewModel itemViewModel) {
                    ContentItem clipboardItem = itemViewModel.ContentItem;
                    if (CutFlag == ClipboardController.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveToFolder(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ClipboardFolder folder = (ClipboardFolder)folderViewModel.Folder;
                    if (CutFlag == ClipboardController.CutFlagEnum.Folder) {
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

        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理

        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(ContentFolderViewModel folderViewModel) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                MainWindowViewModel.Instance.UpdateIndeterminate(true);
                List<Task> taskList = [];
                foreach (ClipboardItemViewModel item in folderViewModel.Items) {
                    if (item.IsPinned) {
                        continue;
                    }
                    Task task = Task.Run(() => {
                        // item.ClipboardItemを削除
                        ClipboardItemViewModelCommands commands = new();
                        commands.DeleteItemCommand.Execute(item);
                    });
                    taskList.Add(task);
                }
                // 全てのタスクが終了したら後続処理を実行
                Task.WhenAll(taskList).ContinueWith((task) => {
                    // フォルダ内のアイテムを読み込む
                    MainUITask.Run(() => {
                        folderViewModel.LoadFolderCommand.Execute(null);
                    });
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                    LogWrapper.Info(CommonStringResources.Instance.DeletedItems);
                });
            }
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected virtual void LoadChildren(int nestLevel = 5) {
            UpdateIndeterminate(true);
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ContentFolderViewModel> _children = [];
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
            MainUITask.Run(() => {
                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            });
        }
        // LoadItems
        protected virtual void LoadItems() {
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<ClipboardItem> _items = Folder.GetItems<ClipboardItem>();
            MainUITask.Run(() => {
                Items.Clear();
                foreach (ContentItem item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            });
        }

        #endregion

        public override void UpdateIndeterminate(bool isIndeterminate) {
            MainWindowViewModel.Instance.UpdateIndeterminate(isIndeterminate);
        }

        public override void LoadFolder(Action afterUpdate) {
            UpdateIndeterminate(true);
            Task.Run(() => {
                LoadChildren(DefaultNextLevel);
                LoadItems();
            }).ContinueWith((task) => {
                afterUpdate?.Invoke();
                MainUITask.Run(() => {
                    UpdateIndeterminate(false);
                    UpdateStatusText();
                });
            });
        }

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

            StatusText.Instance.ReadyText = message;
            StatusText.Instance.Text = message;
        }
    }
}

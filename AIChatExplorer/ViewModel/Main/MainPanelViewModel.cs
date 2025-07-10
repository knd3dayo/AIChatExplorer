using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Common;
using LibPythonAI.Model.Content;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelViewModel : CommonViewModelBase {

        public MainPanelViewModel(CommonViewModelCommandExecutes commands) : base() {
            Commands = commands;
            MainPanelTreeViewControlViewModel = new MainPanelTreeViewControlViewModel(Commands);
            MainPanelDataGridViewControlViewModel = new MainPanelDataGridViewControlViewModel(Commands);
            CommonViewModelProperties.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                    MainPanelDataGridViewControlViewModel.UpdateView();
                }
            };
        }
        private TabControl? MyTabControl { get; set; }

        public override void OnLoadedAction() {
            base.OnLoadedAction();
            MyTabControl = ThisUserControl?.FindName("MyTabControl") as TabControl;
            MainPanelDataGridViewControlViewModel.MyTabControl = MyTabControl;

            MainPanelTreeViewControlViewModel.FolderTreeView = ThisUserControl?.FindName("FolderTreeView") as TreeView;
        }

        public CommonViewModelCommandExecutes Commands { get; set; }
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; }

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; }


        // ShowProperties
        public bool ShowProperties {
            get {
                return PythonAILibManager.Instance.ConfigParams.IsShowProperties();
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateShowProperties(value);

                OnPropertyChanged(nameof(ShowProperties));
                OnPropertyChanged(nameof(PropertiesVisibility));
            }
        }


        // PropertiesVisibility
        public Visibility PropertiesVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(ShowProperties);

        // MarkdownViewVisibility
        public Visibility MarkdownViewVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

    }


    public class MainPanelTreeViewControlViewModel : CommonViewModelBase {

        private CommonViewModelCommandExecutes Commands { get; set; }

        // constructor
        public MainPanelTreeViewControlViewModel(CommonViewModelCommandExecutes commands) {
            Commands = commands;
        }
        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public Action<ContentFolderViewModel> SelectedFolderChangedAction { get; set; } = (selectedFolder) => { };

        // Null許容型
        [AllowNull]
        public FolderViewModelManagerBase RootFolderViewModelContainer { get; set; }

        // 選択中のフォルダ
        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                if (value == null) {
                    _selectedFolder = null;
                } else {
                    _selectedFolder = value;
                }
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }


        // Ctrl + C or X  が押された時のApplicationItemFolder
        private ContentFolderViewModel? _copiedFolder;
        public ContentFolderViewModel? CopiedFolder {
            get {
                return _copiedFolder;
            }
            set {
                _copiedFolder = value;
                OnPropertyChanged(nameof(CopiedFolder));
            }
        }
        public TreeView? FolderTreeView { get; set; }

        #region フォルダツリーのInputBinding用のコマンド
        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            AppViewModelCommandExecutes.ReloadFolderCommandExecute(this.SelectedFolder,
                () => {
                    UpdateIndeterminateAction(true);
                },
                () => {
                    UpdateIndeterminateAction(false);
                }
                );
        });

        // アイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            this.SelectedFolder.CreateItemCommandExecute();
        });

        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand<object> PasteCommand => new((parameter) => {
            Commands.PasteFromClipboardCommandExecute();
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutFolderCommand => new((parameter) => {
            AppViewModelCommandExecutes.CutFolderCommandExecute(this);
        });


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel applicationItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;
            SelectedFolder = applicationItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.FolderCommands.LoadFolderCommand.Execute();
                SelectedFolderChangedAction(SelectedFolder);
            }
        });
        #endregion

        // SelectedTreeViewItemChangeCommandExecute
        public void SelectedTreeViewItemChangeCommandExecute(ContentFolderViewModel folder) {
            if (FolderTreeView == null) {
                LogWrapper.Error("FolderTreeView is null.");
                return;
            }
            ItemsControl itemsControl = FolderTreeView;

            List<ContentFolderViewModel> items = [];
            // folderからRootFolderまでのフォルダを取得 
            ContentFolderViewModel? currentFolder = folder;
            while (currentFolder != null) {
                items.Add(currentFolder);
                currentFolder = currentFolder.ParentFolderViewModel;
            }
            // itemsの順番を逆にして、RootFolderからFolderまでのフォルダをExpandする
            for (int i = items.Count - 1; i >= 0; i--) {

                for (int j = 0; j < itemsControl.Items.Count; j++) {
                    TreeViewItem? childItem = (TreeViewItem?)itemsControl.ItemContainerGenerator.ContainerFromIndex(j);
                    if (childItem == null) {
                        continue;
                    }
                    if (childItem.Header is ContentFolderViewModel folderViewModel) {
                        if (folderViewModel.Folder.Id == items[i].Folder.Id) {
                            if (childItem.Items.Count == 0) {
                                childItem.IsExpanded = true;
                                break;
                            }
                            childItem.IsExpanded = true;
                            itemsControl.UpdateLayout();
                            itemsControl = childItem;
                            break;
                        }
                    }
                }
            }
        }

        // CreateFolderCommand
        public SimpleDelegateCommand<object> CreateFolderCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            // フォルダを作成する
            SelectedFolder.FolderCommands.CreateFolderCommand.Execute();
        });

    }

    public class MainPanelDataGridViewControlViewModel(CommonViewModelCommandExecutes commands) : ObservableObject {

        private CommonViewModelCommandExecutes Commands { get; set; } = commands;

        public TabControl? MyTabControl { get; set; }
        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ContentItemViewModel> _selectedItems = [];
        public ObservableCollection<ContentItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }
        public ContentItemViewModel? SelectedItem {
            get {
                // SelectedItemsの最後のアイテムを返す
                if (SelectedItems.Count > 0) {
                    return SelectedItems[^1];
                }
                return null;
            }
        }

        public void UpdateView() {
            // 前回選択していたTabIndexを取得
            int lastSelectedTabIndex = SelectedItem?.SelectedTabIndex ?? 0;

            // SelectedTabIndexを更新する処理
            if (SelectedItem != null) {
                /**
                 * Task.Run(() => {
                    SelectedChatItem.ContentItem.Load(() => { }, () => {
                        MainUITask.Run(() => {
                            OnPropertyChanged(nameof(SelectedChatItem));
                        });
                    });
                });
                OnPropertyChanged(nameof(SelectedChatItem));
                **/
                // SourceがFileの場合は、ファイルの内容を読み込む
                if (SelectedItem.ContentItem.SourceType == ContentSourceType.File) {
                    ContentItemCommands.ExtractTexts([SelectedItem.ContentItem], () => { }, () => {
                        MainUITask.Run(() => {
                            SelectedItem.UpdateView(MyTabControl);
                            OnPropertyChanged(nameof(SelectedItem));
                        });
                    });
                }
                // 選択中のアイテムのSelectedTabIndexを更新する
                SelectedItem.LastSelectedTabIndex = lastSelectedTabIndex;
                SelectedItem.UpdateView(MyTabControl);
                OnPropertyChanged(nameof(SelectedItem));

            }
        }

        // アイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ApplicationItemSelectionChangedCommand => new((routedEventArgs) => {

            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid dataGrid) {

                if (dataGrid.SelectedItem is ContentItemViewModel applicationItemViewModel) {
                    // SelectedItemsをMainWindowViewModelにセット
                    SelectedItems.Clear();
                    foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                        SelectedItems.Add(item);
                    }
                    UpdateView();
                }
            }

        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            foreach (ContentItemViewModel applicationItemViewModel in SelectedItems) {
                Commands.ChangePinCommand.Execute();
            }
        });


        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            Commands.OpenContentAsFileCommand.Execute(this.SelectedItem);
        });

        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            SelectedFolder?.FolderCommands.GenerateVectorCommand.Execute(this.SelectedItems);
        });


        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new(async (parameter) => {
            if (SelectedItem == null) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            await CommonViewModelCommandExecutes.OpenVectorSearchWindowCommandExecute(SelectedItem);
        });


        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            SelectedFolder?.FolderCommands.DeleteDisplayedItemCommand.Execute();
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            ObservableCollection<ContentItemViewModel> items = [.. SelectedItems];
            CommonViewModelCommandExecutes.DeleteItemsCommandExecute(items, () => {
                // プログレスインジケータを表示
                commands.UpdateIndeterminate(true);
            },
            () => {
                foreach (var itemViewModel in items) {
                    CommonViewModelCommandExecutes.ReloadFolderCommandExecute(itemViewModel.FolderViewModel, () => { }, () => { });
                }
                commands.UpdateIndeterminate(false);
            });
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            AppViewModelCommandExecutes.CutItemCommandExecute(this.SelectedItems);
        });

        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            AppViewModelCommandExecutes.CopyToClipboardCommandExecute(this.SelectedItems);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            AppViewModelCommandExecutes.OpenItemCommandExecute(this.SelectedItem);
        });



    }



}

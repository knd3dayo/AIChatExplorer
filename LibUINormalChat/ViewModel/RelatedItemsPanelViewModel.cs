using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;


namespace LibUINormalChat.ViewModel {
    public class RelatedItemsPanelViewModel : CommonViewModelBase {


        public RelatedItemsPanelViewModel(QAChatStartupPropsBase qAChatStartupPropsBase) {

            ChatSettings chatSettings = qAChatStartupPropsBase.GetContentItem().ChatSettings;

            RelatedItemsDataGridViewModel = new(qAChatStartupPropsBase);

            RelatedItemTreeViewControlViewModel = new(CreateSelectFolderAction());

            InitDataDefinitions(chatSettings);
        }

        public override void OnLoadedAction() {
            base.OnLoadedAction();
            RelatedItemsDataGridViewModel.MyTabControl = ThisUserControl?.FindName("MyTabControl") as TabControl;
        }
        public RelatedItemDataGridViewModel RelatedItemsDataGridViewModel { get; set; }

        public RelatedItemsTreeViewControlViewModel RelatedItemTreeViewControlViewModel { get; set; }

        // ShowProperties
        public bool ShowProperties {
            get {
                return CommonViewModelProperties.Instance.IsShowProperties;
            }
            set {
                CommonViewModelProperties.Instance.IsShowProperties = value;
                OnPropertyChanged(nameof(ShowProperties));
                OnPropertyChanged(nameof(PropertiesVisibility));
            }
        }

        // DataDefinitions
        public ObservableCollection<ContentItemDataDefinition> DataDefinitions { get; set; } = [];

        // PropertiesVisibility
        public Visibility PropertiesVisibility { get => LibUIPythonAI.Utils.Tools.BoolToVisibility(ShowProperties); }

        private static async Task<ObservableCollection<ContentItemDataDefinition>> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = await PromptItem.GetPromptItems();
            promptItems = promptItems.Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ContentItemDataDefinition> items = [.. ContentItemDataDefinition.CreateDefaultDataDefinitions()];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ContentItemDataDefinition(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }

        private Action<ContentFolderViewModel> CreateSelectFolderAction() {
            return (folder) => {
                // フォルダ選択変更時の処理。
                // 選択したファルダ内のItemがCheckedItemsに含まれている場合はIsCheckedをTrueにする。
                MainUITask.Run(() => {
                    foreach (var _item in folder.Items) {
                        if (RelatedItemsDataGridViewModel.CheckedItems.Select(x => x.ContentItem.Id).Contains(_item.ContentItem.Id)) {
                            _item.IsChecked = true;
                        }
                    }
                    RelatedItemsDataGridViewModel.Items = folder.Items;
                });
            };
        }
        private void InitDataDefinitions(ChatSettings chatSettings) {

            Task.Run(async () => {
                // DataDefinitionsが空の場合は、デフォルトのDataDefinitionsを作成
                DataDefinitions = await CreateExportItems();
                // DataDefinitionsを初期化
                List<ContentItemDataDefinition> savedDataDefinitions = [.. chatSettings.RelatedItems.DataDefinitions];
                // DataDefinitionsに保存されているDataDefinitionsの状態を反映
                foreach (ContentItemDataDefinition dataDefinition in DataDefinitions) {
                    ContentItemDataDefinition? savedDataDefinition = savedDataDefinitions.FirstOrDefault(x => x.Name == dataDefinition.Name);
                    if (savedDataDefinition != null) {
                        // 保存されているDataDefinitionの状態を反映
                        dataDefinition.IsChecked = savedDataDefinition.IsChecked;
                    }
                }
                // DataDefinitionsの変更を通知
                OnPropertyChanged(nameof(DataDefinitions));
            });
        }
    }

    public class RelatedItemDataGridViewModel : CommonViewModelBase {

        public RelatedItemDataGridViewModel(QAChatStartupPropsBase qAChatStartupProps) {
            ChatSettings chatSettings = qAChatStartupProps.GetContentItem().ChatSettings;
            FolderViewModelManagerBase folderViewModelManager = qAChatStartupProps.GetViewModelManager();

            foreach (var item in chatSettings.RelatedItems.ContentItems) {
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(item.FolderId);
                if (folder == null) {
                    LogWrapper.Info($"Folder with ID {item.FolderId} not found.");
                    continue;
                }
                ContentItemViewModel contentItemViewModel = folderViewModelManager.CreateItemViewModel(item);
                contentItemViewModel.PropertyChanged += Item_PropertyChanged;
                contentItemViewModel.IsChecked = true;
            }
        }
        // CheckedItems
        public ObservableCollection<ContentItemViewModel> CheckedItems { get; set; } = [];

        private ObservableCollection<ContentItemViewModel> _items = [];

        // フォルダ内のアイテム一覧
        public ObservableCollection<ContentItemViewModel> Items {
            get {
                return _items;
            }
            set {
                if (_items != null) {
                    UnsubscribeFromItemsPinnedChanged();
                    _items.CollectionChanged -= Items_CollectionChanged;
                }
                _items = value;
                if (_items != null) {
                    SubscribeToItemsPinnedChanged();
                    _items.CollectionChanged += Items_CollectionChanged;
                }
                OnPropertyChanged(nameof(Items));
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
                    return SelectedItems[SelectedItems.Count - 1];
                }
                return null;
            }
        }

        public DataGrid? RelatedItemDataSelectionDataGrid {
            get {
                return ThisUserControl?.FindName("RelatedItemDataSelectionDataGrid") as DataGrid;
            }
        }
        public TabControl? MyTabControl { get; set; }

        public ObservableCollection<ContentItemViewModel> CheckedItemsInRelatedItemDataSelectionDataGrid {
            get {
                ObservableCollection<ContentItemViewModel> checkedItems = [];
                if (RelatedItemDataSelectionDataGrid == null) {
                    return checkedItems;
                }
                foreach (ContentItemViewModel item in RelatedItemDataSelectionDataGrid.Items) {
                    if (item.IsChecked) {
                        checkedItems.Add(item);
                    }
                }
                return checkedItems;
            }
        }

        // アイテムが選択された時の処理
        // DataGridで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ApplicationItemSelectionChangedCommand => new((routedEventArgs) => {
            // routedEventArgsはDataGridの前提。DataGrid以外の場合はキャスト時に例外が発生する。
            DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
            // 前回選択していたTabIndexを取得

            int lastSelectedIndex = SelectedItem?.SelectedTabIndex ?? 0;

            // SelectedItemsをMainWindowViewModelにセット
            SelectedItems.Clear();
            foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                SelectedItems.Add(item);
            }

            // SelectedTabIndexを更新する処理
            if (SelectedItem != null) {
                OnPropertyChanged(nameof(SelectedItem));
                SelectedItem.SelectedTabIndex = lastSelectedIndex;
            }
            UpdateView();
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Itemsから選択中のアイテムを削除
            foreach (ContentItemViewModel item in SelectedItems) {
                Items.Remove(item);
            }
        });

        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            Items.Clear();
        });

        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            if (SelectedItem == null) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            ContentFolderViewModel folderViewModel = SelectedItem.FolderViewModel;

            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(folderViewModel, SelectedItem,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    folderViewModel.FolderCommands.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            // editItemControlをWindowとして開く
            Window editItemWindow = new() {
                Content = editItemControl,
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            editItemControl.SetCloseUserControl(() => {
                editItemWindow.Close();
            });
            editItemWindow.ShowDialog();

        });

        public void UpdateView() {
            // 前回選択していたTabIndexを取得
            int lastSelectedTabIndex = SelectedItem?.SelectedTabIndex ?? 0;

            // SelectedTabIndexを更新する処理
            if (SelectedItem != null) {
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

        // Itemsコレクションの変更に対応
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (ContentItemViewModel item in e.OldItems) {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null) {
                foreach (ContentItemViewModel item in e.NewItems) {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void SubscribeToItemsPinnedChanged() {
            foreach (var item in Items) {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        private void UnsubscribeFromItemsPinnedChanged() {
            foreach (var item in Items) {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ContentItemViewModel.IsChecked)) {
                // IsCheckedが変更されたときの処理をここに記述
                // 例: 必要に応じてUI更新や他のロジックを呼び出す
                if (sender is ContentItemViewModel item) {
                    // CheckedItemsからIdがマッチする既存アイテムを削除
                    var delItems = CheckedItems.Where(x => x.ContentItem.Id == item.ContentItem.Id);
                    foreach (var delItem in delItems.ToList()) {
                        CheckedItems.Remove(delItem);
                    }
                    // IsCheckedがTrueの場合はCheckedItemsに追加
                    if (item.IsChecked) {
                        CheckedItems.Add(item);
                    }
                }

            }
        }

    }
    public class RelatedItemsTreeViewControlViewModel : CommonViewModelBase {

        public RelatedItemsTreeViewControlViewModel(Action<ContentFolderViewModel> selectFolderAction) {
            SelectFolderAction = selectFolderAction;
            UpdateIndeterminateAction = CommonViewModelProperties.UpdateIndeterminate;
            FolderViewModels = FolderViewModelManagerBase.FolderViewModels;
            OnPropertyChanged(nameof(FolderViewModels));
            SelectedFolder = FolderViewModels[0];
        }

        public ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; }

        public Action<bool> UpdateIndeterminateAction { get; set; }

        public Action<ContentFolderViewModel> SelectFolderAction { get; set; } = (folder) => { };

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
                    // Load
                    _selectedFolder.LoadFolderExecute(beforeAction: () => { }, afterAction: () => {
                        SelectFolderAction(_selectedFolder);
                    });
                }
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel applicationItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;
            SelectedFolder = applicationItemFolderViewModel;
        });
    }
}

using System.Collections.ObjectModel;
using System.Windows.Controls;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Item;


namespace LibUIPythonAI.ViewModel.Folder {
    public abstract class ContentFolderViewModel(ContentFolderWrapper folder, CommonViewModelCommandExecutes commands) : CommonViewModelBase {
        public ContentFolderWrapper Folder { get; set; } = folder;

        public ContentFolderViewModelCommands FolderCommands => new(this, commands);


        // フォルダ作成コマンドの実装

        public abstract void CreateItemCommandExecute();

        public abstract ContentItemViewModel CreateItemViewModel(ContentItemWrapper item);

        public abstract ObservableCollection<MenuItem> FolderMenuItems { get; }

        public abstract ObservableCollection<ContentItemViewModel> GetSelectedItems();

        // RootFolderのViewModelを取得する
        public abstract ContentFolderViewModel GetRootFolderViewModel();

        public abstract ContentFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder);

        // フォルダ作成コマンドの実装
        public virtual void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow.OpenFolderEditWindow(CreateChildFolderViewModel(Folder.CreateChild("")), afterUpdate);
        }

        public virtual void EditFolderCommandExecute(Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(this, afterUpdate);
        }

        // フォルダを読み込む（async/await対応）
        public virtual async Task LoadFolderExecuteAsync(Action beforeAction, Action afterAction) {
            beforeAction();
            await Task.Run(() => {
                LoadChildren(DefaultNextLevel);
            });
            await LoadItemsAsync();
            afterAction();
        }
        public virtual void LoadChildren(int nestLevel) {
            LoadChildren<ContentFolderViewModel, ContentFolderWrapper>(nestLevel);
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected void LoadChildren<ViewModel, Model>(int nestLevel) where ViewModel : ContentFolderViewModel where Model : ContentFolderWrapper {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ViewModel> _children = [];
            foreach (var child in Folder.GetChildren<Model>().OrderBy(x => x.FolderName)) {
                if (child == null) {
                    continue;
                }
                ViewModel childViewModel = (ViewModel)CreateChildFolderViewModel(child);
                // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                if (nestLevel > 0) {
                    childViewModel.LoadChildren<ViewModel, Model>(nestLevel - 1);
                }
                _children.Add(childViewModel);
            }
            MainUITask.Run(() => {
                Children = [.. _children];
                OnPropertyChanged(nameof(Children));
            });
        }

        // LoadItemsAsync
        public virtual async Task LoadItemsAsync() {
            await LoadItemsAsync<ContentItemWrapper>();
        }

        protected async Task LoadItemsAsync<Item>() where Item : ContentItemWrapper {
            List<Item> _items = await Folder.GetItems<Item>();
            _items = _items.OrderByDescending(x => x.UpdatedAt).ToList();

            MainUITask.Run(() => {
                Items.Clear();
                foreach (Item item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            });
        }


        // GetItems
        public ObservableCollection<ContentItemViewModel> Items { get; } = [];

        // 子フォルダ
        public ObservableCollection<ContentFolderViewModel> Children { get; set; } = [];


        public bool IsNameEditable {
            get {
                return Folder.IsRootFolder == false;
            }
        }

        public ContentFolderViewModel? ParentFolderViewModel { get; set; }

        // DisplayText
        public string Description {
            get {
                return Folder.Description;
            }
            set {
                Folder.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                return Folder.IsRootFolder == false;
            }
        }

        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public virtual int DefaultNextLevel { get; } = 1;



        public string FolderName {
            get {
                return Folder.FolderName;
            }
            set {
                Folder.FolderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }
        public string FolderPath {
            get {
                return Folder.ContentFolderPath;
            }
        }

        public virtual void UpdateStatusText() {
            Task.Run(async () => {
                string message = await Folder.GetStatusText();
                StatusText.Instance.ReadyText = message;
                StatusText.Instance.Text = message;
            });
        }

    }
}

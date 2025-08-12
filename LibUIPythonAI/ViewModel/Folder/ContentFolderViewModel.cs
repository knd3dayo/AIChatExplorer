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
        public abstract ContentFolderViewModel? GetRootFolderViewModel();

        public abstract ContentFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder);

        // フォルダ作成コマンドの実装
        public virtual void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {

            // folderViewModel 引数は未使用です。必要なら活用してください。
            FolderEditWindow.OpenFolderEditWindow(CreateChildFolderViewModel(Folder.CreateChild("")), afterUpdate);
        }

        public virtual void EditFolderCommandExecute(Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(this, afterUpdate);
        }

        // フォルダを読み込む（async/await対応）
        public virtual async Task LoadFolderExecuteAsync() {
            try {
                await LoadChildren(DefaultNextLevel);
                await LoadItemsAsync();
            } catch (Exception ex) {
                // エラー通知やログ出力を追加
                System.Diagnostics.Debug.WriteLine($"LoadFolderExecuteAsync Error: {ex.Message}");
            }
        }
        public virtual async Task LoadChildren(int nestLevel) {
            try {
                await LoadChildren<ContentFolderViewModel, ContentFolderWrapper>(nestLevel);
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"LoadChildren Error: {ex.Message}");
            }
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected async Task LoadChildren<ViewModel, Model>(int nestLevel) where ViewModel : ContentFolderViewModel where Model : ContentFolderWrapper {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            var _children = new List<ViewModel>(); // 互換性のため明示的に初期化
            try {
                var folders = await Folder.GetChildrenAsync<Model>(true);
                foreach (var child in folders.OrderBy(x => x.FolderName)) {
                    if (child == null) {
                        continue;
                    }
                    ViewModel childViewModel = (ViewModel)CreateChildFolderViewModel(child);
                    // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                    if (nestLevel > 0) {
                        await childViewModel.LoadChildren<ViewModel, Model>(nestLevel - 1);
                    }
                    _children.Add(childViewModel);
                }
                MainUITask.Run(() => {
                    Children = new ObservableCollection<ContentFolderViewModel>(_children.Cast<ContentFolderViewModel>());
                    OnPropertyChanged(nameof(Children));
                });
            } catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine($"LoadChildren<T> Error: {ex.Message}");
            }
        }

        // LoadItemsAsync
        public virtual async Task LoadItemsAsync() {
            await LoadItemsAsync<ContentItemWrapper>();
        }

        protected async Task LoadItemsAsync<Item>() where Item : ContentItemWrapper {
            List<Item> _items = await Folder.GetItemsAsync<Item>();
            _items = _items.OrderByDescending(x => x.UpdatedAt).ToList();

            MainUITask.Run(() => {
                Items.Clear();
                foreach (Item item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
                OnPropertyChanged(nameof(Items));
            });
        }

        // GetItemsAsync
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
 
        public virtual void UpdateStatusText() {
            Task.Run(async () => {
                string message = await Folder.GetStatusText();
                StatusText.Instance.ReadyText = message;
                StatusText.Instance.Text = message;
            });
        }

    }
}

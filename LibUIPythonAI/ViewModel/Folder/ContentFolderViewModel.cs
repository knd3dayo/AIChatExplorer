using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using LibPythonAI.Utils.Common;


namespace LibUIPythonAI.ViewModel.Folder {
    public abstract class ContentFolderViewModel(ContentFolderWrapper folder, ContentItemViewModelCommands commands) : ChatViewModelBase {


        public ContentFolderWrapper Folder { get; set; } = folder;

        public ContentItemViewModelCommands Commands { get; set; } = commands;


        // フォルダ作成コマンドの実装
        public abstract void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate);

        public abstract void CreateItemCommandExecute();
        public abstract void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate);
        public abstract ContentItemViewModel CreateItemViewModel(ContentItemWrapper item);

        public abstract ObservableCollection<MenuItem> FolderMenuItems { get; }

        // RootFolderのViewModelを取得する
        public abstract ContentFolderViewModel GetRootFolderViewModel();

        public abstract ContentFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder);

        // フォルダを読み込む
        public abstract void LoadFolderExecute(Action beforeAction, Action afterAction);

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected virtual void LoadChildren(int nestLevel = 5) {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ContentFolderViewModel> _children = [];
            foreach (var child in Folder.GetChildren()) {
                if (child == null) {
                    continue;
                }
                ContentFolderViewModel childViewModel = CreateChildFolderViewModel(child);
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
            List<ContentItemWrapper> _items = Folder.GetItems();
            MainUITask.Run(() => {
                Items.Clear();
                foreach (ContentItemWrapper item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            });
        }

        public abstract SimpleDelegateCommand<object> LoadFolderCommand { get; }


        // フォルダー保存コマンド
        public virtual SimpleDelegateCommand<ContentFolderViewModel> SaveFolderCommand => new((folderViewModel) => {
            Folder.Save();
        });
        // 新規フォルダ作成コマンド
        public SimpleDelegateCommand<object> CreateFolderCommand => new((parameter) => {

            CreateFolderCommandExecute(this, () => {
                // 親フォルダを保存
                this.Folder.Save();
                this.LoadFolderCommand.Execute();

            });
        });

        // フォルダ編集コマンド
        public SimpleDelegateCommand<object> EditFolderCommand => new((parameter) => {

            EditFolderCommandExecute(this, () => {
                //　フォルダを保存
                this.Folder.Save();
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.FolderEdited);
            });
        });
        public void DeleteDisplayedItemCommandExecute(Action beforeAction, Action afterAction) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                beforeAction();
                ContentItemViewModel.DeleteItems([.. Items]).ContinueWith((task) => {
                    // 全ての削除処理が終了した後、後続処理を実行
                    // フォルダ内のアイテムを再読み込む
                    afterAction();
                });
            }
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

        // アイテム保存コマンド
        public SimpleDelegateCommand<ContentItemViewModel> AddItemCommand => new((item) => {
            Folder.AddItem(item.ContentItem);
        });

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
        public virtual int DefaultNextLevel { get; } = 5;



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

        public SimpleDelegateCommand<object> DeleteFolderCommand => new((parameter) => {

            // フォルダ削除するかどうか確認
            if (MessageBox.Show(StringResources.ConfirmDeleteFolder, StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            // 親フォルダを取得
            ContentFolderViewModel? parentFolderViewModel = this.ParentFolderViewModel;

            this.Folder.Delete();

            // 親フォルダが存在する場合は、親フォルダを再読み込み
            if (parentFolderViewModel != null) {
                parentFolderViewModel.LoadFolderCommand.Execute();
            }

            LogWrapper.Info(StringResources.FolderDeleted);
        });
        // ベクトルのリフレッシュ
        public SimpleDelegateCommand<object> RefreshVectorDBCollectionCommand => new((parameter) => {
            Task.Run(() => {
                try {
                    // MainWindowViewModelのIsIndeterminateをTrueに設定
                    UpdateIndeterminate(true);
                    Folder.GetMainVectorSearchProperty().RefreshVectorDBCollection(Folder.Description, () => {
                        // フォルダ内のアイテムを取得して、ベクトルを作成
                        foreach (var item in Folder.GetItems()) {
                            ContentItemCommands.UpdateEmbeddings([item]);
                            // Save
                            item.Save();
                        }
                    });
                } finally {
                    UpdateIndeterminate(false);
                }
            });

        });
        // ExportImportFolderCommand
        public SimpleDelegateCommand<object> ExportImportFolderCommand => new((parameter) => {
            // ExportImportFolderWindowを開く
            ExportImportWindow.OpenExportImportFolderWindow(this, () => {
                // ファイルを再読み込み
                this.LoadFolderCommand.Execute();
            });
        });
        protected virtual void UpdateStatusText() {
            string message = Folder.GetStatusText();
            StatusText.Instance.ReadyText = message;
            StatusText.Instance.Text = message;
        }


    }
}

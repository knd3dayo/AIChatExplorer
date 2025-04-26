using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Item;
using WpfAppCommon.Model;


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
        public virtual void LoadFolderExecute(Action beforeAction, Action afterAction) {
            beforeAction();
            Task.Run(() => {
                LoadChildren(DefaultNextLevel);
                LoadItems();
            }).ContinueWith((task) => {
                afterAction();
            });
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
            foreach (var child in Folder.GetChildren<Model>()) {
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
                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            });
        }

        // LoadItems
        public virtual void LoadItems() {
            LoadItems<ContentItemWrapper>();
        }


        public void LoadItems<Item>() where Item : ContentItemWrapper {
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<Item> _items = Folder.GetItems<Item>().OrderByDescending(x => x.UpdatedAt).ToList();
            MainUITask.Run(() => {
                Items.Clear();
                foreach (Item item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            });
        }

        public virtual SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadFolderExecute(
                () => {
                    Commands.UpdateIndeterminate(true);
                },
                () => {
                    MainUITask.Run(() => {
                        Commands.UpdateIndeterminate(false);
                        UpdateStatusText();
                    });
                });
        });

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

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(() => {
                Commands.UpdateIndeterminate(true);
            }, () => {
                // 全ての削除処理が終了した後、後続処理を実行
                // フォルダ内のアイテムを再読み込む
                MainUITask.Run(() => {
                    LoadFolderCommand.Execute();
                });
                LogWrapper.Info(CommonStringResources.Instance.Deleted);
                Commands.UpdateIndeterminate(false);
            });
        });

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
                // MainWindowViewModelのIsIndeterminateをTrueに設定
                UpdateIndeterminate(true);
                Folder.GetMainVectorSearchProperty().DeleteVectorDBCollection();
                ContentItemCommands.UpdateEmbeddings(Folder.GetItems<ContentItemWrapper>(), () => { }, () => {

                    ContentItemWrapper.SaveItems(Folder.GetItems<ContentItemWrapper>());
                    UpdateIndeterminate(false);
                });

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

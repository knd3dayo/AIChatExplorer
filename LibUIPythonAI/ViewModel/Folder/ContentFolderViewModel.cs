using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.Content;
using QAChat.Model;
using QAChat.View.Folder;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;


namespace QAChat.ViewModel.Folder {
    public abstract class ContentFolderViewModel(ContentFolder folder) : QAChatViewModelBase {


        // フォルダ作成コマンドの実装
        public abstract void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate);

        public abstract void CreateItemCommandExecute();
        public abstract void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate);
        public abstract ContentItemViewModel CreateItemViewModel(ContentItem item);

        public abstract ObservableCollection<MenuItem> FolderMenuItems { get; }

        // RootFolderのViewModelを取得する
        public abstract ContentFolderViewModel GetRootFolderViewModel();

        // フォルダを読み込む
        public abstract void LoadFolder(Action afterUpdate);


        // フォルダー保存コマンド
        public virtual SimpleDelegateCommand<ContentFolderViewModel> SaveFolderCommand => new((folderViewModel) => {
            Folder.Save();
        });
        // 新規フォルダ作成コマンド
        public SimpleDelegateCommand<ContentFolderViewModel> CreateFolderCommand => new((folderViewModel) => {

            CreateFolderCommandExecute(folderViewModel, () => {
                // 親フォルダを保存
                folderViewModel.Folder.Save();
                folderViewModel.LoadFolderCommand.Execute();

            });
        });

        // フォルダ編集コマンド
        public SimpleDelegateCommand<ContentFolderViewModel> EditFolderCommand => new((folderViewModel) => {

            EditFolderCommandExecute(folderViewModel, () => {
                //　フォルダを保存
                folderViewModel.Folder.Save();
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.FolderEdited);
            });
        });

        // GetItems
        public ObservableCollection<ContentItemViewModel> Items { get; } = [];

        // 子フォルダ
        public ObservableCollection<ContentFolderViewModel> Children { get; set; } = [];

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


        public ContentFolder Folder { get; set; } = folder;


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
                return Folder.FolderPath;
            }
        }

        public SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadFolder(() => { });
        });


        public virtual void UpdateIndeterminate(bool isIndeterminate) { }

        public SimpleDelegateCommand<ContentFolderViewModel> DeleteFolderCommand => new((folderViewModel) => {

            // フォルダ削除するかどうか確認
            if (MessageBox.Show(StringResources.ConfirmDeleteFolder, StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            // 親フォルダを取得
            ContentFolderViewModel? parentFolderViewModel = folderViewModel.ParentFolderViewModel;

            folderViewModel.Folder.Delete();

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
                    Folder.RefreshVectorDBCollection<ContentItem>();
                } finally {
                    UpdateIndeterminate(false);
                }
            });

        });
        // ExportImportFolderCommand
        public static SimpleDelegateCommand<ContentFolderViewModel> ExportImportFolderCommand => new((folderViewModel) => {
            // ExportImportFolderWindowを開く
            ExportImportWindow.OpenExportImportFolderWindow(folderViewModel, () => {
                // ファイルを再読み込み
                folderViewModel.LoadFolderCommand.Execute();
            });
        });



    }
}

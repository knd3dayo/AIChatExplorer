using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Content;
using QAChat.Model;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace QAChat.ViewModel.Folder {
    public abstract class ContentFolderViewModel(ContentFolder folder) : QAChatViewModelBase {

        // フォルダ作成コマンドの実装
        public abstract void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate);

        public abstract void CreateItemCommandExecute();


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

        public virtual SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => { });

        // フォルダー保存コマンド
        public virtual SimpleDelegateCommand<ContentFolderViewModel> SaveFolderCommand => new((folderViewModel) => { });

        public virtual void UpdateIndeterminate(bool isIndeterminate) {}

    }
}

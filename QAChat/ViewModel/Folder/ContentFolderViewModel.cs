using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Content;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace QAChat.ViewModel.Folder {
    public class ContentFolderViewModel(ContentFolder folder) : ObservableObject {
        protected CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

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

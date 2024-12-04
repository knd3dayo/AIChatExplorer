using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Content;
using WpfAppCommon.Utils;


namespace QAChat.ViewModel.Folder {
    public partial class ContentFolderViewModel(ContentFolder folder) : ObservableObject {

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

    }
}

using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.Resource;
using QAChat.View.Folder;
using QAChat.View.VectorDB;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDB {
    public class SelectVectorDBItemWindowViewModel : ObservableObject {
        public SelectVectorDBItemWindowViewModel(ContentFolderViewModel rootFolderViewModel, bool closeAfterSelect, Action<List<VectorDBItem>> action) {
            Action = action;
            FolderViewModel = rootFolderViewModel;
            CloseAfterSelect = closeAfterSelect;
        }
        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public Action<List<VectorDBItem>> Action { get; set; }

        public bool CloseAfterSelect { get; set; }

        public ContentFolderViewModel FolderViewModel { get; set; }


        private bool isFolder = true;
        public bool IsFolder {
            get {
                return isFolder;
            }
            set {
                isFolder = value;
                OnPropertyChanged(nameof(IsFolder));
            }
        }
        private bool isExternal = true;
        public bool IsExternal {
            get {
                return isExternal;
            }
            set {
                isExternal = value;
                OnPropertyChanged(nameof(IsExternal));
            }
        }


        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {
            if (IsFolder) {
                FolderSelectWindow.OpenFolderSelectWindow(FolderViewModel, (folderViewModel) => {
                    List<VectorDBItem> vectorDBItemBases = [];
                    vectorDBItemBases.Add(folderViewModel.Folder.GetVectorDBItem());
                    Action(vectorDBItemBases);
                });
            }
            if (IsExternal) {
                List<VectorDBItem> vectorDBItemBases = [];
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (vectorDBItemBase) => {
                    vectorDBItemBases.Add(vectorDBItemBase);
                    Action(vectorDBItemBases);
                });
            }
            if (CloseAfterSelect) {
                window.Close();
            }
        });

        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            window.Close();
        });


    }
}

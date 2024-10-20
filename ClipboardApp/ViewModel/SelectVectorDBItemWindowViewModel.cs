using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.VectorDB;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;
using QAChat.Resource;

namespace ClipboardApp.ViewModel {
    public class SelectVectorDBItemWindowViewModel : ObservableObject {

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public Action<List<VectorDBItem>> Action { get; set; }

        public bool CloseAfterSelect { get; set; }

        public ClipboardFolderViewModel FolderViewModel { get; set; }
        public SelectVectorDBItemWindowViewModel(ClipboardFolderViewModel rootFolderViewModel, bool closeAfterSelect, Action<List<VectorDBItem>> action) {
            Action = action;
            FolderViewModel = rootFolderViewModel;
            CloseAfterSelect = closeAfterSelect;
        }

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
                    vectorDBItemBases.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
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

using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Abstract;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public class SelectVectorDBItemWindowViewModel : ObservableObject {

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public Action<List<VectorDBItemBase>> Action { get; set; }

        public ClipboardFolderViewModel FolderViewModel { get; set; }
        public SelectVectorDBItemWindowViewModel(ClipboardFolderViewModel rootFolderViewModel, Action<List<VectorDBItemBase>> action) {
            Action = action;
            FolderViewModel = rootFolderViewModel;
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
                    List<VectorDBItemBase> vectorDBItemBases = [];
                    vectorDBItemBases.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
                    Action(vectorDBItemBases);
                });
                return;
            }
            if (IsExternal) {
                List<VectorDBItemBase> vectorDBItemBases = [];
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (vectorDBItemBase) => {
                    vectorDBItemBases.Add(vectorDBItemBase);
                    Action(vectorDBItemBases);
                });
                return;
            }
            window.Close();
        });

        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            window.Close();
        });


    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace LibUIPythonAI.ViewModel.Folder {
    public class RootFolderViewModelContainer : ObservableObject {

        // ClipboardFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];


    }
}

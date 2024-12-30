using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.ViewModel.Folder;
using System.Collections.ObjectModel;

namespace PythonAILibUI.ViewModel.Folder {
    public class RootFolderViewModelContainer : ObservableObject {

        // ClipboardFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];


    }
}

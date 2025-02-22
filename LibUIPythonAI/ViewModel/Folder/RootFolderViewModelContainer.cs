using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILibUI.ViewModel.Item;
using System.Collections.ObjectModel;

namespace LibUIPythonAI.ViewModel.Folder {
    public class RootFolderViewModelContainer(ContentItemViewModelCommands commands) : ObservableObject {

        // ClipboardFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];

        // Commands
        public ContentItemViewModelCommands Commands { get; set; } = commands;


    }
}

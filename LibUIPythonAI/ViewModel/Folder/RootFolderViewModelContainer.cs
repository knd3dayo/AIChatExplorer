using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.ViewModel.Common;
using System.Collections.ObjectModel;

namespace LibUIPythonAI.ViewModel.Folder {
    public class RootFolderViewModelContainer(CommonViewModelCommandExecutes commands) : ObservableObject {

        // ApplicationFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];

        // Commands
        public CommonViewModelCommandExecutes Commands { get; set; } = commands;


    }
}

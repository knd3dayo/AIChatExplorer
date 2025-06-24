using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.ViewModel.Common;
using System.Collections.ObjectModel;

namespace LibUIPythonAI.ViewModel.Folder {
    public abstract class FolderViewModelManagerBase(CommonViewModelCommandExecutes commands) : ObservableObject {

        // ApplicationFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];

        // Commands
        public CommonViewModelCommandExecutes Commands { get; set; } = commands;

        public abstract ContentFolderViewModel GetApplicationRootFolderViewModel();

        public abstract ContentFolderViewModel GetSearchRootFolderViewModel();

    }
}

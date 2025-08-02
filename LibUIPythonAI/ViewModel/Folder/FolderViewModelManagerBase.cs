using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Item;
using System.Collections.ObjectModel;

namespace LibUIPythonAI.ViewModel.Folder {
    public abstract class FolderViewModelManagerBase(CommonViewModelCommandExecutes commands) : ObservableObject {

        // ApplicationFolder
        public static ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = [];

        // Commands
        public CommonViewModelCommandExecutes Commands { get; set; } = commands;

        public abstract ContentFolderViewModel? GetApplicationRootFolderViewModel();

        public abstract ContentFolderViewModel? GetSearchRootFolderViewModel();

        public abstract Task<ContentFolderViewModel?> CreateFolderViewModel(string folderId, string type);

        public abstract Task<ContentItemViewModel?> CreateItemViewModel(ContentItemWrapper item);

    }
}

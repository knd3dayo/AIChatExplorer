using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using PythonAILib.Model.Content;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace MergeChat {
    public class MergeChatFolderViewModel : ContentFolderViewModel{

        public MergeChatFolderViewModel(ContentFolder folder) : base(folder) {
        }
        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            throw new NotImplementedException();
        }
        public override void CreateItemCommandExecute() {
            throw new NotImplementedException();
        }
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            throw new NotImplementedException();
        }
        public override ContentItemViewModel CreateItemViewModel(ContentItem item) {
            throw new NotImplementedException();
        }
        public override ContentFolderViewModel GetRootFolderViewModel() {
            throw new NotImplementedException();
        }

        public override ObservableCollection<MenuItem> FolderMenuItems { get; } = [];


    }
}

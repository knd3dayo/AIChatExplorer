using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView
{

    public class FolderSelectWindowViewModel : ObservableObject
    {
        private static FolderSelectWindowViewModel? Instance;
        // フォルダツリーのルート
        public ObservableCollection<ClipboardItemFolderViewModel> RootFolders { get; set; } = new ObservableCollection<ClipboardItemFolderViewModel>();

        // フォルダ選択時のAction
        public Action<ClipboardItemFolderViewModel>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        private ClipboardItemFolderViewModel? _selectedFolder { get; set; }
        public ClipboardItemFolderViewModel? SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged("SelectedFolder");
            }
        }

        private string _selectedFolderAbsoluteCollectionName = "";
        public string SelectedFolderAbsoluteCollectionName
        {
            get
            {
                return _selectedFolderAbsoluteCollectionName;
            }
            set
            {
                _selectedFolderAbsoluteCollectionName = value;
                OnPropertyChanged("SelectedFolderAbsoluteCollectionName");
            }
        }

        public void Initialize(ClipboardItemFolderViewModel rootFolderViewModel, Action<ClipboardItemFolderViewModel> _FolderSelectedAction)
        {

            FolderSelectedAction = _FolderSelectedAction;
            if (rootFolderViewModel == null)
            {
                return;
            }
            RootFolders.Add(rootFolderViewModel);
            Instance = this;
        }
        public static SimpleDelegateCommand SelectFolderCommand => new ((parameter) => {
            if (Instance == null) {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if (Instance.SelectedFolder == null) {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.FolderSelectedAction?.Invoke(Instance.SelectedFolder);
            // Windowを閉じる
            if (parameter is FolderSelectWindow folderSelectWindow) {
                folderSelectWindow.Close();
            }

        });

        public SimpleDelegateCommand CancelCommand => new((parameter) => {
            // Windowを閉じる
            if (parameter is FolderSelectWindow folderSelectWindow) {
                folderSelectWindow.Close();
            }

        });

        public static void FolderSelectWindowSelectFolderCommandExecute(object parameter)
        {
            if (Instance == null)
            {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if (parameter is not ClipboardItemFolderViewModel folder)
            {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.SelectedFolder = folder;
            Instance.SelectedFolderAbsoluteCollectionName = folder.ClipboardItemFolder.AbsoluteCollectionName;

        }
    }
}

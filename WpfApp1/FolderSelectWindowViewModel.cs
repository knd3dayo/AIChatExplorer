using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{

    public class FolderSelectWindowViewModel: ObservableObject
    {
        private static FolderSelectWindowViewModel? Instance;
        // フォルダツリーのルート
        public ObservableCollection<ClipboardItemFolder> RootFolders { get; set; } = new ObservableCollection<ClipboardItemFolder>();

        // フォルダ選択時のAction
        public Action<ClipboardItemFolder>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        private  ClipboardItemFolder? _selectedFolder { get; set; }
        public  ClipboardItemFolder? SelectedFolder
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

        private  string _selectedFolderAbsoluteCollectionName = "";
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

        public void Initialize(Action<ClipboardItemFolder> _FolderSelectedAction)
        {
            FolderSelectedAction = _FolderSelectedAction;
            ClipboardItemFolder? rootFolder = ClipboardDatabaseController.GetFolderTree();
            if (rootFolder == null)
            {
                return;
            }
            RootFolders.Add(rootFolder);
            Instance = this;
        }
        public static SimpleDelegateCommand SelectFolderCommand => new SimpleDelegateCommand(SelectFolderCommandExecute);
        public static void SelectFolderCommandExecute(object parameter)
        {
            if ( Instance == null)
            {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if ( Instance.SelectedFolder == null)
            {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.FolderSelectedAction?.Invoke(Instance.SelectedFolder);
            FolderSelectWindow.Current?.Close();
        }

    public SimpleDelegateCommand CancelCommand => new SimpleDelegateCommand(CancelCommandExecute);
        private void CancelCommandExecute(object parameter)
        {
            // Windowを閉じる
            FolderSelectWindow.Current?.Close();
        }

        public static void FolderSelectWindowSelectFolderCommandExecute(object parameter)
        {
            if (Instance == null)
            {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if (parameter is not ClipboardItemFolder folder)
            {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.SelectedFolder = folder;
            Instance.SelectedFolderAbsoluteCollectionName = folder.AbsoluteCollectionName;

        }
    }
}

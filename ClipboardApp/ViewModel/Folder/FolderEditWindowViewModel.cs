using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.View.SelectVectorDBView;
using PythonAILib.Model.VectorDB;
using QAChat.Control;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {
    public class FolderEditWindowViewModel : ClipboardAppViewModelBase {

        private ClipboardFolderViewModel? _FolderViewModel = null;
        public ClipboardFolderViewModel? FolderViewModel {
            get {
                return _FolderViewModel;
            }
            set {
                _FolderViewModel = value;
                OnPropertyChanged(nameof(FolderViewModel));
            }
        }
        // 検索条件を常時適用するかどうか
        private bool _alwaysApplySearchCondition = false;
        public bool AlwaysApplySearchCondition {
            get {
                return _alwaysApplySearchCondition;
            }
            set {
                _alwaysApplySearchCondition = value;
                OnPropertyChanged(nameof(AlwaysApplySearchCondition));
            }
        }

        // フォルダ作成後に実行するコマンド
        private Action? _afterUpdate;

        // 起動時の処理
        public FolderEditWindowViewModel(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            _afterUpdate = afterUpdate;
            FolderViewModel = folderViewModel;

        }

        private bool isDrawerOpen = false;
        public bool IsDrawerOpen {
            get {
                return isDrawerOpen;
            }
            set {
                isDrawerOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }

        private ObservableCollection<VectorDBItem> _vectorDBItemBases = [];
        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return _vectorDBItemBases;
            }
            set {
                _vectorDBItemBases = value;
                OnPropertyChanged(nameof(VectorDBItems));
            }
        }

        private VectorDBItem? _SelectedVectorDBItem = null;
        public VectorDBItem? SelectedVectorDBItem {
            get {
                return _SelectedVectorDBItem;
            }
            set {
                _SelectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }

        public SimpleDelegateCommand<Window> CreateCommand => new((window) => {
            if (FolderViewModel == null) {
                LogWrapper.Error(StringResources.FolderNotSpecified);
                return;
            }

            // フォルダ名が空の場合はエラー
            if (FolderViewModel.FolderName == "") {
                LogWrapper.Error(StringResources.EnterFolderName);
                return;
            }

            FolderViewModel.SaveFolderCommand.Execute(null);
            // フォルダ作成後に実行するコマンドが設定されている場合
            _afterUpdate?.Invoke();
            // ウィンドウを閉じる
            window.Close();
        });

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                VectorDBItems.Remove(SelectedVectorDBItem);
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });
        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            if (MainWindowViewModel.ActiveInstance == null) {
                LogWrapper.Error("MainWindowViewModelがNullです");
                return;
            }
            SelectVectorDBWindow.OpenSelectVectorDBWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, false, (selectedItems) => {
                foreach (var item in selectedItems) {
                    VectorDBItems.Add(item);
                }
            }); OnPropertyChanged(nameof(VectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                VectorDBItems.Add(selectedItem);
            });
        });


    }

}

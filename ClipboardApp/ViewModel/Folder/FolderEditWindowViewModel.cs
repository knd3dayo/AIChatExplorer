using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.View.SelectVectorDBView;
using PythonAILib.Model.VectorDB;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {
    public class FolderEditWindowViewModel : ClipboardAppViewModelBase {

        // 起動時の処理
        public FolderEditWindowViewModel(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            FolderViewModel = folderViewModel;
            OnPropertyChanged(nameof(FolderViewModel));
        }
        public ClipboardFolderViewModel FolderViewModel { get; set; }
        // フォルダ作成後に実行するコマンド
        private Action AfterUpdate { get; set; }

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
        // IncludeInReferenceVectorDBItems
        public bool IncludeInReferenceVectorDBItems {
            get {
                return FolderViewModel.ClipboardItemFolder.IncludeInReferenceVectorDBItems;
            }
            set {
                FolderViewModel.ClipboardItemFolder.IncludeInReferenceVectorDBItems = value;
                OnPropertyChanged(nameof(IncludeInReferenceVectorDBItems));
            }
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

        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                ObservableCollection<VectorDBItem> _vectorDBItemBases = new();
                foreach (var item in FolderViewModel.ClipboardItemFolder.ReferenceVectorDBItems) {
                    if (!_vectorDBItemBases.Contains(item)) {
                        _vectorDBItemBases.Add(item);
                    }
                }
                return _vectorDBItemBases;
            }
            set {
                FolderViewModel.ClipboardItemFolder.ReferenceVectorDBItems.Clear();
                foreach (var item in value) {
                    FolderViewModel.ClipboardItemFolder.ReferenceVectorDBItems.Add(item);
                }
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
            // フォルダ名が空の場合はエラー
            if (FolderViewModel.FolderName == "") {
                LogWrapper.Error(StringResources.EnterFolderName);
                return;
            }

            FolderViewModel.SaveFolderCommand.Execute(null);
            // フォルダ作成後に実行するコマンドが設定されている場合
            AfterUpdate?.Invoke();
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
            SelectVectorDBWindow.OpenSelectVectorDBWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, true, (selectedItems) => {
                foreach (var item in selectedItems) {
                    // VectorDBItems.Add(item);
                    FolderViewModel.ClipboardItemFolder.ReferenceVectorDBItems.Add(item);
                }
            }); OnPropertyChanged(nameof(VectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                VectorDBItems.Add(selectedItem);
                OnPropertyChanged(nameof(VectorDBItems));

            });
        });


    }

}

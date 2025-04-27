using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIPythonAI.ViewModel.Folder {
    public class FolderEditWindowViewModel : ChatViewModelBase {

        // 起動時の処理
        public FolderEditWindowViewModel(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            AfterUpdate = afterUpdate;
            FolderViewModel = folderViewModel;

            OnPropertyChanged(nameof(FolderViewModel));
        }

        public ContentFolderViewModel FolderViewModel { get; set; }
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

        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return [.. VectorDBItem.GetVectorDBItems(true)];
            }
        }

        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get {
                return _selectedTabIndex;
            }
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
                // ボタンの表示を更新
                OnPropertyChanged(nameof(VectorDBItemButtonVisibility));
            }
        }

        public Visibility VectorDBItemButtonVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(SelectedTabIndex == 1);

        private ObservableCollection<VectorDBProperty> _vectorSearchProperties = [];
        public ObservableCollection<VectorDBProperty> VectorSearchProperties {
            get {
                return _vectorSearchProperties;
            }
            set {
                _vectorSearchProperties = value;
                OnPropertyChanged(nameof(VectorSearchProperties));
            }
        }

        private VectorDBProperty? _selectedVectorSearchProperty = null;
        public VectorDBProperty? SelectedVectorSearchProperty {
            get {
                return _selectedVectorSearchProperty;
            }
            set {
                _selectedVectorSearchProperty = value;
                OnPropertyChanged(nameof(SelectedVectorSearchProperty));
            }
        }

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorSearchProperty != null) {
                // VectorDBItemsから削除
                VectorSearchProperties.Remove(SelectedVectorSearchProperty);
            }
            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });



        public SimpleDelegateCommand<Window> CreateCommand => new((window) => {
            // フォルダ名が空の場合はエラー
            if (FolderViewModel.FolderName == "") {
                LogWrapper.Error(StringResources.EnterFolderName);
                return;
            }
            //　説明がない場合はエラー
            if (FolderViewModel.Description == "") {
                LogWrapper.Error(StringResources.EnterDescription);
                return;
            }
            // VectorSearchPropertiesを設定
            FolderViewModel.Folder.ReferenceVectorSearchProperties = [.. VectorSearchProperties];

            FolderViewModel.SaveFolderCommand.Execute(null);
            // フォルダ作成後に実行するコマンドが設定されている場合
            AfterUpdate?.Invoke();
            // ウィンドウを閉じる
            window.Close();
        });

    }

}

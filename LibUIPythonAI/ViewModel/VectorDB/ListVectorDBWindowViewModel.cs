using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Data;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.VectorDB {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListVectorDBWindowViewModel : CommonViewModelBase {

        public ListVectorDBWindowViewModel(ActionModeEnum mode, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action<VectorDBProperty> callBackup) {

            this.mode = mode;
            this.callBackup = callBackup;

            LoadVectorItemsCommand.Execute();
            FolderSelectWindowViewModel = new(rootFolderViewModels, (selectedFolder, finished) => {
                FolderViewModel = selectedFolder;
            });

        }

        public enum ActionModeEnum {
            Edit,
            Select,
        }
        // VectorDBItemのリスト
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        private ActionModeEnum mode;
        Action<VectorDBProperty>? callBackup;


        // 選択中のVectorDBItem
        private VectorDBItemViewModel? selectedVectorDBItem;
        public VectorDBItemViewModel? SelectedVectorDBItem {
            get {
                return selectedVectorDBItem;
            }
            set {
                selectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }
        // システム用のVectorDBItemを表示するか否か
        private bool isShowSystemCommonVectorDB;
        public bool IsShowSystemCommonVectorDB {
            get {
                return isShowSystemCommonVectorDB;
            }
            set {
                isShowSystemCommonVectorDB = value;
                OnPropertyChanged(nameof(IsShowSystemCommonVectorDB));
                // リストを更新
                LoadVectorItemsCommand.Execute();

            }
        }

        // FolderSelectWindowViewModel
        public FolderSelectWindowViewModel FolderSelectWindowViewModel { get; set; }
        // ContentFolderViewModel 
        public ContentFolderViewModel? FolderViewModel { get; set; }

        // 選択ボタンの表示可否
        public Visibility SelectModeVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(mode == ActionModeEnum.Select);

        // SelectedTabIndex
        private int selectedTabIndex;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }


        // VectorDBItemのロード
        public SimpleDelegateCommand<object> LoadVectorItemsCommand => new((parameter) => {
            List<VectorDBItem> items = [];
            Task.Run(() => {
                // VectorDBItemのリストを取得
                items = VectorDBItem.GetVectorDBItems(IsShowSystemCommonVectorDB);
            }).ContinueWith((task) => {
                MainUITask.Run(() => {
                    // VectorDBItemのリストを初期化
                    VectorDBItems.Clear();
                    foreach (var item in items) {
                        VectorDBItems.Add(new VectorDBItemViewModel(item));
                    }
                    OnPropertyChanged(nameof(VectorDBItems));
                });
            });
        });

        // VectorDB Sourceの追加
        public SimpleDelegateCommand<object> AddVectorDBCommand => new((parameter) => {
            SelectedVectorDBItem = new VectorDBItemViewModel(new VectorDBItem());
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {
                // リストを更新
                MainUITask.Run(() => {
                    // リストを更新
                    LoadVectorItemsCommand.Execute();
                });
            });

        });
        // Vector DB編集
        public SimpleDelegateCommand<object> EditVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectVectorDBToEdit);
                return;
            }
            // ベクトルDBの編集Windowを開く
            EditVectorDBWindow.OpenEditVectorDBWindow(SelectedVectorDBItem, (afterUpdate) => {

                // リストを更新
                LoadVectorItemsCommand.Execute();
            });

        });
        // DeleteVectorDBCommand
        public SimpleDelegateCommand<object> DeleteVectorDBCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectVectorDBToDelete);
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteSelectedVectorDB, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                Task.Run(async () => {
                    try {
                        // 選択したVectorDBItemを削除
                        await SelectedVectorDBItem.Item.DeleteAsync();
                        MainUITask.Run(() => {
                            // リストを更新
                            LoadVectorItemsCommand.Execute();
                        });
                    } catch (Exception ex) {
                        LogWrapper.Error(ex.Message);
                    }
                });
            }
        });

        // SelectCommand
        public SimpleDelegateCommand<Window> SelectCommand => new((window) => {
            // SelectedTabIndexが0の場合は、選択したVectorDBItemを返す
            if (SelectedTabIndex == 0) {
                if (SelectedVectorDBItem == null) {
                    LogWrapper.Error(CommonStringResources.Instance.SelectVectorDBPlease);
                    return;
                }
                VectorDBProperty? prop = new() { VectorDBItemName = SelectedVectorDBItem.Item.Name };
                callBackup?.Invoke(prop);
            }
            // SelectedTabIndexが1の場合は、選択したFolderのVectorDBItemを返す
            else if (SelectedTabIndex == 1) {
                VectorDBProperty? item = FolderViewModel?.Folder.GetMainVectorSearchProperty();
                if (item == null) {
                    LogWrapper.Error(CommonStringResources.Instance.SelectVectorDBPlease);
                    return;
                }
                callBackup?.Invoke(item);
            }
            // Windowを閉じる
            window.Close();
        });

    }
}

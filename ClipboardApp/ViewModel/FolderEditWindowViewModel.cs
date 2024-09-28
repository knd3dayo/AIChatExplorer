using System.Windows;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
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

    }

}

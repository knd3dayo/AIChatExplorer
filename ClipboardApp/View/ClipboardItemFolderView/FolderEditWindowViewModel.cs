using System.Windows;
using WpfAppCommon.Utils;
using WpfAppCommon.Model;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    public partial class FolderEditWindowViewModel : MyWindowViewModel {


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
        public void Initialize(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            _afterUpdate = afterUpdate;
            FolderViewModel = folderViewModel;

        }
        public SimpleDelegateCommand<Window> CreateCommand => new ((window) => {
            if (FolderViewModel == null) {
                Tools.Error("フォルダが指定されていません");
                return;
            }
            // CollectionNameが空の場合はエラー
            if (FolderViewModel.CollectionName == "") {
                Tools.Error("フォルダ名を入力してください");
                return;
            }
            // DisplayNameが空の場合はエラー
            if (FolderViewModel.DisplayName == "") {
                Tools.Error("表示名を入力してください");
                return;
            }
            // CollectionNameが[a-Z$_]以外の場合はエラー
            if (!MyRegex().IsMatch(FolderViewModel.CollectionName)) {
                Tools.Error("フォルダ名は英文字で入力してください");
                return;
            }

            FolderViewModel.Save();
            // フォルダ作成後に実行するコマンドが設定されている場合
            _afterUpdate?.Invoke();
            // ウィンドウを閉じる
            window.Close();
        });


        public SimpleDelegateCommand<Window> CancelCommand => new ((window) => {
            // ウィンドウを閉じる
            window.Close();

        });

        [System.Text.RegularExpressions.GeneratedRegex("^[a-zA-Z0-9]+$")]
        private static partial System.Text.RegularExpressions.Regex MyRegex();
    }

}

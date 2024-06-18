using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Views.ClipboardItemView {
    public class ClipboardItemFolderContextMenuItems : ObservableCollection<ClipboardAppMenuItem> {

        private ClipboardItemViewModel _itemViewModel;

        public ClipboardItemFolderContextMenuItems(ClipboardItemViewModel itemViewModel) {
            _itemViewModel = itemViewModel;
            InitContextMenu();
        }

        private void InitContextMenu() {
            // MainViewModelが存在しない場合は何もしない
            if (MainWindowViewModel.ActiveInstance == null) {
                return;
            }
            // コンテキストメニューの初期化
            Add(new ClipboardAppMenuItem("開く", MainWindowViewModel.ActiveInstance.OpenSelectedItemCommand, "Ctrl+O"));
            Add(new ClipboardAppMenuItem("テキストファイルとして開く", MainWindowViewModel.ActiveInstance.OpenContentAsFileCommand, "Ctrl+Shit+O"));
            Add(new ClipboardAppMenuItem("ピン留め", ClipboardItemViewModel.ChangePinCommand));

            Add(new ClipboardAppMenuItem("コピー", MainWindowViewModel.ActiveInstance.CopyItemCommand, "Ctrl+C"));
            Add(new ClipboardAppMenuItem("削除", MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand, "Delete"));

        }
    }
}

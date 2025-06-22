using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;

namespace AIChatExplorer.ViewModel.Folders.ScreenShot {
    public class IntegratedMonitorHistoryFolderMenu : ApplicationFolderMenu {

        public IntegratedMonitorHistoryFolderViewModel FolderViewModel { get; private set; }

        public IntegratedMonitorHistoryFolderMenu(IntegratedMonitorHistoryFolderViewModel folderViewModel) : base(folderViewModel) {
            FolderViewModel = folderViewModel;
        }

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                //テキストの抽出
                menuItems.Add(ExtractTextMenuItem);

                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);

                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;

                #endregion
            }
        }


    }
}

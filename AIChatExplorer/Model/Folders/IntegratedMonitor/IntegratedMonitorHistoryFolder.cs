using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Main;

namespace AIChatExplorer.Model.Folders.ScreenShot {
    public class IntegratedMonitorHistoryFolder : ApplicationFolder {

        // コンストラクタ
        public IntegratedMonitorHistoryFolder() : base() {
            FolderTypeString = FolderManager.SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN;
        }
    }
}

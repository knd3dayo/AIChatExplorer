using AIChatExplorer.Model.Folders.Application;
using LibMain.Model.Folders;

namespace AIChatExplorer.Model.Folders.ScreenShot {
    public class ScreenShotHistoryFolder : ApplicationFolder {

        // コンストラクタ
        public ScreenShotHistoryFolder() : base() {
            FolderTypeString = FolderManager.SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN;
        }
    }
}

using System.Drawing;
using System.Windows.Forms;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using LibPythonAI.Common;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using WpfAppCommon.Model;

namespace AIChatExplorer.Model.Folders.ScreenShot {
    /// <summary>
    /// Class for clipboard monitoring feature
    /// </summary>
    public class ScreenShotController {

        private static ScreenShotController? _instance;
        public static ScreenShotController Instance {
            get {
                _instance ??= new ScreenShotController();
                return _instance;
            }
        }

        // コンストラクタ
        private ScreenShotController() {

        }

        public bool IsScreenMonitorEnabled { get; set; } = false;

        public Action<ContentItemWrapper> AfterTakeScreenShot { get; set; } = (parameter) => { };

        public ApplicationFolder? Folder { get; set; }

        public int Interval { get; set; } = 10; // Interval in seconds for taking screenshots
        public void Start(ApplicationFolder folder, int interval,  Action<ContentItemWrapper> afterClipboardChanged) {
            AfterTakeScreenShot = afterClipboardChanged;
            // Enable clipboard monitoring
            IsScreenMonitorEnabled = true;
            Folder = folder;
            Interval = interval;
            ProcessApplicationItem();

        }

        /// <summary>
        /// Stop clipboard monitoring
        /// </summary>
        public void Stop() {
            IsScreenMonitorEnabled = false;
        }

        /// Create ContentItem
        public static ApplicationItem CreateApplicationItem(
            ApplicationFolder clipboardFolder, System.Drawing.Image image) {

            ContentItemTypes.ContentItemTypeEnum contentItemTypes;
            string sourceType;
            contentItemTypes = ContentItemTypes.ContentItemTypeEnum.Image;
            sourceType = ContentSourceType.Application;


            ApplicationItem item = new(clipboardFolder.Entity) {
                ContentType = contentItemTypes,
                SourceType = sourceType

            };
            // byte
            item.Base64Image = ContentItemTypes.GetBase64StringFromImage(image);

            return item;

        }
        public static System.Drawing.Image? TakeScreenShot() {
            // Take a screenshot of the entire screen
            // デスクトップのサイズを取得
            Rectangle? bounds = Screen.PrimaryScreen?.Bounds;
            if (bounds == null) {
                LogWrapper.Info("Failed to get screen bounds.");
                return null;
            }
            // スクリーンショットを保存するためのビットマップを作成
            Bitmap screenshot = new(bounds.Value.Width, bounds.Value.Height);

            // ビットマップにスクリーンショットを描画
            using (Graphics g = Graphics.FromImage(screenshot)) {
                g.CopyFromScreen(bounds.Value.X, bounds.Value.Y, 0, 0, bounds.Value.Size);
            }
            // スクリーンショットを返す
            System.Drawing.Image image = screenshot;
            return image;
        }

        public void ProcessApplicationItem() {
            if (Folder == null) {
                LogWrapper.Error($"{CommonStringResources.Instance.Folder} is null.");
                return;
            }
            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;
            // Execute in a separate thread
            Task.Run(async () => {
                while (Instance.IsScreenMonitorEnabled) {
                    StatusText statusText = StatusText.Instance;
                    MainUITask.Run(() => {
                        statusText.UpdateInProgress(true, CommonStringResources.Instance.AutoProcessing);
                    });
                    try {
                        // Take a screenshot
                        System.Drawing.Image? image = TakeScreenShot();
                        if (image == null) {
                            // If the image is null, return
                            continue;
                        }
                        // Create a new ApplicationItem
                        ApplicationItem item = CreateApplicationItem(Folder, image);
                        // Apply automatic processing
                        ContentItemWrapper updatedItemTask = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                        if (updatedItemTask == null) {
                            // If the item is ignored, return
                            return;
                        }
                        // アイテムの内容からユーザーの意図を推測する。
                        if (configParams.IsAutoPredictUserIntentEnabled()) {
                            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetBackgroundInfo);
                            await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.PredictUserIntentFromImage.ToString());
                        }

                        // Notify the completion of processing
                        AfterTakeScreenShot(updatedItemTask);

                    } catch (System.Exception ex) {
                        LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                    } finally {
                        MainUITask.Run(() => {
                            statusText.UpdateInProgress(false);
                        });
                    }

                    // Wait for Next Screenshot // interval seconds * 1000 milliseconds
                    Thread.Sleep(Interval * 1000);
                }
            });
        }

    }
}

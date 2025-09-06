using System.Reflection;
using AIChatExplorer.Model.Folders.Application;
using LibPythonAI.Common;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Resources;
using LibUIMain.Resource;
using LibUIMain.ViewModel.Common;
using LibUIMain.Utils;
using LibPythonAI.Utils.Common;
using WK.Libraries.SharpClipboardNS;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.Model.Folders.ClipboardHistory {
    /// <summary>
    /// Class for clipboard monitoring feature
    /// </summary>
    public class ClipboardController {
        //--------------------------------------------------------------------------------
        // 最後に処理したクリップボードのEventArgs
        public static ClipboardChangedEventArgs? LastClipboardChangedEventArgs { get; set; } = null;

        private static ClipboardController? _instance;
        public static ClipboardController Instance {
            get {
                _instance ??= new ClipboardController();
                return _instance;
            }
        }

        // コンストラクタ
        private ClipboardController() {
            // コンストラクタ
            _clipboard = new SharpClipboard();
            _clipboard.ClipboardChanged += ClipboardChanged;

        }

        // Cutフラグ
        public enum CutFlagEnum {
            None,
            Item,
            Folder
        }
        public CutFlagEnum CutFlag { get; set; } = CutFlagEnum.None;

        public ApplicationFolder? Folder { get; set; } = null;

        /// <summary>
        /// コピーされたアイテム
        /// </summary>
        // Ctrl + C or X が押された時のApplicationItem or ApplicationFolder
        public List<object> CopiedObjects { get; set; } = [];

        // ClipboardChangedが呼ばれたときの処理
        public Action<ClipboardChangedEventArgs> OnClipboardChanged { get; set; } = (e) => { };

        // Application monitoring enable/disable flag
        public bool IsClipboardMonitorEnabled { get; set; } = false;
        private SharpClipboard _clipboard;
        private Action<ContentItem> _afterClipboardChanged = (parameter) => { };

        /// <summary>
        /// Start clipboard monitoring
        /// </summary>
        /// <param name="afterClipboardChanged"></param>
        public void Start(ApplicationFolder contentFolder, Action<ContentItem> afterClipboardChanged) {
            // Set the folder to save clipboard history
            Folder = contentFolder;
            _afterClipboardChanged = afterClipboardChanged;
            // Enable clipboard monitoring
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Stop clipboard monitoring
        /// </summary>
        public void Stop() {
            IsClipboardMonitorEnabled = false;
        }
        /// <summary>
        /// Copy the content of ContentItem to the clipboard
        /// </summary>
        /// <param name="item"></param>
        public void SetDataObject(ContentItem item) {
            // System.Windows.MessageBox.Show(item.Path);

            IsClipboardMonitorEnabled = false;
            // If ContentType is Text, copy to clipboard
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                if (item.Content == null) {
                    return;
                }
                System.Windows.Clipboard.SetDataObject(item.Content);
            }
            // If ContentType is Files, copy files to clipboard
            else if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                // SourcePathの取得
                System.Collections.Specialized.StringCollection strings = [item.SourcePath];
                // Stringsが空の場合は何もしない
                if (strings.Count == 0) {
                    return;
                }
                System.Windows.Clipboard.SetFileDropList(strings);
            }
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Processing when the content of the clipboard is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object? sender, ClipboardChangedEventArgs e) {
            LastClipboardChangedEventArgs = e;

            // このアプリケーションのクリップボード操作は無視
            var assembly = Assembly.GetExecutingAssembly().GetName();
            if (e.SourceApplication.Name == assembly.Name + ".exe") {
                return;
            }
            OnClipboardChanged(e);


            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Application monitor disabled");
                return;
            }
            // Determine if it is a target application for monitoring
            if (!IsMonitorTargetApp(e)) {
                return;
            }

            if (_clipboard == null) {
                return;
            }
            if (Folder == null) {
                // If Folder is not set, do not process
                return;
            }
            ProcessClipboardItem(e, Folder, _afterClipboardChanged);

        }


        /// Create ContentItem
        public static List<ApplicationItem> CreateApplicationItem(
            ApplicationFolder clipboardFolder, ClipboardChangedEventArgs e) {

            List<ApplicationItem> result = [];

            ContentItemTypes.ContentItemTypeEnum contentItemTypes;
            string sourceType;
            if (e.ContentType == ContentTypes.Text) {
                contentItemTypes = ContentItemTypes.ContentItemTypeEnum.Text;
                sourceType = ContentSourceType.Application;
            } else if (e.ContentType == ContentTypes.Files) {
                contentItemTypes = ContentItemTypes.ContentItemTypeEnum.Files;
                sourceType = ContentSourceType.File;
            } else if (e.ContentType == ContentTypes.Image) {
                contentItemTypes = ContentItemTypes.ContentItemTypeEnum.Image;
                sourceType = ContentSourceType.Application;
            } else if (e.ContentType == ContentTypes.Other) {
                return result;
            } else {
                return result;
            }

            // If ContentType is Text, set text data
            if (contentItemTypes == ContentItemTypes.ContentItemTypeEnum.Text) {
                ApplicationItem item = new(clipboardFolder.Entity) {
                    ContentType = contentItemTypes,
                    SourceType = sourceType
                };
                ApplicationFolder.SetApplicationInfo(item, e);
                item.Content = (string)e.Content;
                result.Add(item);
                return result;
            }

            // If ContentType is BitmapImage, set image data
            if (contentItemTypes == ContentItemTypes.ContentItemTypeEnum.Image) {
                ApplicationItem item = new(clipboardFolder.Entity) {
                    ContentType = contentItemTypes,
                    SourceType = sourceType

                };
                ApplicationFolder.SetApplicationInfo(item, e);
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                // byte
                item.Base64Image = ContentItemTypes.GetBase64StringFromImage(image);
                result.Add(item);
                return result;
            }

            // If ContentType is Files, set file data
            if (contentItemTypes == ContentItemTypes.ContentItemTypeEnum.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ApplicationItem item = new(clipboardFolder.Entity) {
                        ContentType = contentItemTypes,
                        SourceType = sourceType
                    };
                    ApplicationFolder.SetApplicationInfo(item, e);
                    item.SourcePath = files[i];
                    item.LastModified = new System.IO.FileInfo(item.SourcePath).LastWriteTime.Ticks;
                    result.Add(item);
                }
                return result;
            }
            return result;
        }

        public static void ProcessClipboardItem(ClipboardChangedEventArgs e, ApplicationFolder folder, Action<ContentItem> _afterClipboardChanged) {

            // Get the cut/copied text.
            List<ApplicationItem> items = CreateApplicationItem(folder, e);

            foreach (var item in items) {
                // Process clipboard applicationItem
                ProcessApplicationItem(item, _afterClipboardChanged);
            }
        }


        /// <summary>
        /// Determine if the clipboard item is a monitoring target
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsMonitorTargetApp(ClipboardChangedEventArgs e) {
            // If MonitorTargetAppNames is not an empty string and does not contain in MonitorTargetAppNames, do not process
            var names = PythonAILibManager.Instance.ConfigParams.GetMonitorTargetAppNames();
            if (names != "") {
                // Compare uppercase letters
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = names.ToUpper();
                if (!upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
                    return false;
                }
            }
            return true;
        }

        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessApplicationItem(ContentItem item, Action<ContentItem> _afterClipboardChanged) {

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;
            // Execute in a separate thread
            Task.Run(async () => {
                StatusText statusText = StatusText.Instance;
                MainUITask.Run(() => {
                    statusText.UpdateInProgress(true, CommonStringResources.Instance.AutoProcessing);
                });
                try {
                    // Apply automatic processing
                    ContentItem updatedItemTask = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                    if (updatedItemTask == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // アイテムの内容からユーザーの意図を推測する。
                    if (configParams.IsAutoPredictUserIntentEnabled()) {
                        LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetBackgroundInfo);
                        await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.PredictUserIntentFromClipboard.ToString());
                    }

                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItemTask);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                } finally {
                    MainUITask.Run(() => {
                        statusText.UpdateInProgress(false);
                    });
                }
            });
        }

    }
}

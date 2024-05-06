using System.Windows;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfAppCommon.Factory.Default {
    /// <summary>
    /// Class for clipboard monitoring feature
    /// </summary>
    class DefaultClipboardController : IClipboardController {
        //--------------------------------------------------------------------------------

        // String definition
        private static StringResources StringResources { get; } = StringResources.Instance;

        // Clipboard monitoring enable/disable flag
        public bool IsClipboardMonitorEnabled { get; set; } = false;
        private SharpClipboard? _clipboard = null;
        private Action<ActionMessage> _afterClipboardChanged = (parameter) => { };

        /// <summary>
        /// Start clipboard monitoring
        /// </summary>
        /// <param name="afterClipboardChanged"></param>
        public void Start(Action<ActionMessage> afterClipboardChanged) {
            _afterClipboardChanged = afterClipboardChanged;
            if (_clipboard == null) {
                // Register ClipboardChanged event
                _clipboard = new SharpClipboard();
                _clipboard.ClipboardChanged += ClipboardChanged;
            }
            // Enable clipboard monitoring
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Stop clipboard monitoring
        /// </summary>
        public void Stop() {
            if (_clipboard != null) {
                _clipboard.Dispose();
            }
            IsClipboardMonitorEnabled = false;
        }
        /// <summary>
        /// Copy the content of ClipboardItem to the clipboard
        /// </summary>
        /// <param name="item"></param>
        public void SetDataObject(ClipboardItem item) {
            // System.Windows.MessageBox.Show(item.Content);

            IsClipboardMonitorEnabled = false;
            // If ContentType is Text, copy to clipboard
            if (item.ContentType == ClipboardContentTypes.Text) {
                if (item.Content == null) {
                    return;
                }
                System.Windows.Clipboard.SetDataObject(item.Content);
            }
            // If ContentType is Files, copy files to clipboard
            else if (item.ContentType == ClipboardContentTypes.Files) {
                System.Windows.Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { item.Content });
            }
            IsClipboardMonitorEnabled = true;
        }

        /// <summary>
        /// Processing when the content of the clipboard is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClipboardChanged(object? sender, ClipboardChangedEventArgs e) {

            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Clipboard monitor disabled");
                return;
            }
            if (_clipboard == null) {
                return;
            }
            // Is the content copied of text type?
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                // Get the cut/copied text.
                ProcessClipboardItem(ClipboardContentTypes.Text, _clipboard.ClipboardText, null, e);
            }
            // Is the content copied of file type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                // Get the cut/copied file/files.
                for (int i = 0; i < _clipboard.ClipboardFiles.Count; i++) {
                    ProcessClipboardItem(ClipboardContentTypes.Files, _clipboard.ClipboardFiles[i], null, e);
                }

            }
            // Is the content copied of image type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                // Get the cut/copied image.
                System.Drawing.Image img = _clipboard.ClipboardImage;
                ProcessClipboardItem(ClipboardContentTypes.Image, "", img, e);

            }
            // If the cut/copied content is complex, use 'Other'.
            else if (e.ContentType == SharpClipboard.ContentTypes.Other) {
                // Do nothing
                // System.Windows.MessageBox.Show(_clipboard.ClipboardObject.ToString());
            }
            // Notify the observer
            _afterClipboardChanged?.Invoke(
                ActionMessage.Info(StringResources.ClipboardChangedMessage)
            );

        }
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="contentTypes"></param>
        /// <param name="content"></param>
        /// <param name="image"></param>
        /// <param name="e"></param>
        private void ProcessClipboardItem(ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e) {
            // Determine if it is a target application for monitoring
            if (!IsMonitorTargetApp(e)) {
                return;
            }

            ClipboardItem item = CreateClipboardItem(contentTypes, content, image, e);

            // Execute in a separate thread
            Task.Run(() => {
                string oldReadyText = Tools.StatusText.ReadyText;
                Application.Current.Dispatcher.Invoke(() => {
                    Tools.StatusText.ReadyText = StringResources.AutoProcessing;
                });
                try {
                    // Apply automatic processing
                    ApplyAutoAction(item, image);
                    // Add ClipboardItem to RootFolder
                    ClipboardFolder.RootFolder.AddItem(item, _afterClipboardChanged);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.AddItemFailed}\n{ex.Message}");
                } finally {
                    Application.Current.Dispatcher.Invoke(() => {
                        Tools.StatusText.ReadyText = oldReadyText;
                    });
                }
            });
        }

        /// <summary>
        /// Determine if the clipboard item is a monitoring target
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsMonitorTargetApp(ClipboardChangedEventArgs e) {
            // If MonitorTargetAppNames is not an empty string and does not contain in MonitorTargetAppNames, do not process
            if (ClipboardAppConfig.MonitorTargetAppNames != "") {
                // Compare uppercase letters
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = ClipboardAppConfig.MonitorTargetAppNames.ToUpper();
                if (!upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
                    return false;
                }
            }
            return true;
        }

        /// Create ClipboardItem
        private static ClipboardItem CreateClipboardItem(ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e) {
            ClipboardItem item = new() {
                ContentType = contentTypes
            };
            SetApplicationInfo(item, e);
            item.Content = content;
            item.CollectionName = ClipboardFolder.RootFolder.AbsoluteCollectionName;

            // If ContentType is Image, set image data
            if (contentTypes == ClipboardContentTypes.Image && image != null) {
                ClipboardItemImage imageItem = new();
                imageItem.SetImage(image);
                item.ClipboardItemImage = imageItem;
            }
            // If ContentType is Files, set file data
            else if (contentTypes == ClipboardContentTypes.Files) {
                ClipboardItemFile clipboardItemFile = new(content);
                item.ClipboardItemFile = clipboardItemFile;
            }
            return item;

        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        private static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static void ApplyAutoAction(ClipboardItem item, System.Drawing.Image? image) {
            // ★TODO Implement processing based on automatic processing rules.

            // If AUTO_DESCRIPTION is set, automatically set the Description
            if (ClipboardAppConfig.AutoDescription) {
                try {
                    Tools.Info(StringResources.AutoSetTitle);
                    ClipboardItem.CreateAutoDescription(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.AutoSetTitle}\n{ex.Message}");
                }
            }
            // ★TODO Implement processing based on automatic processing rules.
            // If AUTO_TAG is set, automatically set the tags
            if (ClipboardAppConfig.AutoTag) {
                Tools.Info(StringResources.AutoSetTag);
                try {
                    ClipboardItem.CreateAutoTags(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.SetTagFailed}\n{ex.Message}");
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle) {
                Tools.Info(StringResources.AutoMerge);
                try {
                    ClipboardFolder.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.MergeFailed}\n{ex.Message}");
                }
            }
            // ★TODO Implement processing based on automatic processing rules.
            // If UseOCR is set, perform OCR
            if (ClipboardAppConfig.UseOCR && image != null) {
                Tools.Info(StringResources.OCR);
                try {
                    string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
                    item.Content = text;
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.OCRFailed}\n{ex.Message}");
                }
            }
        }

    }
}

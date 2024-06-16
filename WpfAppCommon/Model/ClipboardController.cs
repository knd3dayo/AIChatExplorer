using System.Reflection;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfAppCommon.Model {
    /// <summary>
    /// Class for clipboard monitoring feature
    /// </summary>
    public class ClipboardController {
        //--------------------------------------------------------------------------------
        // 最後に処理したクリップボードのEventArgs
        public static ClipboardChangedEventArgs? LastClipboardChangedEventArgs { get; set; } = null;

        // コンストラクタ
        public ClipboardController() {
            // コンストラクタ
            _clipboard = new SharpClipboard();
            _clipboard.ClipboardChanged += ClipboardChanged;

        }
        // Clipboard monitoring enable/disable flag
        public bool IsClipboardMonitorEnabled { get; set; } = false;
        private SharpClipboard _clipboard;
        private Action<ClipboardItem> _afterClipboardChanged = (parameter) => { };

        /// <summary>
        /// Start clipboard monitoring
        /// </summary>
        /// <param name="afterClipboardChanged"></param>
        public void Start(Action<ClipboardItem> afterClipboardChanged) {
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
                // FilePathの取得
                System.Collections.Specialized.StringCollection strings = new();
                foreach (var itemFile in item.ClipboardItemFiles) {
                    strings.Add(itemFile.FilePath);
                }
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

            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Clipboard monitor disabled");
                return;
            }
            // Determine if it is a target application for monitoring
            if (!IsMonitorTargetApp(e)) {
                return;
            }

            // このアプリケーションのクリップボード操作は無視
            var assembly = Assembly.GetExecutingAssembly().GetName();
            if (e.SourceApplication.Name == assembly.Name + ".exe") {
                return;
            }

            if (_clipboard == null) {
                return;
            }
            ClipboardFolder.ProcessClipboardItem(ClipboardFolder.RootFolder, e, _afterClipboardChanged);

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

    }
}

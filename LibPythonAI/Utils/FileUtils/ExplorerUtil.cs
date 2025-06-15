using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LibPythonAI.Utils.FileUtils {
    public class ExplorerUtil {


        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern bool SHGetSpecialFolderPath(nint hwndOwner, StringBuilder lpszPath, int nFolder, bool fCreate);

        const int CSIDL_RECENT = 0x0008; // 最近のドキュメントのフォルダID

        public static List<string> GetRecentFilePaths() {
            List<string> files = [];

            StringBuilder path = new(260);

            if (SHGetSpecialFolderPath(nint.Zero, path, CSIDL_RECENT, false)) {
                Console.WriteLine("最近のドキュメントのフォルダパス: " + path.ToString());
                foreach (var file in Directory.GetFiles(path.ToString())) {
                    if (file.EndsWith(".lnk")) {
                        files.Add(GetShortcutTarget(file));
                    } else {
                        files.Add(file);
                    }
                }
            }
            return files;
        }

        public static string GetShortcutTarget(string shortcutFile) {
            dynamic? shell = null;   // IWshRuntimeLibrary.WshShell
            dynamic? lnk = null;     // IWshRuntimeLibrary.IWshShortcut
            try {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                if (type == null) {
                    throw new Exception("WScript.Shell not found");
                }
                shell = Activator.CreateInstance(type);
                if (shell == null) {
                    throw new Exception("WScript.Shell not found");
                }
                lnk = shell.CreateShortcut(shortcutFile);
                return lnk.TargetPath;

            } finally {
                if (lnk != null) Marshal.ReleaseComObject(lnk);
                if (shell != null) Marshal.ReleaseComObject(shell);
            }
        }
    }
}

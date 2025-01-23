using System.IO;

namespace PythonAILib.Utils.Common {
    internal class Tools {

        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool copyOnNewFile) {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles()) {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                // copyOnNewFile == Trueの場合は、コピー元ファイルが新しい場合のみコピーする
                if (copyOnNewFile && File.Exists(targetFilePath) && File.GetLastWriteTime(targetFilePath) >= file.LastWriteTime) {
                    continue;
                }
                // ファイルが存在する場合は上書きする
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive) {
                foreach (DirectoryInfo subDir in dirs) {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, recursive, copyOnNewFile);
                }
            }
        }
        // Listの要素を要素 > 要素 ... の形式にして返す.最後の要素の後には>はつかない
        // Listの要素がNullの場合はNull > と返す
        public static string ListToString(List<string> list) {
            return list == null ? "Null" : string.Join(" > ", list);
        }
    }
}

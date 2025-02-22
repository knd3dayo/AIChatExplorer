using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibUIPythonAI.Utils {
    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO {
        public nint hIcon;
        public nint iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    public class FileTypeChecker {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern nint SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        public const uint SHGFI_TYPENAME = 0x000000400;

        public static string GetTypeName(string filePath) {
            SHFILEINFO shinfo = new();
            nint hImg = SHGetFileInfo(filePath, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), SHGFI_TYPENAME);

            if (hImg == nint.Zero) {
                Console.WriteLine("File Type: " + shinfo.szTypeName);
                return shinfo.szTypeName;
            }
            return "";
        }
    }
}

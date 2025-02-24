using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.Model.Content {
    public class ContentSourceType {

        // アプリ
        public  static  string Application { get; } = "Application";
        // ファイル
        public static string File { get; } = "File";
        // URL,
        public static string Url { get; } = "Url";

    }
}

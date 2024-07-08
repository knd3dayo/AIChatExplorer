using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.HelpView {
    public class VersionWindowViewModel {

        public string Version { get; set; }

        public string CopyRight { get; set; }

        public string Url { get; set; }

        public string Title { get; set; } = "バージョン情報";
        public VersionWindowViewModel() {
            Version = "0.1.0";
            CopyRight = "© 2024 by knd3dayo";
            Url = "https://github.com/knd3dayo/ClipboardApp";
        }

        public string VersionInfoText {
            get {
                // Title,Version,CopyRight,Urlの情報を改行で連結して返す
                return $"{Title}\nVersion: {Version}\nCopyRight: {CopyRight}\nUrl: {Url}";
            }
        }

        // CloseCommand
        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            window.Close();
        });
    }
}

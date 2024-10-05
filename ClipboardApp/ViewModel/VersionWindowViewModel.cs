using WpfAppCommon.Model;

namespace ClipboardApp.ViewModel {
    public class VersionWindowViewModel : ClipboardAppViewModelBase {

        public string Version { get; set; }

        public string CopyRight { get; set; }

        public string Url { get; set; }

        public string Title { get; set; } = CommonStringResources.Instance.VersionInformation;
        public VersionWindowViewModel() {
            Version = "0.2.1";
            CopyRight = "© 2024 by knd3dayo";
            Url = "https://github.com/knd3dayo/ClipboardApp";
        }

        public string VersionInfoText {
            get {
                // Title,Version,CopyRight,Urlの情報を改行で連結して返す
                return $"{Title}\nVersion: {Version}\nCopyRight: {CopyRight}\nUrl: {Url}";
            }
        }

    }
}

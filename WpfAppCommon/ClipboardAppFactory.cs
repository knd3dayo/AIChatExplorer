using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;

namespace WpfAppCommon {
    public class ClipboardAppFactory {
        // Factory
        public static IClipboardFactory Instance { get; } = new DefaultClipboardFactory();
    }
}


using ClipboardApp.Factory.Default;

namespace ClipboardApp.Factory {
    public class ClipboardAppFactory {
        // Factory
        public static IClipboardFactory Instance { get; } = new DefaultClipboardFactory();
    }
}

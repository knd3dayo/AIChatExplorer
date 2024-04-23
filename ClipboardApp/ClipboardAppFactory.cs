using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipboardApp.Factory.Default;
using ClipboardApp.Factory;

namespace ClipboardApp {
    public class ClipboardAppFactory {
        // Factory
        public static IClipboardFactory Instance { get; } = new DefaultClipboardFactory();
    }
}

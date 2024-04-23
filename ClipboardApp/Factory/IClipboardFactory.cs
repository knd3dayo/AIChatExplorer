using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardApp.Factory {
    public interface IClipboardFactory {

        IClipboardController GetClipboardController();

        IClipboardDBController GetClipboardDBController();

        IBackupController GetBackupController();

        IClipboardProcessController GetClipboardProcessController();



    }
}

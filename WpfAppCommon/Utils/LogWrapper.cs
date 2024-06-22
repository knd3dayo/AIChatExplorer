using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace WpfAppCommon.Utils {
    public class LogWrapper {

        public static NLog.Logger Logger { get; } = LogManager.LogFactory.GetCurrentClassLogger<CustomLogger>();

        public static void Info(string message) {
            Logger.Info(message);
        }

        public static void Warn(string message) {
            Logger.Warn(message);
        }

        public static void Error(string message) {
            Logger.Error(message);
        }



    }
}

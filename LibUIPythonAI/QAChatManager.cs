using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QAChat.Abstract;

namespace QAChat {
    public class QAChatManager {
    
        public static QAChatManager? Instance { get; private set; }

        public IQAChatConfigParams ConfigParams { get; private set; }

        private QAChatManager(IQAChatConfigParams parmas) {

            ConfigParams = parmas;

        }
        public static void Init(IQAChatConfigParams parmas) {

            Instance = new QAChatManager(parmas);
        }


    }
}

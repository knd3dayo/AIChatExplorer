using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class ChatResult {

        public List<string> ReferencedContents { get; set; } = new List<string>();
        public List<string> ReferencedFilePath { get; set; } = new List<string>();

        public string Response { get; set; } = "";

        public string Verbose { get; set; } = "";

        public ChatResult() { }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class ChatResult {

        public List<Dictionary<string,string>> ReferencedContents { get; set; } = [];
        public List<string> ReferencedFilePath { get; set; } = [];

        public string Response { get; set; } = "";

        public string Verbose { get; set; } = "";

        public ChatResult() { }

    }

}

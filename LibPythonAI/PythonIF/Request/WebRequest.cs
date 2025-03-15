using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class WebRequest {

        public WebRequest(string url) {
            URL = url;
        }

        // URL
        public string URL { get; set; } = "";


        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["url"] = URL;
            return dict;
        }
    }
}

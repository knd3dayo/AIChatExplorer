using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class AutogenRequest {

        public AutogenRequest(string inputText) {
            InputText = inputText;
        }

        public string InputText { get; private set; } = "";



        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["input_text"] = InputText;
            return dict;
        }
    }
}

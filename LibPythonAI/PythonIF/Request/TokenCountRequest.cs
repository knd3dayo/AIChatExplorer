using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class TokenCountRequest {

        public TokenCountRequest(string model, string inputText) {
            Model = model;
            InputText = inputText;
        }
        public string InputText { get; private set; }

        public string Model { get; private set; }
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["model"] = Model;
            dict["input_text"] = InputText;
            return dict;
        }
    }
}

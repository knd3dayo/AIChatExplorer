using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class FileRequest {

        // FilePath
        public string FilePath { get; set; } = "";
        // Extension
        public string Extension { get; set; } = "";
        // Base64Data
        public string Base64Data { get; set; } = "";


        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["file_path"] = FilePath;
            dict["extension"] = Extension;
            dict["base64_data"] = Base64Data;
            return dict;
        }
    }
}

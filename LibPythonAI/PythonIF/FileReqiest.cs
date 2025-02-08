using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF {
    public class FileRequest {

        // FilePath
        public string FilePath { get;  set; } = "";
        // Extension
        public string Extension { get;  set; } = "";
        // Base64Data
        public string Base64Data { get;  set; } = "";

        // ExcelSheetName
        public string ExcelSheetName { get;  set; } = "";

        // URL
        public string URL { get;  set; } = "";

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["file_path"] = FilePath;
            dict["extension"] = Extension;
            dict["base64_data"] = Base64Data;
            dict["excel_sheet_name"] = ExcelSheetName;
            dict["url"] = URL;
            return dict;
        }
    }
}

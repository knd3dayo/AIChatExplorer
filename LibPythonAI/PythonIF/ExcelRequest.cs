using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model.File;

namespace LibPythonAI.PythonIF {
    public class ExcelRequest {

        public ExcelRequest(string filePath, CommonDataTable data) {
            FilePath = filePath;
            Data = data;
        }
        public string FilePath { get; private set; } = "";
        public CommonDataTable Data { get; private set; } = new CommonDataTable([]);


        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["file_path"] = FilePath;
            dict["data_json"] = CommonDataTable.ToJson(Data);
            return dict;
        }
    }
}

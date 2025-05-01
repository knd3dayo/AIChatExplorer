using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class ContentFolderRequest {


        public string Id { get; set; } = "";

        public string FolderName { get; set; } = "";

        // Description
        public string Description { get; set; } = "";

        // FolderTypeString
        public string FolderTypeString { get; set; } = "";
        // IsRootFolder
        public bool IsRootFolder { get; set; } = false;

        // ExtendedPropertiesJson
        public string ExtendedPropertiesJson { get; set; } = "{}";

        // ParentId
        public string? ParentId { get; set; } = null;


        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                ["Id"] = Id,
                ["FolderName"] = FolderName,
                ["FolderTypeString"] = FolderTypeString,
                ["Description"] = Description,
                ["IsRootFolder"] = IsRootFolder,
                ["ExtendedPropertiesJson"] = ExtendedPropertiesJson
            };
            if (ParentId != null) {
                dict["parent_id"] = ParentId;
            }
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<ContentFolderRequest> requests) {
            List<Dictionary<string, object>> dictList = new();
            foreach (var request in requests) {
                dictList.Add(request.ToDict());
            }
            return dictList;
        }
        // FromDict
        public static ContentFolderRequest FromDict(Dictionary<string, object> dict) {
            ContentFolderRequest request = new() {
                Id = dict["Id"]?.ToString() ?? "",
                FolderName = dict["FolderName"]?.ToString() ?? "",
                Description = dict["Description"]?.ToString() ?? "",
                IsRootFolder = Convert.ToBoolean(dict["IsRootFolder"]),
                ExtendedPropertiesJson = dict["ExtendedPropertiesJson"]?.ToString() ?? "{}"
            };
            if (dict.ContainsKey("parent_id")) {
                request.ParentId = dict["parent_id"]?.ToString();
            } else {
                request.ParentId = null;
            }

            return request;
        }
        // FromDictList
        public static List<ContentFolderRequest> FromDictList(List<Dictionary<string, object>> dicts) {
            List<ContentFolderRequest> requests = new();
            foreach (var dict in dicts) {
                requests.Add(FromDict(dict));
            }
            return requests;
        }


    }
}

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

        public string? FolderPath { get; set; } = null;

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                ["id"] = Id,
                ["folder_name"] = FolderName,
                ["folder_type_string"] = FolderTypeString,
                ["description"] = Description,
                ["is_root_folder"] = IsRootFolder,
                ["extended_properties_json"] = ExtendedPropertiesJson
            };
            if (ParentId != null) {
                dict["parent_id"] = ParentId;
            }
            if (FolderPath != null) {
                dict["folder_path"] = FolderPath;
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
                Id = dict["id"]?.ToString() ?? "",
                FolderName = dict["folder_name"]?.ToString() ?? "",
                Description = dict["description"]?.ToString() ?? "",
                IsRootFolder = Convert.ToBoolean(dict["is_root_folder"]),
                ExtendedPropertiesJson = dict["extended_properties_json"]?.ToString() ?? "{}"
            };
            if (dict.ContainsKey("parent_id")) {
                request.ParentId = dict["parent_id"]?.ToString();
            } else {
                request.ParentId = null;
            }
            if (dict.ContainsKey("folder_path")) {
                request.FolderPath = dict["folder_path"]?.ToString();
            } else {
                request.FolderPath = null;
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

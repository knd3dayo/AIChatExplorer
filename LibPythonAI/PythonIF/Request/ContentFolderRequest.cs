using LibPythonAI.Model.Content;

namespace LibPythonAI.PythonIF.Request {
    public class ContentFolderRequest {

        public const string ID_KEY = "id";
        public const string FOLDER_NAME_KEY = "folder_name";
        public const string FOLDER_TYPE_STRING_KEY = "folder_type_string";
        public const string DESCRIPTION_KEY = "description";
        public const string IS_ROOT_FOLDER_KEY = "is_root_folder";
        public const string EXTENDED_PROPERTIES_JSON_KEY = "extended_properties_json";
        public const string PARENT_ID_KEY = "parent_id";
        public const string FOLDER_PATH_KEY = "folder_path";


        public ContentFolderRequest(ContentFolderWrapper contentFolderWrapper) {
            Id = contentFolderWrapper.Id.ToString();
            FolderName = contentFolderWrapper.FolderName;
            FolderTypeString = contentFolderWrapper.FolderTypeString;
            Description = contentFolderWrapper.Description;
            IsRootFolder = contentFolderWrapper.IsRootFolder;
            ExtendedPropertiesJson = contentFolderWrapper.ExtendedPropertiesJson;
            ParentId = contentFolderWrapper.Parent?.Id?.ToString();
        }

        public string Id { get; set; }

        public string FolderName { get; set; }

        // Description
        public string Description { get; set; }

        // FolderTypeString
        public string FolderTypeString { get; set; }
        // IsRootFolder
        public bool IsRootFolder { get; set; } = false;

        // ExtendedPropertiesJson
        public string ExtendedPropertiesJson { get; set; }

        // ParentId
        public string? ParentId { get; set; } 

        // FolderPath
        public string? FolderPath { get; set; } 

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { FOLDER_NAME_KEY, FolderName },
                { FOLDER_TYPE_STRING_KEY, FolderTypeString },
                { DESCRIPTION_KEY, Description },
                { IS_ROOT_FOLDER_KEY, IsRootFolder },
                { EXTENDED_PROPERTIES_JSON_KEY, ExtendedPropertiesJson }
            };
            if (ParentId != null) {
                dict[PARENT_ID_KEY] = ParentId;
            }
            if (FolderPath != null) {
                dict[FOLDER_PATH_KEY] = FolderPath;
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
    }
}

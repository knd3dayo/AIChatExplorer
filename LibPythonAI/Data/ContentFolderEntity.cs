
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibGit2Sharp;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace LibPythonAI.Data {
    public class ContentFolderEntity {

        public const string ID_KEY = "id";
        public const string FOLDER_NAME_KEY = "folder_name";
        public const string FOLDER_TYPE_STRING_KEY = "folder_type_string";
        public const string DESCRIPTION_KEY = "description";
        public const string IS_ROOT_FOLDER_KEY = "is_root_folder";
        public const string EXTENDED_PROPERTIES_JSON_KEY = "extended_properties_json";
        public const string PARENT_ID_KEY = "parent_id";
        public const string FOLDER_PATH_KEY = "folder_path";

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // フォルダの種類の文字列
        public string FolderTypeString { get; set; } = "Normal";


        // 親フォルダのID

        public string? ParentId { get; set; }

        //　フォルダ名
        public string FolderName { get; set; } = "";

        // Description
        public string Description { get; set; } = "";


        public string ExtendedPropertiesJson { get; set; } = "{}";

        private Dictionary<string, object?>? _extendedProperties;
        // 拡張プロパティ (Dictionary<string, object> は EF でサポートされないため、Json で保存)
        [NotMapped]
        public Dictionary<string, object?> ExtendedProperties {
            get {
                if (_extendedProperties == null) {
                    _extendedProperties = JsonUtil.ParseJson(ExtendedPropertiesJson);
                }
                return _extendedProperties ?? [];
            }
        }

        // IsRootFolder
        public bool IsRootFolder { get; set; } = false;

        public void SaveExtendedPropertiesJson() {
            if (_extendedProperties != null) {
                ExtendedPropertiesJson = JsonSerializer.Serialize(ExtendedProperties, JsonUtil.JsonSerializerOptions);
            }
        }


        public List<ContentFolderEntity> GetChildren() {
            using PythonAILibDBContext context = new();
            var items = context.ContentFolders
                .Where(x => x.ParentId == this.Id).ToList();
            return items;
        }

        public List<ContentFolderEntity> GetChildrenAll() {
            List<ContentFolderEntity> result = [];
            foreach (var child in GetChildren()) {
                result.Add(child);
                result.AddRange(child.GetChildrenAll());
            }
            return result;
        }

        // ルートフォルダを取得  ParentIdがnullでFolderTypeStringが一致するものを取得
        public ContentFolderEntity GetRootFolder() {
            using PythonAILibDBContext context = new();
            var folder = context.ContentFolders
                .FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == FolderTypeString);
            if (folder == null) {
                throw new Exception("Root folder not found");
            }
            return folder;
        }


        public static ContentFolderEntity? GetFolder(string? id) {
            if (id == null) {
                return null;
            }
            using PythonAILibDBContext context = new();
            var folder = context.ContentFolders
                .FirstOrDefault(x => x.Id == id);
            return folder;
        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            ContentFolderEntity other = (ContentFolderEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
        // ToDict
        public Dictionary<string, object> ToDict() {
            // ExtendedPropertiesをJsonに変換
            SaveExtendedPropertiesJson();
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { FOLDER_NAME_KEY, FolderName },
                { DESCRIPTION_KEY, Description },
                { IS_ROOT_FOLDER_KEY, IsRootFolder },
                { FOLDER_TYPE_STRING_KEY, FolderTypeString },
                { EXTENDED_PROPERTIES_JSON_KEY, ExtendedPropertiesJson },
            };
            if (ParentId != null) {
                dict[PARENT_ID_KEY] = ParentId;
            }

            return dict;
        }
        // FromDict
        public static ContentFolderEntity FromDict(Dictionary<string, object> dict) {
            ContentFolderEntity folderEntity = new() {
                Id = dict[ID_KEY]?.ToString() ?? "",
                ParentId = dict[PARENT_ID_KEY]?.ToString() ?? null,
                FolderName = dict[FOLDER_NAME_KEY]?.ToString() ?? "",
                Description = dict[DESCRIPTION_KEY]?.ToString() ?? "",
                FolderTypeString = dict[FOLDER_TYPE_STRING_KEY]?.ToString() ?? "",
                ExtendedPropertiesJson = dict[EXTENDED_PROPERTIES_JSON_KEY].ToString() ?? "{}",
            };
            return folderEntity;

        }


    }
}

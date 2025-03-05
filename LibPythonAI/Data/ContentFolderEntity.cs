
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.EntityFrameworkCore;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {
    public class ContentFolderEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // フォルダの種類の文字列
        public string FolderTypeString { get; set; } = "Normal";


        // 親フォルダのID

        public string? ParentId { get; set; }

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        //　フォルダ名
        public string FolderName { get; set; } = "";

        // Description
        public string Description { get; set; } = "";

        //　OS上のフォルダ名
        public string ContentOutputFolderPrefix { get; set; } = "";

        public List<VectorDBPropertyEntity> VectorDBProperties { get; set; } = new();

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

        public void SaveExtendedPropertiesJson() {
            if (_extendedProperties != null) {
                ExtendedPropertiesJson = JsonSerializer.Serialize(ExtendedProperties, jsonSerializerOptions);
            }
        }


        public List<ContentItemEntity> GetContentItems() {
            using PythonAILibDBContext context = new();
            var items = context.ContentItems
                .Where(x => x.FolderId == this.Id).ToList();
            return items;
        }

        public List<ContentFolderEntity> GetChildren() {
            using PythonAILibDBContext context = new();
            var items = context.ContentFolders
                .Include(b => b.VectorDBProperties)
                .Where(x => x.ParentId == this.Id).ToList();
            return items;
        }

        public static ContentFolderEntity? GetFolder(string? id) {
            if (id == null) {
                return null;
            }
            using PythonAILibDBContext context = new();
            var folder = context.ContentFolders
                .Include(b => b.VectorDBProperties)
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
    }
}

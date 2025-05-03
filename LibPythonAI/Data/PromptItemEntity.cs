using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {
    public class PromptItemEntity {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;


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

        // 複数アイテムのSave
        public static void SaveItems(List<PromptItemEntity> items) {
            using PythonAILibDBContext context = new();
            foreach (var item in items) {
                item.SaveExtendedPropertiesJson();
                var ExistingItem = context.PromptItems.FirstOrDefault(x => x.Id == item.Id);
                if (ExistingItem == null) {
                    context.PromptItems.Add(item);
                } else {
                    context.Entry(ExistingItem).CurrentValues.SetValues(item);
                }
            }
            context.SaveChanges();
        }



        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            PromptItemEntity other = (PromptItemEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}

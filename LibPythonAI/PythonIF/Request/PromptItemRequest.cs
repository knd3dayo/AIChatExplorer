using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPythonAI.Model.Prompt;
namespace LibPythonAI.PythonIF.Request {
    public class PromptItemRequest {

        public const string ID_KEY = "id";
        public const string NAME_KEY = "name";
        public const string DESCRIPTION_KEY = "description";
        public const string PROMPT_KEY = "prompt";
        public const string PROMPT_TEMPLATE_TYPE_KEY = "prompt_template_type";
        public const string EXTENDED_PROPERTIES_JSON_KEY = "extended_properties_json";

        public PromptItemRequest(PromptItem promptItem) {
            Id = promptItem.Id;
            Name = promptItem.Name;
            Description = promptItem.Description;
            Prompt = promptItem.Prompt;
            PromptTemplateType = (int)promptItem.PromptTemplateType;
            ExtendedPropertiesJson = promptItem.ExtendedPropertiesJson;
        }
        // Id
        public string Id { get; set; }

        // Name
        public string Name { get; set; }

        // Description
        public string Description { get; set; }

        // Prompt
        public string Prompt { get; set; }

        // PromptTemplateType
        public int PromptTemplateType { get; set; } 

        // ExtendedPropertiesJson
        public string ExtendedPropertiesJson { get; set; }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { NAME_KEY, Name },
                { DESCRIPTION_KEY, Description },
                { PROMPT_KEY, Prompt },
                { PROMPT_TEMPLATE_TYPE_KEY, PromptTemplateType },
                { EXTENDED_PROPERTIES_JSON_KEY, ExtendedPropertiesJson }
            };
            return dict;
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<PromptItemRequest> promptItemRequests) {
            return promptItemRequests.Select(item => item.ToDict()).ToList();
        }

    }
}

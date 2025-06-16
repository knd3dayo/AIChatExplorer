using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Prompt;
namespace LibPythonAI.PythonIF.Request {
    public class AutoProcessRuleRequest {

        public const string ID_KEY = "id";
        // RuleName
        public const string RULE_NAME_KEY = "rule_name";
        // is_enabled
        public const string IS_ENABLED_KEY = "is_enabled";
        // priority
        public const string PRIORITY_KEY = "priority";
        // conditions_json
        public const string CONDITIONS_JSON_KEY = "conditions_json";
        // auto_process_item_id
        public const string AUTO_PROCESS_ITEM_ID_KEY = "auto_process_item_id";
        // target_folder_id
        public const string TARGET_FOLDER_ID_KEY = "target_folder_id";
        // destination_folder_id
        public const string DESTINATION_FOLDER_ID_KEY = "destination_folder_id";



        public AutoProcessRuleRequest(AutoProcessRule rule) {
            Id = rule.Id;
            RuleName = rule.RuleName;
            IsEnabled = rule.IsEnabled;
            Priority = rule.Priority;
            ConditionsJson = rule.ConditionsJson;
            AutoProcessItemId = rule.AutoProcessItemId;
            TargetFolderId = rule.TargetFolderId;
            DestinationFolderId = rule.DestinationFolderId;
        }
        // Id
        public string Id { get; set; }

        // RuleName
        public string RuleName { get; set; }

        // is_enabled
        public bool IsEnabled { get; set; }

        // priority
        public int Priority { get; set; }

        // conditions_json
        public string ConditionsJson { get; set; }

        // auto_process_item_id
        public string? AutoProcessItemId { get; set; }

        // target_folder_id
        public string? TargetFolderId { get; set; }

        // destination_folder_id
        public string? DestinationFolderId { get; set; }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { RULE_NAME_KEY, RuleName },
                { IS_ENABLED_KEY, IsEnabled },
                { PRIORITY_KEY, Priority },
                { CONDITIONS_JSON_KEY, ConditionsJson },
            };
            if (AutoProcessItemId != null) {
                dict[AUTO_PROCESS_ITEM_ID_KEY] = AutoProcessItemId;
            }
            if (TargetFolderId != null) {
                dict[TARGET_FOLDER_ID_KEY] = TargetFolderId;
            }
            if (DestinationFolderId != null) {
                dict[DESTINATION_FOLDER_ID_KEY] = DestinationFolderId;
            }

            return dict;
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoProcessRuleRequest> promptItemRequests) {
            return promptItemRequests.Select(item => item.ToDict()).ToList();
        }

    }
}

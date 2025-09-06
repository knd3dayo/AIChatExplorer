using LibMain.Model.AutoProcess;
namespace LibMain.PythonIF.Request {
    public class AutoProcessItemRequest {

        public const string ID_KEY = "id";
        public const string DESCRIPTION_KEY = "description";
        // display_name
        public const string DISPLAY_NAME_KEY = "display_name";
        // action_type
        public const string ACTION_TYPE_KEY = "action_type";

        public AutoProcessItemRequest(AutoProcessItem item) {
            Id = item.Id;
            DisplayName = item.DisplayName;
            Description = item.Description;
            ActionType = item.TypeName;

        }
        // Id
        public string Id { get; set; }

        // DisplayName
        public string DisplayName { get; set; }

        // Description
        public string Description { get; set; }

        // ActionType
        public AutoProcessActionTypeEnum ActionType { get; set; }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { DISPLAY_NAME_KEY, DisplayName },
                { DESCRIPTION_KEY, Description },
                { ACTION_TYPE_KEY, (int)ActionType }
            };
            return dict;
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoProcessItemRequest> promptItemRequests) {
            return promptItemRequests.Select(item => item.ToDict()).ToList();
        }

    }
}

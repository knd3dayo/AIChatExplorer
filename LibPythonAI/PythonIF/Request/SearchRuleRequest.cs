using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPythonAI.Model.Search;

namespace LibPythonAI.PythonIF.Request {
    public class SearchRuleRequest {


        public const string ID_KEY = "id";
        // Name
        public const string NAME_KEY = "name";
        // SearchConditionJson
        public const string SEARCH_CONDITION_JSON_KEY = "search_condition_json";
        // SearchFolderId
        public const string SEARCH_FOLDER_ID_KEY = "search_folder_id";
        // TargetFolderId
        public const string TARGET_FOLDER_ID_KEY = "target_folder_id";
        // IsIncludeSubFolder
        public const string IS_INCLUDE_SUB_FOLDER_KEY = "is_include_sub_folder";
        // IsGlobalSearch
        public const string IS_GLOBAL_SEARCH_KEY = "is_global_search";

        public SearchRuleRequest(SearchRule rule) {
            Id = rule.Id;
            Name = rule.Name;
            SearchConditionJson = rule.SearchConditionJson;
            SearchFolderId = rule.SearchFolderId;
            TargetFolderId = rule.TargetFolderId;
            IsIncludeSubFolder = rule.IsIncludeSubFolder;
            IsGlobalSearch = rule.IsGlobalSearch;
        }

        // Id
        public string Id { get; set; }
        // Name
        public string Name { get; set; } = string.Empty;
        // SearchConditionJson
        public string SearchConditionJson { get; set; }

        // SearchFolderId
        public string? SearchFolderId { get; set; }

        // TargetFolderId
        public string? TargetFolderId { get; set; }

        // IsIncludeSubFolder
        public bool IsIncludeSubFolder { get; set; }

        // IsGlobalSearch
        public bool IsGlobalSearch { get; set; }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { NAME_KEY, Name },
                { SEARCH_CONDITION_JSON_KEY, SearchConditionJson },
                { IS_INCLUDE_SUB_FOLDER_KEY, IsIncludeSubFolder },
                { IS_GLOBAL_SEARCH_KEY, IsGlobalSearch }
            };
            if (SearchFolderId != null) {
                dict[SEARCH_FOLDER_ID_KEY] = SearchFolderId;
            }
            if (TargetFolderId != null) {
                dict[TARGET_FOLDER_ID_KEY] = TargetFolderId;
            }
            return dict;
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<SearchRuleRequest> requests) {
            return requests.Select(r => r.ToDict()).ToList();
        }
    }
}

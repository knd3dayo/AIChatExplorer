using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Search;
using LiteDB;
using PythonAILib.Common;

namespace ClipboardApp.Factory
{
    public class ClipboardDBController : PythonAILibDataFactory, IClipboardDBController {

        public const string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        public const string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";
        public const string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";

        //---- 自動処理関連 ----------------------------------------------
        public ILiteCollection<AutoProcessRule> GetAutoProcessRuleCollection() {
            return GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
        }

        public ILiteCollection<SearchRule> GetSearchRuleCollection() {
            return GetDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
        }

    }
}
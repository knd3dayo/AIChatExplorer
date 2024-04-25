using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory {
    public interface IClipboardDBController {


        //-- ClipboardItem
        public void UpsertItem(ClipboardItem item);
        public void DeleteItem(ClipboardItem item);

        //-- ClipboardItemFolder
        public ClipboardItemFolder GetFolder(string collectionName);
        public ClipboardItemFolder GetRootFolder();
        public ClipboardItemFolder GetSearchRootFolder();
        public void UpsertFolderRelation(ClipboardItemFolder parent, ClipboardItemFolder child);

        public void DeleteFolder(ClipboardItemFolder folder);
        public void UpsertFolder(ClipboardItemFolder folder);

        public IEnumerable<string> GetFolderRelations(string parentCollectionName);

        public IEnumerable<ClipboardItem> SearchItems(string collectionName, SearchCondition searchCondition);

        public IEnumerable<ClipboardItem> GetItems(string collectionName);


        // public void DeleteItems(List<ClipboardItem> items);

        // -- SearchRule
        public SearchRule? GetSearchRuleByFolderName(string collectionName);
        public SearchRule? GetSearchRule(string name);

        public void UpsertSearchRule(SearchRule conditionRule);

        // -- AutoProcessRule
        public IEnumerable<AutoProcessRule> GetAutoProcessRules(ClipboardItemFolder? targetFolder);

        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules();

        public void UpsertAutoProcessRule(AutoProcessRule rule);

        public void DeleteAutoProcessRule(AutoProcessRule rule);

        public IEnumerable<AutoProcessRule> GetCopyToMoveToRules();

        //-- Tag 要改修
        public IEnumerable<TagItem> GetTagList();

        public void DeleteTag(string tag);

        public void InsertTag(string tag);


        public IEnumerable<ScriptItem> GetScriptItems();

        public void UpsertPromptTemplate(PromptItem promptItem);

        public ICollection<PromptItem> GetAllPromptTemplates();

        public void DeletePromptTemplate(PromptItem promptItem);
        }

}

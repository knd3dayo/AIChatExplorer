using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory {
    public interface IClipboardDBController {


        //-- ClipboardItem
        public void UpsertItem(ClipboardItem item, bool updateModifiedTime = true);
        public void DeleteItem(ClipboardItem item);

        //-- ClipboardFolder
        public ClipboardFolder GetFolder(string collectionName);
        public ClipboardFolder GetRootFolder();
        public ClipboardFolder GetSearchRootFolder();
        public void UpsertFolderRelation(ClipboardFolder parent, ClipboardFolder child);

        public void DeleteFolder(ClipboardFolder folder);
        public void UpsertFolder(ClipboardFolder folder);

        public IEnumerable<string> GetFolderRelations(string parentCollectionName);

        public IEnumerable<ClipboardItem> SearchItems(string collectionName, SearchCondition searchCondition);

        public IEnumerable<ClipboardItem> GetItems(string collectionName);


        // public void DeleteItems(List<ClipboardItem> items);

        // -- SearchRule
        public SearchRule? GetSearchRuleByFolderName(string collectionName);
        public SearchRule? GetSearchRule(string name);

        public void UpsertSearchRule(SearchRule conditionRule);

        // -- AutoProcessRule
        public IEnumerable<AutoProcessRule> GetAutoProcessRules(ClipboardFolder? targetFolder);

        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules();

        public void UpsertAutoProcessRule(AutoProcessRule rule);

        public void DeleteAutoProcessRule(AutoProcessRule rule);

        public IEnumerable<AutoProcessRule> GetCopyToMoveToRules();

        //-- Tag 要改修
        public IEnumerable<TagItem> GetTagList();

        public void DeleteTag(TagItem tag);

        public void InsertTag(TagItem tag);

        public IEnumerable<TagItem> FilterTag(string tag, bool exclude);

        public IEnumerable<ScriptItem> GetScriptItems();

        public void UpsertPromptTemplate(PromptItem promptItem);

        public ICollection<PromptItem> GetAllPromptTemplates();

        public void DeletePromptTemplate(PromptItem promptItem);
    }

}

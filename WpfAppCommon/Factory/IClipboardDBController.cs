using LiteDB;
using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory {
    public interface IClipboardDBController {


        //-- ClipboardItem
        public void UpsertItem(ClipboardItem item, bool updateModifiedTime = true);
        public void DeleteItem(ClipboardItem item);
        public IEnumerable<ClipboardItem> SearchItems(string collectionName, SearchCondition searchCondition);
        public IEnumerable<ClipboardItem> GetItems(string collectionName);

        //-- ClipboardItemImage
        public void UpsertItemImage(ClipboardItemImage item);
        public void DeleteItemImage(ClipboardItemImage item);
        public ClipboardItemImage? GetItemImage(ObjectId id);

        //-- ClipboardItemFile
        public void UpsertItemFile(ClipboardItemFile item);
        public void DeleteItemFile(ClipboardItemFile item);
        public ClipboardItemFile? GetItemFile(ObjectId id);

        //-- ClipboardFolder
        public ClipboardFolder GetFolder(string collectionName);
        public ClipboardFolder GetRootFolder();
        public ClipboardFolder GetSearchRootFolder();
        public void UpsertFolderRelation(ClipboardFolder parent, ClipboardFolder child);

        public void DeleteFolder(ClipboardFolder folder);
        public void UpsertFolder(ClipboardFolder folder);

        public IEnumerable<string> GetFolderRelations(string parentCollectionName);


        // public void DeleteItems(List<ClipboardItem> items);

        // -- SearchRule
        public SearchRule? GetSearchRuleByFolderName(string collectionName);
        public SearchRule? GetSearchRule(string name);

        public void UpsertSearchRule(SearchRule conditionRule);

        // -- AutoProcessRule
        public List<AutoProcessRule> GetAutoProcessRules(ClipboardFolder targetFolder);

        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules();

        public void UpsertAutoProcessRule(AutoProcessRule rule);

        public void DeleteAutoProcessRule(AutoProcessRule rule);

        public IEnumerable<AutoProcessRule> GetCopyToMoveToRules();

        //-- Tag 要改修
        public IEnumerable<TagItem> GetTagList();

        public void DeleteTag(TagItem tag);

        public void UpsertTag(TagItem tag);


        public IEnumerable<TagItem> FilterTag(string tag, bool exclude);

        // --- Python Script
        public IEnumerable<ScriptItem> GetScriptItems();

        public void UpsertPromptTemplate(PromptItem promptItem);

        public ICollection<PromptItem> GetAllPromptTemplates();

        public void DeletePromptTemplate(PromptItem promptItem);

        //----  RAGSourceItem
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item);
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item);
        // get
        public IEnumerable<RAGSourceItem> GetRAGSourceItems();

        //--- -  VectorDBItem
        // update
        public void UpsertVectorDBItem(VectorDBItem item);
        // delete
        public void DeleteVectorDBItem(VectorDBItem item);
        // get
        public IEnumerable<VectorDBItem> GetVectorDBItems();

    }

}

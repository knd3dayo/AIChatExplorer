using LiteDB;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;

namespace WpfAppCommon.Factory
{
    public interface IClipboardDBController {


        //-- ClipboardItem
        public ClipboardItem? GetItem(ObjectId id);

        public void UpsertItem(ClipboardItem item, bool contentIsModified = true);
        public void DeleteItem(ClipboardItem item);
        public IEnumerable<ClipboardItem> SearchItems(ClipboardFolder folder, SearchCondition searchCondition);
        public IEnumerable<ClipboardItem> GetItems(ClipboardFolder folder);

        //-- ClipboardItemFiles
        public void UpsertItemFile(ClipboardItemFile item);
        public void DeleteItemFile(ClipboardItemFile item);
        public ClipboardItemFile? GetItemFile(ObjectId id);

        //-- ClipboardFolder
        
        public ClipboardFolder? GetFolder(ObjectId? objectId);
        public List<ClipboardFolder> GetFoldersByParentId(ObjectId? objectId);

        
        public ClipboardFolder? GetRootFolderByType(ClipboardFolder.FolderTypeEnum folderType);

        public void DeleteFolder(ClipboardFolder folder);
        public void UpsertFolder(ClipboardFolder folder);

        
        // public void DeleteItems(List<ClipboardItem> items);

        // -- SearchRule
        public SearchRule? GetSearchRuleByFolder(ClipboardFolder folder);
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

        public PromptItem GetPromptTemplate(ObjectId id);


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
        public void UpsertVectorDBItem(VectorDBItemBase item);
        // delete
        public void DeleteVectorDBItem(VectorDBItemBase item);
        // get
        public IEnumerable<VectorDBItemBase> GetVectorDBItems();

    }

}

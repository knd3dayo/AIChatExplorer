using ClipboardApp.Model;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using LiteDB;
using PythonAILib.Model.Abstract;

namespace ClipboardApp.Factory {
    public interface IClipboardDBController : IDataFactory {

        public IEnumerable<ClipboardItem> SearchItems(ClipboardFolder folder, SearchCondition searchCondition);
        public IEnumerable<ClipboardItem> GetItems(ClipboardFolder folder);

        //-- ClipboardFolder

        public List<ClipboardFolder> GetFoldersByParentId(ObjectId? objectId);
        public ClipboardFolder? GetRootFolderByType(ClipboardFolder.FolderTypeEnum folderType);

        // public void DeleteItems(List<ContentItem> items);

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

    }

}

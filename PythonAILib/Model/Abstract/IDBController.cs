namespace PythonAILib.Model.Abstract
{
    public interface IDBController {

        //-- ClipboardItem
        public ContentItemBase? GetItem(ContentItemBase item);

        public void UpsertItem(ContentItemBase item, bool contentIsModified = true);
        public void DeleteItem(ContentItemBase item);

        // Prompt
        // create
        public PromptItemBase CreatePromptItem();

        public ICollection<PromptItemBase> GetAllPromptTemplates();

        //----  RAGSourceItem
        public RAGSourceItemBase CreateRAGSourceItem();

        public IEnumerable<RAGSourceItemBase> GetRAGSourceItems();

        // -- VectorDBItem
        public VectorDBItemBase CreateVectorDBItem();

        public IEnumerable<VectorDBItemBase> GetVectorDBItems(bool isSystemVectorDB);
    }
}

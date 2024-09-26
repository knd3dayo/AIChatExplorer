using LiteDB;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Abstract
{
    public interface IDataFactory {

        // Database
        public LiteDatabase GetDatabase();

        //-- ContentItemBase
        public ContentItemBase? GetItem(ContentItemBase item);

        public void UpsertItem(ContentItemBase item, bool contentIsModified = true);
        public void DeleteItem(ContentItemBase item);

        //-- AttachedItems
        public void UpsertAttachedItem(ContentAttachedItem item);
        public void DeleteAttachedItem(ContentAttachedItem item);
        public ContentAttachedItem? GetAttachedItem(ObjectId id);


        // Prompt
        // create
        public PromptItem CreatePromptItem();

        public ICollection<PromptItem> GetAllPromptTemplates();

        public void UpsertPromptTemplate(PromptItem promptItem);

        public PromptItem GetPromptTemplate(ObjectId id);

        public PromptItem? GetPromptTemplateByName(string name);

        public PromptItem? GetSystemPromptTemplateByName(string name);

        public void DeletePromptTemplate(PromptItem promptItem);

        //----  RAGSourceItem
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item);
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item);

        //--- -  VectorDBItem
        // update
        public void UpsertVectorDBItem(VectorDBItem item);
        // delete
        public void DeleteVectorDBItem(VectorDBItem item);
        // get
        public IEnumerable<VectorDBItem> GetVectorDBItems();

        public VectorDBItem GetSystemVectorDBItem(); 


        //----  RAGSourceItem
        public RAGSourceItem CreateRAGSourceItem();

        public IEnumerable<RAGSourceItem> GetRAGSourceItems();

        // -- VectorDBItem
        public VectorDBItem CreateVectorDBItem();

    }
}

using LiteDB;
using PythonAILib.Model.Content;

namespace PythonAILib.Model.AutoProcess {
    // 自動処理の引数用のクラス
    public class AutoProcessItem {

        public enum TypeEnum {
            Ignore,
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MergeAllItems,
            MergeItemsWithSameSourceApplicationTitle,
            PromptTemplate,
        }
        public ObjectId? Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public TypeEnum TypeName { get; set; } = TypeEnum.CopyToFolder;
        public ObjectId DestinationFolderId { get; set; } = ObjectId.Empty;


    }
    }

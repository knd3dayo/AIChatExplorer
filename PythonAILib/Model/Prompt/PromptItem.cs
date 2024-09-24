using LiteDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Prompt {

    public class PromptItem {

        public enum PromptTemplateTypeEnum {
            // ユーザー定義
            UserDefined,
            // システム定義
            SystemDefined,
            // 変更を加えたシステム定義
            ModifiedSystemDefined
        }

        public ObjectId Id { get; set; } = ObjectId.Empty;
        // 名前

        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;

        // Save
        public void Save() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

            libManager.DataFactory.UpsertPromptTemplate(this);
        }

    }
}

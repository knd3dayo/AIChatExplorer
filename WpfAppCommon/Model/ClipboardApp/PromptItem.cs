using LiteDB;
using PythonAILib.Model;
using WpfAppCommon.Factory;
using WpfAppCommon.Model.QAChat;

namespace WpfAppCommon.Model.ClipboardApp {
    public class PromptItem : PromptItemBase {
        // システム定義のプロンプト名
        public enum SystemDefinedPromptNames {
            // タイトル生成
            TitleGeneration,
            // 背景情報生成
            BackgroundInformationGeneration,
            // サマリー生成
            SummaryGeneration,
        }

        public ObjectId Id { get; set; } = ObjectId.Empty;
        // 名前

        // Save
        public override void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertPromptTemplate(this);
        }

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplate(id);
        }
        // 名前を指定してPromptItemを取得
        public static PromptItem? GetPromptItemByName(string name) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplateByName(name);
        }
        // 名前を指定してシステム定義のPromptItemを取得
        public static PromptItem GetSystemPromptItemByName(SystemDefinedPromptNames name) {
            
            var item =  ClipboardAppFactory.Instance.GetClipboardDBController().GetSystemPromptTemplateByName(name.ToString());
            if (item == null) {
                throw new System.Exception("PromptItem not found");
            }
            return item;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();

            // TitleGenerationをDBから取得
            PromptItem? titleGeneration = clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.TitleGeneration.ToString());

            if (titleGeneration == null) {
                titleGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TitleGeneration.ToString(),
                    Description = PromptStringResource.Instance.TitleGeneration,
                    Prompt = PromptStringResource.Instance.TitleGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(titleGeneration);
            }
            // BackgroundInformationGenerationをDBから取得
            PromptItem? backgroundInformationGeneration = clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    Description = PromptStringResource.Instance.BackgroundInformationGeneration,
                    Prompt = PromptStringResource.Instance.BackgroundInformationGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(backgroundInformationGeneration);
            }
            // SummaryGenerationをDBから取得
            PromptItem? summaryGeneration = clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.SummaryGeneration.ToString());
            if (summaryGeneration == null) {
                summaryGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.SummaryGeneration.ToString(),
                    Description = PromptStringResource.Instance.SummaryGenerationPrompt,
                    Prompt = PromptStringResource.Instance.SummaryGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(summaryGeneration);
            }
        }

    }
}

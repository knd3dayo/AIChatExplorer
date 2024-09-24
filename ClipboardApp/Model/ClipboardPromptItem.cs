using ClipboardApp.Factory;
using LiteDB;
using PythonAILib.Model.Prompt;
using PythonAILib.Resource;
using QAChat;

namespace ClipboardApp.Model
{
    public class ClipboardPromptItem : PromptItem {
        // システム定義のプロンプト名
        public enum SystemDefinedPromptNames {
            // タイトル生成
            TitleGeneration,
            // 背景情報生成
            BackgroundInformationGeneration,
            // サマリー生成
            SummaryGeneration,
            // 課題リスト生成
            IssuesGeneration
        }


        // PromptItemを取得
        public static ClipboardPromptItem GetPromptItemById(ObjectId id) {
            return (ClipboardPromptItem)ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplate(id);
        }
        // 名前を指定してPromptItemを取得
        public static ClipboardPromptItem? GetPromptItemByName(string name) {
            return (ClipboardPromptItem?)ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplateByName(name);
        }
        // 名前を指定してシステム定義のPromptItemを取得
        public static ClipboardPromptItem GetSystemPromptItemByName(SystemDefinedPromptNames name) {

            var item = (ClipboardPromptItem?)ClipboardAppFactory.Instance.GetClipboardDBController().GetSystemPromptTemplateByName(name.ToString());
            if (item == null) {
                throw new System.Exception("PromptItem not found");
            }
            return item;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();

            // TitleGenerationをDBから取得
            ClipboardPromptItem? titleGeneration = (ClipboardPromptItem?)clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.TitleGeneration.ToString());

            if (titleGeneration == null) {
                titleGeneration = new ClipboardPromptItem() {
                    Name = SystemDefinedPromptNames.TitleGeneration.ToString(),
                    Description = PromptStringResource.Instance.TitleGeneration,
                    Prompt = PromptStringResource.Instance.TitleGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(titleGeneration);
            }
            // BackgroundInformationGenerationをDBから取得
            ClipboardPromptItem? backgroundInformationGeneration = (ClipboardPromptItem?)clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new ClipboardPromptItem() {
                    Name = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    Description = PromptStringResource.Instance.BackgroundInformationGeneration,
                    Prompt = PromptStringResource.Instance.BackgroundInformationGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(backgroundInformationGeneration);
            }
            // SummaryGenerationをDBから取得
            ClipboardPromptItem? summaryGeneration = (ClipboardPromptItem?)clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.SummaryGeneration.ToString());
            if (summaryGeneration == null) {
                summaryGeneration = new ClipboardPromptItem() {
                    Name = SystemDefinedPromptNames.SummaryGeneration.ToString(),
                    Description = PromptStringResource.Instance.SummaryGenerationPrompt,
                    Prompt = PromptStringResource.Instance.SummaryGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(summaryGeneration);
            }
            // IssuesGenerationをDBから取得
            ClipboardPromptItem? issuesGeneration = (ClipboardPromptItem?)clipboardDBController.GetSystemPromptTemplateByName(SystemDefinedPromptNames.IssuesGeneration.ToString());
            if (issuesGeneration == null) {
                issuesGeneration = new ClipboardPromptItem() {
                    Name = SystemDefinedPromptNames.IssuesGeneration.ToString(),
                    Description = PromptStringResource.Instance.IssuesGeneration,
                    Prompt = PromptStringResource.Instance.IssuesGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                clipboardDBController.UpsertPromptTemplate(issuesGeneration);
            }
        }

    }
}

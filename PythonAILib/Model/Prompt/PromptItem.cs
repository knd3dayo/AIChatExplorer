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
        public enum SystemDefinedPromptNames {
            // タイトル生成
            TitleGeneration,
            // 背景情報生成
            BackgroundInformationGeneration,
            // サマリー生成
            SummaryGeneration,
            // 課題リスト生成
            IssuesGeneration,
            // 文脈情報生成
            ContextInformationGeneration,
        }
        public enum PromptResultTypeEnum {
            // テキスト
            Text,
            // リスト
            List,
        }
        public enum ChatTypeEnum {
            // Normal
            Normal,
            // RAG,
            RAG,
            // Langchain
            Langchain,
        }


        public ObjectId Id { get; set; } = ObjectId.Empty;

        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;

        // プロンプト結果の種類
        public PromptResultTypeEnum PromptResultType { get; set; } = PromptResultTypeEnum.Text;

        // チャットタイプ
        public ChatTypeEnum ChatType { get; set; } = ChatTypeEnum.Normal;

        // Save
        public void Save() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

            libManager.DataFactory.UpsertPromptTemplate(this);
        }

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetPromptTemplate(id);
        }
        // 名前を指定してPromptItemを取得
        public static PromptItem? GetPromptItemByName(string name) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetPromptTemplateByName(name);
        }
        // 名前を指定してシステム定義のPromptItemを取得
        public static PromptItem GetSystemPromptItemByName(SystemDefinedPromptNames name) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

            var item = libManager.DataFactory.GetSystemPromptTemplateByName(name.ToString());
            if (item == null) {
                throw new System.Exception("PromptItem not found");
            }
            return item;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            // TitleGenerationをDBから取得
            PromptItem? titleGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.TitleGeneration.ToString());

            if (titleGeneration == null) {
                titleGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TitleGeneration.ToString(),
                    Description = PromptStringResource.Instance.TitleGeneration,
                    Prompt = PromptStringResource.Instance.TitleGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                libManager.DataFactory.UpsertPromptTemplate(titleGeneration);
            }
            // BackgroundInformationGenerationをDBから取得
            PromptItem? backgroundInformationGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    Description = PromptStringResource.Instance.BackgroundInformationGeneration,
                    Prompt = PromptStringResource.Instance.BackgroundInformationGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                libManager.DataFactory.UpsertPromptTemplate(backgroundInformationGeneration);
            }
            // SummaryGenerationをDBから取得
            PromptItem? summaryGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.SummaryGeneration.ToString());
            if (summaryGeneration == null) {
                summaryGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.SummaryGeneration.ToString(),
                    Description = PromptStringResource.Instance.SummaryGenerationPrompt,
                    Prompt = PromptStringResource.Instance.SummaryGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                libManager.DataFactory.UpsertPromptTemplate(summaryGeneration);
            }
            // IssuesGenerationをDBから取得
            PromptItem? issuesGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.IssuesGeneration.ToString());
            if (issuesGeneration == null) {
                issuesGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.IssuesGeneration.ToString(),
                    Description = PromptStringResource.Instance.IssuesGeneration,
                    Prompt = PromptStringResource.Instance.IssuesGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined
                };
                libManager.DataFactory.UpsertPromptTemplate(issuesGeneration);
            }


        }
    }
}

using LiteDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Prompt {
    public class PromptItem {
        public enum PromptTemplateTypeEnum {
            // ユーザー定義
            UserDefined = 0,
            // システム定義
            SystemDefined = 1,
            // 変更を加えたシステム定義
            ModifiedSystemDefined = 2
        }
        public enum SystemDefinedPromptNames {
            // タイトル生成
            TitleGeneration = 0,
            // 背景情報生成
            BackgroundInformationGeneration = 1,
            // サマリー生成
            SummaryGeneration = 2,
            // 課題リスト生成
            TasksGeneration = 3,
        }
        public enum PromptResultTypeEnum {
            // テキスト
            TextContent = 0,
            // リスト
            ListContent = 1,
            // 複雑なテキスト
            ComplexContent = 2,
        }
        public enum PromptOutputTypeEnum {
            // 新規作成
            NewContent = 0,
            // 本文を上書き
            OverwriteContent = 1,
            // タイトルを上書き
            OverwriteTitle = 2,
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
        public PromptResultTypeEnum PromptResultType { get; set; } = PromptResultTypeEnum.TextContent;

        // チャットタイプ
        public OpenAIExecutionModeEnum ChatType { get; set; } = OpenAIExecutionModeEnum.Normal;

        // プロンプトの出力タイプ
        public PromptOutputTypeEnum PromptOutputType { get; set; } = PromptOutputTypeEnum.NewContent;

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
        // List<PromptItem>を取得
        public static List<PromptItem> GetPromptItems() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            List<PromptItem> promptItems = [];
            foreach (var item in libManager.DataFactory.GetAllPromptTemplates()) {
                promptItems.Add(item);
            }
            return promptItems;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            // TitleGenerationをDBから取得
            PromptItem? titleGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.TitleGeneration.ToString());

            if (titleGeneration != null) {
                libManager.DataFactory.DeletePromptTemplate(titleGeneration);
                titleGeneration = null;
            }

            if (titleGeneration == null) {
                titleGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TitleGeneration.ToString(),
                    Description = PromptStringResource.Instance.TitleGeneration,
                    Prompt = PromptStringResource.Instance.TitleGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatType = OpenAIExecutionModeEnum.Normal,
                    PromptOutputType = PromptOutputTypeEnum.OverwriteTitle

                };
                libManager.DataFactory.UpsertPromptTemplate(titleGeneration);
            }
            // BackgroundInformationGenerationをDBから取得
            PromptItem? backgroundInformationGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());

            if (backgroundInformationGeneration != null) {
                libManager.DataFactory.DeletePromptTemplate(backgroundInformationGeneration);
                backgroundInformationGeneration = null;
            }

            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    Description = PromptStringResource.Instance.BackgroundInformationGeneration,
                    Prompt = PromptStringResource.Instance.BackgroundInformationGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatType = OpenAIExecutionModeEnum.OpenAIRAG,
                    PromptOutputType = PromptOutputTypeEnum.NewContent
                };
                libManager.DataFactory.UpsertPromptTemplate(backgroundInformationGeneration);
            }
            // SummaryGenerationをDBから取得
            PromptItem? summaryGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.SummaryGeneration.ToString());

            if (summaryGeneration != null) {
                libManager.DataFactory.DeletePromptTemplate(summaryGeneration);
                summaryGeneration = null;
            }

            if (summaryGeneration == null) {
                summaryGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.SummaryGeneration.ToString(),
                    Description = PromptStringResource.Instance.SummaryGeneration,
                    Prompt = PromptStringResource.Instance.SummaryGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatType = OpenAIExecutionModeEnum.Normal,
                    PromptOutputType = PromptOutputTypeEnum.NewContent

                };
                libManager.DataFactory.UpsertPromptTemplate(summaryGeneration);
            }
            // TasksGenerationをDBから取得
            PromptItem? TasksGeneration = libManager.DataFactory.GetSystemPromptTemplateByName(SystemDefinedPromptNames.TasksGeneration.ToString());

            if (TasksGeneration != null) {
                libManager.DataFactory.DeletePromptTemplate(TasksGeneration);
                TasksGeneration = null;
            }

            if (TasksGeneration == null) {
                TasksGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TasksGeneration.ToString(),
                    Description = PromptStringResource.Instance.TasksGeneration,
                    Prompt = PromptStringResource.Instance.TasksGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.ComplexContent,
                    ChatType = OpenAIExecutionModeEnum.OpenAIRAG,
                    PromptOutputType = PromptOutputTypeEnum.NewContent

                };
                libManager.DataFactory.UpsertPromptTemplate(TasksGeneration);
            }


        }
    }
}

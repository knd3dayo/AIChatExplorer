using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Resource;

namespace PythonAILib.Model.Prompt {
    public partial class PromptItem {

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
        public OpenAIExecutionModeEnum ChatMode { get; set; } = OpenAIExecutionModeEnum.Normal;

        // 分割モード
        public SplitOnTokenLimitExceedModeEnum SplitMode { get; set; } = SplitOnTokenLimitExceedModeEnum.None;

        // ベクトルDBを使用する
        public bool UseVectorDB { get; set; } = false;

        // プロンプトの出力タイプ
        public PromptOutputTypeEnum PromptOutputType { get; set; } = PromptOutputTypeEnum.NewContent;

        // PromptInputName
        public string PromptInputName { get; set; } = string.Empty;

        // Save
        public void Save() {

            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
            collection.Upsert(this);
        }
        // Delete
        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
            collection.Delete(Id);
        }

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            return libManager.DataFactory.GetPromptCollection<PromptItem>().FindById(id);
        }
        // 名前を指定してPromptItemを取得
        public static PromptItem? GetPromptItemByName(string name) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            return libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name);
        }
        // 名前を指定してシステム定義のPromptItemを取得
        public static PromptItem GetSystemPromptItemByName(SystemDefinedPromptNames name) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            var item = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(
                x => x.Name == name.ToString() && (
                x.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined ||
                x.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined)
                ) ?? throw new System.Exception("PromptItem not found");
            return item;
        }
        // List<PromptItem>を取得
        public static List<PromptItem> GetPromptItems() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            List<PromptItem> promptItems = [];
            foreach (var item in libManager.DataFactory.GetPromptCollection<PromptItem>().FindAll()) {
                promptItems.Add(item);
            }
            return promptItems;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            PythonAILibManager libManager = PythonAILibManager.Instance;
            // TitleGenerationをDBから取得
            string name1 = SystemDefinedPromptNames.TitleGeneration.ToString();
            PromptItem? titleGeneration = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name1);

            if (titleGeneration != null) {
                libManager.DataFactory.GetPromptCollection<PromptItem>().Delete(titleGeneration.Id);
                titleGeneration = null;
            }

            if (titleGeneration == null) {
                titleGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TitleGeneration.ToString(),
                    Description = PromptStringResource.Instance.TitleGeneration,
                    Prompt = PromptStringResource.Instance.TitleGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBは使用しない
                    UseVectorDB = false,
                    PromptOutputType = PromptOutputTypeEnum.OverwriteTitle

                };
                var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
                collection.Upsert(titleGeneration);
            }
            // BackgroundInformationGenerationをDBから取得
            string name2 = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            PromptItem? backgroundInformationGeneration = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name2);

            if (backgroundInformationGeneration != null) {
                libManager.DataFactory.GetPromptCollection<PromptItem>().Delete(backgroundInformationGeneration.Id);
                backgroundInformationGeneration = null;
            }

            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(),
                    Description = PromptStringResource.Instance.BackgroundInformationGeneration,
                    Prompt = PromptStringResource.Instance.BackgroundInformationGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBを使用する
                    UseVectorDB = true,
                    PromptOutputType = PromptOutputTypeEnum.NewContent
                };
                var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
                collection.Upsert(backgroundInformationGeneration);

            }
            // SummaryGenerationをDBから取得
            string name3 = SystemDefinedPromptNames.SummaryGeneration.ToString();
            PromptItem? summaryGeneration = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name3);

            if (summaryGeneration != null) {
                libManager.DataFactory.GetPromptCollection<PromptItem>().Delete(summaryGeneration.Id);
                summaryGeneration = null;
            }

            if (summaryGeneration == null) {
                summaryGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.SummaryGeneration.ToString(),
                    Description = PromptStringResource.Instance.SummaryGeneration,
                    Prompt = PromptStringResource.Instance.SummaryGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBを使用しない
                    UseVectorDB = false,
                    PromptOutputType = PromptOutputTypeEnum.NewContent

                };
                var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
                collection.Upsert(summaryGeneration);
            }
            // TasksGenerationをDBから取得
            string name4 = SystemDefinedPromptNames.TasksGeneration.ToString();
            PromptItem? TasksGeneration = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name4);

            if (TasksGeneration != null) {
                libManager.DataFactory.GetPromptCollection<PromptItem>().Delete(TasksGeneration.Id);
                TasksGeneration = null;
            }

            if (TasksGeneration == null) {
                TasksGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TasksGeneration.ToString(),
                    Description = PromptStringResource.Instance.TasksGeneration,
                    Prompt = PromptStringResource.Instance.TasksGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TableContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBを使用する
                    UseVectorDB = true,
                    PromptOutputType = PromptOutputTypeEnum.NewContent

                };
                var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
                collection.Upsert(TasksGeneration);
            }

            // DocumentReliabilityCheckをDBから取得
            string name5 = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString();
            PromptItem? DocumentReliabilityCheck = libManager.DataFactory.GetPromptCollection<PromptItem>().FindOne(x => x.Name == name5);
            if (DocumentReliabilityCheck != null) {
                libManager.DataFactory.GetPromptCollection<PromptItem>().Delete(DocumentReliabilityCheck.Id);
                DocumentReliabilityCheck = null;
            }
            if (DocumentReliabilityCheck == null) {
                DocumentReliabilityCheck = new PromptItem() {
                    Name = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString(),
                    Description = PromptStringResource.Instance.DocumentReliability,
                    Prompt = PromptStringResource.Instance.DocumentReliabilityCheckPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.TextContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBを使用しない
                    UseVectorDB = false,
                    PromptOutputType = PromptOutputTypeEnum.NewContent,

                };
                var collection = libManager.DataFactory.GetPromptCollection<PromptItem>();
                collection.Upsert(DocumentReliabilityCheck);
            }
        }

        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            PromptItem item = (PromptItem)obj;
            return Id == item.Id;
        }

        // GetHashCode
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}

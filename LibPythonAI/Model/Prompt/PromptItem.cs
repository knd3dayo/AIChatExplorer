using LibPythonAI.Data;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;
using PythonAILib.Resources;

namespace LibPythonAI.Model.Prompt {
    public partial class PromptItem {

        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";


        // コンストラクタ
        public PromptItem(PromptItemEntity entity) {
            Entity = entity;
        }

        public PromptItemEntity Entity { get; set; }

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
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.Find(Entity.Id);
            if (item != null) {
                db.PromptItems.Update(Entity);
            } else {
                db.PromptItems.Add(Entity);
            }
            db.SaveChanges();
        }

        // Delete
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.Find(Entity.Id);
            if (item != null) {
                db.PromptItems.Remove(item);
                db.SaveChanges();
            }
        }

        // PromptItemを取得
        public static PromptItem? GetPromptItemById(string id) {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.Find(id);
            if (item == null) {
                return null;
            }
            return new PromptItem(item);

        }
        // 名前を指定してPromptItemを取得
        public static PromptItem? GetPromptItemByName(string name) {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.FirstOrDefault(x => x.Name == name);
            if (item == null) {
                return null;
            }
            return new PromptItem(item);
        }

        // 名前を指定してシステム定義のPromptItemを取得
        public static PromptItem GetSystemPromptItemByName(SystemDefinedPromptNames name) {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.FirstOrDefault(x => x.Name == name.ToString() && (
                x.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined ||
                x.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined)
                ) ?? throw new Exception("PromptItem not found");

            return new PromptItem(item);
        }
        // List<PromptItem>を取得
        public static List<PromptItem> GetPromptItems() {
            using PythonAILibDBContext db = new();
            List<PromptItem> promptItems = [];
            foreach (var item in db.PromptItems) {
                promptItems.Add(new PromptItem(item));
            }
            return promptItems;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            using PythonAILibDBContext db = new();

            // TitleGenerationをDBから取得
            string name1 = SystemDefinedPromptNames.TitleGeneration.ToString();
            var titleGeneration = db.PromptItems.FirstOrDefault(x => x.Name == name1);

            if (titleGeneration == null) {
                titleGeneration = new PromptItemEntity() {
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
                db.PromptItems.Add(titleGeneration);
                db.SaveChanges();
            }
            // BackgroundInformationGenerationをDBから取得
            string name2 = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            var backgroundInformationGeneration = db.PromptItems.FirstOrDefault(x => x.Name == name2);

            if (backgroundInformationGeneration == null) {
                backgroundInformationGeneration = new PromptItemEntity() {
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
                db.PromptItems.Add(backgroundInformationGeneration);
                db.SaveChanges();

            }
            // SummaryGenerationをDBから取得
            string name3 = SystemDefinedPromptNames.SummaryGeneration.ToString();
            var summaryGeneration = db.PromptItems.FirstOrDefault(x => x.Name == name3);


            if (summaryGeneration == null) {
                summaryGeneration = new PromptItemEntity() {
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
                db.PromptItems.Add(summaryGeneration);
                db.SaveChanges();
            }
            // TasksGenerationをDBから取得
            string name4 = SystemDefinedPromptNames.TasksGeneration.ToString();
            var tasksGeneration = db.PromptItems.FirstOrDefault(x => x.Name == name4);

            if (tasksGeneration == null) {
                tasksGeneration = new PromptItemEntity() {
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
                db.PromptItems.Add(tasksGeneration);
                db.SaveChanges();
            }

            // DocumentReliabilityCheckをDBから取得
            string name5 = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString();
            var documentReliabilityCheck = db.PromptItems.FirstOrDefault(x => x.Name == name5);

            if (documentReliabilityCheck == null) {
                documentReliabilityCheck = new PromptItemEntity() {
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
                db.PromptItems.Add(documentReliabilityCheck);
                db.SaveChanges();
            }
        }

        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            PromptItem item = (PromptItem)obj;
            return Entity == item.Entity;
        }

        // GetHashCode
        public override int GetHashCode() {
            return Entity.GetHashCode();
        }
    }
}

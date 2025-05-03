using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;
using PythonAILib.Resources;

namespace LibPythonAI.Model.Prompt {
    public partial class PromptItem {

        public PromptItemEntity Entity { get; private set; } = new();

        // ID
        public string Id {
            get => Entity.Id;
        }

        // 名前
        public string Name {
            get => Entity.Name;
            set => Entity.Name = value;
        }
        // 説明
        public string Description {
            get => Entity.Description;
            set => Entity.Description = value;
        }

        // プロンプト
        public string Prompt {
            get => Entity.Prompt;
            set => Entity.Prompt = value;
        }


        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType {
            get => Entity.PromptTemplateType;
            set => Entity.PromptTemplateType = value;
        }

        // プロンプト結果の種類
        public PromptResultTypeEnum PromptResultType {
            get {
                Entity.ExtendedProperties.TryGetValue("PromptResultType", out object? value);
                if (value is int intValue) {
                    return (PromptResultTypeEnum)intValue;
                }
                return PromptResultTypeEnum.TextContent;
            }
            set {
                Entity.ExtendedProperties["PromptResultType"] = (int)value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // チャットタイプ
        public OpenAIExecutionModeEnum ChatMode {
            get {
                Entity.ExtendedProperties.TryGetValue("ChatMode", out object? value);
                if (value is int intValue) {
                    return (OpenAIExecutionModeEnum)intValue;
                }
                return OpenAIExecutionModeEnum.Normal;
            }
            set {
                Entity.ExtendedProperties["ChatMode"] = (int)value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // 分割モード
        public SplitOnTokenLimitExceedModeEnum SplitMode {
            get {
                Entity.ExtendedProperties.TryGetValue("SplitMode", out object? value);
                if (value is int intValue) {
                    return (SplitOnTokenLimitExceedModeEnum)intValue;
                }
                return SplitOnTokenLimitExceedModeEnum.None;
            }
            set {
                Entity.ExtendedProperties["SplitMode"] = (int)value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // ベクトルDBを使用する
        public bool UseVectorDB {
            get {
                Entity.ExtendedProperties.TryGetValue("UseVectorDB", out object? value);
                if (value is bool boolValue) {
                    return boolValue;
                }
                return false;
            }
            set {
                Entity.ExtendedProperties["UseVectorDB"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // タグ一覧を参照する
        public bool UseTagList {
            get {
                Entity.ExtendedProperties.TryGetValue("UseTagList", out object? value);
                if (value is bool boolValue) {
                    return boolValue;
                }
                return false;
            }
            set {
                Entity.ExtendedProperties["UseTagList"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // プロンプトの出力タイプ
        public PromptOutputTypeEnum PromptOutputType {
            get {
                Entity.ExtendedProperties.TryGetValue("PromptOutputType", out object? value);
                if (value is int intValue) {
                    return (PromptOutputTypeEnum)intValue;
                }
                return PromptOutputTypeEnum.NewContent;
            }
            set {
                Entity.ExtendedProperties["PromptOutputType"] = (int)value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // PromptInputName
        public string PromptInputName {
            get {
                Entity.ExtendedProperties.TryGetValue("PromptInputName", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["PromptInputName"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // Save
        public void Save() {
            PromptItemEntity.SaveItems([this.Entity]);
        }


        // DeleteAsync
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.Find(Id);
            if (item != null) {
                db.PromptItems.Remove(item);
                db.SaveChanges();
            }
        }
        // タグ一覧を参照するプロンプトを生成する.
        public static async Task<string> GenerateTagListPrompt() {
            // タグ一覧を取得
            List<TagItem> tagItems = await TagItem.GetTagItemsAsync();
            // タグ一覧をカンマ区切りで取得
            string tagList = string.Join(", ", tagItems.Select(x => x.Tag));

            string prompt = PromptStringResource.Instance.TagListPrompt(tagList);
            return prompt;
        }

        // PromptItemを取得
        public static PromptItem? GetPromptItemById(string id) {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.Find(id);
            if (item == null) {
                return null;
            }
            return new PromptItem() { Entity = item};

        }
        // 名前を指定してPromptItemを取得
        public static PromptItem? GetPromptItemByName(string name) {
            using PythonAILibDBContext db = new();
            var item = db.PromptItems.FirstOrDefault(x => x.Name == name);
            if (item == null) {
                return null;
            }
            return new PromptItem() { Entity = item };
        }

        // 名前を指定してシステム定義のPromptItemを取得
        public static List<PromptItem> GetSystemPromptItems() {
            using PythonAILibDBContext db = new();
            var items = db.PromptItems.Where(x => ( x.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined || x.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined));
            List<PromptItem> promptItems = [];
            foreach (var item in items) {
                promptItems.Add(new PromptItem() { Entity = item });
            }
            return promptItems;
        }
        // List<PromptItem>を取得
        public static List<PromptItem> GetPromptItems() {
            using PythonAILibDBContext db = new();
            List<PromptItem> promptItems = [];
            foreach (var item in db.PromptItems) {
                promptItems.Add(new PromptItem() { Entity = item });
            }
            return promptItems;
        }

        // システム定義のPromptItemを取得
        public static void InitSystemPromptItems() {

            using PythonAILibDBContext db = new();

            // TitleGenerationをDBから取得
            string name1 = SystemDefinedPromptNames.TitleGeneration.ToString();
            var titleGeneration = GetPromptItemByName(name1);

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
                titleGeneration.Save();
            }
            // BackgroundInformationGenerationをDBから取得
            string name2 = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            var backgroundInformationGeneration = GetPromptItemByName(name1);
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
                backgroundInformationGeneration.Save();

            }
            // SummaryGenerationをDBから取得
            string name3 = SystemDefinedPromptNames.SummaryGeneration.ToString();
            var summaryGeneration = GetPromptItemByName(name3);

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
                summaryGeneration.Save();
            }
            // TasksGenerationをDBから取得
            string name4 = SystemDefinedPromptNames.TasksGeneration.ToString();
            var tasksGeneration = GetPromptItemByName(name4);

            if (tasksGeneration == null) {
                tasksGeneration = new PromptItem() {
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
                tasksGeneration.Save();
            }

            // DocumentReliabilityCheckをDBから取得
            string name5 = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString();
            var documentReliabilityCheck = GetPromptItemByName(name5);

            if (documentReliabilityCheck == null) {
                documentReliabilityCheck = new PromptItem() {
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
                documentReliabilityCheck.Save();
            }

            // TagGenerationをDBから取得
            string name6 = SystemDefinedPromptNames.TagGeneration.ToString();
            var tagGeneration = GetPromptItemByName(name6);
            if (tagGeneration == null) {
                tagGeneration = new PromptItem() {
                    Name = SystemDefinedPromptNames.TagGeneration.ToString(),
                    Description = PromptStringResource.Instance.TagGeneration,
                    Prompt = PromptStringResource.Instance.TagGenerationPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.ListContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    // ベクトルDBを使用しない
                    UseVectorDB = false,
                    PromptOutputType = PromptOutputTypeEnum.AppendTags,
                };
                tagGeneration.Save();
            }
            // SelectExistingTagsをDBから取得
            string name7 = SystemDefinedPromptNames.SelectExistingTags.ToString();
            var selectExistedTags = GetPromptItemByName(name7);
            if (selectExistedTags == null) {
                selectExistedTags = new PromptItem() {
                    Name = SystemDefinedPromptNames.SelectExistingTags.ToString(),
                    Description = PromptStringResource.Instance.SelectExistingTags,
                    Prompt = PromptStringResource.Instance.SelectExistingTagsPrompt,
                    PromptTemplateType = PromptTemplateTypeEnum.SystemDefined,
                    PromptResultType = PromptResultTypeEnum.ListContent,
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                    UseTagList = true,
                    // ベクトルDBを使用しない
                    UseVectorDB = false,
                    PromptOutputType = PromptOutputTypeEnum.AppendTags,
                };
                selectExistedTags.Save();
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

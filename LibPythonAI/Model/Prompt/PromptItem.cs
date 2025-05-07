using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
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
                // valueのtypeを確認
                LogWrapper.Debug($"PromptResultType: {value?.GetType()}");
                if (value is Decimal intValue) {
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
                if (value is Decimal intValue) {
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
                if (value is Decimal intValue) {
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
                if (value is Decimal intValue) {
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
            return new PromptItem() { Entity = item };

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

        // システム定義のPromptItemを取得
        public static List<PromptItem> GetSystemPromptItems() {
            using PythonAILibDBContext db = new();
            var items = db.PromptItems.Where(x => (x.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined || x.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined));
            List<PromptItem> promptItems = [];
            foreach (var item in items) {
                promptItems.Add(new PromptItem() { Entity = item });
            }
            return promptItems;
        }

        // システム定義以外のPromptItemを取得
        public static List<PromptItem> GetUserDefinedPromptItems() {
            using PythonAILibDBContext db = new();
            var items = db.PromptItems.Where(x => x.PromptTemplateType == PromptTemplateTypeEnum.UserDefined);
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
                    UseTagList = true,
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

        public static void ExecutePromptTemplate(List<ContentItemWrapper> items, PromptItem promptItem, Action beforeAction, Action afterAction) {

            // promptNameからDescriptionを取得
            string description = promptItem.Description;

            LogWrapper.Info(PythonAILibStringResources.Instance.PromptTemplateExecute(description));
            int count = items.Count;
            Task.Run(() => {
                beforeAction();
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new() {
                    MaxDegreeOfParallelism = 4
                };
                Parallel.For(0, count, parallelOptions, async (i) => {
                    lock (lockObject) {
                        start_count++;
                    }
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    string message = $"{PythonAILibStringResources.Instance.PromptTemplateInProgress(description)} ({start_count}/{count})";
                    LogWrapper.UpdateInProgress(true, message);
                    ContentItemWrapper item = items[index];

                    await CreateChatResultAsync(item, promptItem.Name);
                    // Save
                    item.Save();
                });
                // Execute if obj is an Action
                afterAction();
                LogWrapper.UpdateInProgress(false);
                LogWrapper.Info(PythonAILibStringResources.Instance.PromptTemplateExecuted(description));
            });

        }
        // ExecuteSystemDefinedPromptを実行する
        public static async Task CreateChatResultAsync(ContentItemWrapper item, string promptName) {
            // システム定義のPromptItemを取得
            PromptItem promptItem = PromptItem.GetPromptItemByName(promptName) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            await CreateChatResultAsync(item, promptItem);
        }

        // PromptItemの内容でチャットを実行して結果をPromptChatResultに保存する
        public static async Task CreateChatResultAsync(ContentItemWrapper item, PromptItem promptItem) {

            // PromptItemのPromptInputNameがある場合はPromptInputNameのContentを取得
            string contentText;
            if (string.IsNullOrEmpty(promptItem.PromptInputName)) {
                contentText = item.Content;
            } else {
                // PromptInputNameのContentを取得
                contentText = item.PromptChatResult.GetTextContent(promptItem.PromptInputName);
                // inputContentがない場合は処理しない
                if (string.IsNullOrEmpty(contentText)) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.InputContentNotFound);
                    return;
                }
            }
            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(item.Content)) {
                LogWrapper.Info(PythonAILibStringResources.Instance.InputContentNotFound);
                return;
            }
            // ヘッダー情報とコンテンツ情報を結合
            // ★TODO タグ情報を追加するか否かはPromptItemの設定にする
            contentText = item.HeaderText + "\n" + contentText;

            // PromptTemplateTextの設定。 UseTagListがTrueの場合は、全タグ情報を追加する
            if (promptItem.UseTagList) {
                string tagListPrompt = await GenerateTagListPrompt();
                contentText = contentText + "\n" + tagListPrompt;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBProperty> vectorSearchProperties = promptItem.UseVectorDB ? item.VectorDBProperties : [];

            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = vectorSearchProperties,
                OpenAIProperties = openAIProperties,
                PromptTemplateText = promptItem.Prompt,
                SplitMode = promptItem.SplitMode,
            };


            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TextContent) {
                string result = ChatUtil.CreateTextChatResult(chatRequestContext, promptItem, contentText);
                if (string.IsNullOrEmpty(result)) {
                    return;
                }
                // PromptOutputTypeがOverwriteContentの場合はContentに結果を保存
                if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteContent) {
                    item.Content = result;
                    return;
                }
                // PromptChatResultに結果を保存
                item.PromptChatResult.SetTextContent(promptItem.Name, result);
                // PromptOutputTypeがOverwriteTitleの場合はDescriptionに結果を保存
                if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteTitle) {
                    item.Description = result;
                }
                return;
            }

            // PromptResultTypeがTableContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TableContent) {
                Dictionary<string, dynamic?> response = ChatUtil.CreateTableChatResult(chatRequestContext, promptItem, contentText);
                // resultからキー:resultを取得
                if (response.ContainsKey("result") == false) {
                    return;
                }
                dynamic? results = response["result"];
                // resultがない場合は処理しない
                if (results == null) {
                    return;
                }
                if (results.Count == 0) {
                    return;
                }
                // resultからDynamicDictionaryObjectを作成
                List<Dictionary<string, object>> resultDictList = [];
                foreach (var result in results) {
                    resultDictList.Add(result);
                }
                // PromptChatResultに結果を保存
                return;
            }

            // PromptResultTypeがListの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.ListContent) {
                List<string> response = ChatUtil.CreateListChatResult(chatRequestContext, promptItem, contentText);
                // PromptOutputTypeがOverwriteTagsの場合はTagsに結果を保存
                if (promptItem.PromptOutputType == PromptOutputTypeEnum.AppendTags) {
                    // タグ一覧を取得
                    List<TagItem> tagItems = await TagItem.GetTagItemsAsync();
                    foreach (var tag in response) {
                        item.Tags.Add(tag);
                        // タグ一覧に存在しない場合は追加
                        if (tagItems.Any(x => x.Tag == tag) == false) {
                            // タグ一覧に追加
                            TagItem tagItemEntity = new() {
                                Tag = tag,
                                IsPinned = false,
                            };
                            await tagItemEntity.SaveAsync();
                        }
                    }
                    return;
                }
                if (response.Count > 0) {
                    // PromptChatResultに結果を保存
                    item.PromptChatResult.SetListContent(promptItem.Name, response);
                }
                return;
            }
        }
        // OpenAIを使用してタイトルを生成する
        public static async Task CreateAutoTitleWithOpenAIAsync(ContentItemWrapper item) {
            // ContentTypeがTextの場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                using PythonAILibDBContext db = new();
                PromptItemEntity? promptItem = db.PromptItems.FirstOrDefault(x => x.Name == SystemDefinedPromptNames.TitleGeneration.ToString());
                if (promptItem == null) {
                    LogWrapper.Error("PromptItem not found");
                    return;
                }
                await CreateChatResultAsync(item, promptItem.Name);
                return;
            }
            // ContentTypeがFiles,の場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Files) {
                // ファイル名をタイトルとして使用
                item.Description += item.FileName;
                return;
            }
            // ContentTypeがImageの場合
            item.Description = "Image";
        }



        // 文章の信頼度を判定する
        public async static Task CheckDocumentReliability(ContentItemWrapper item) {

            await CreateChatResultAsync(item, SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // PromptChatResultからキー：DocumentReliabilityCheck.ToString()の結果を取得
            string result = item.PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // resultがない場合は処理しない
            if (string.IsNullOrEmpty(result)) {
                return;
            }
            // ChatUtl.CreateDictionaryChatResultを実行
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = item.GetFolder().GetVectorSearchProperties(),
                OpenAIProperties = openAIProperties,
            };

            Dictionary<string, dynamic?> response = ChatUtil.CreateDictionaryChatResult(chatRequestContext, new PromptItem() {
                ChatMode = OpenAIExecutionModeEnum.Normal,
                // ベクトルDBを使用する
                UseVectorDB = true,
                Prompt = PromptStringResource.Instance.DocumentReliabilityDictionaryPrompt
            }, result);
            // responseからキー：reliabilityを取得
            if (response.ContainsKey("reliability") == false) {
                return;
            }
            dynamic? reliability = response["reliability"];

            int reliabilityValue = int.Parse(reliability?.ToString() ?? "0");

            // DocumentReliabilityにReliabilityを設定
            item.DocumentReliability = reliabilityValue;
            // responseからキー：reasonを取得
            if (response.ContainsKey("reason")) {
                dynamic? reason = response["reason"];
                // DocumentReliabilityReasonにreasonを設定
                item.DocumentReliabilityReason = reason?.ToString() ?? "";
            }
        }




    }
}

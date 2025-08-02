using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;

namespace LibPythonAI.Model.Prompt {
    public partial class PromptItem {

        // ID
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // 名前
        public string Name { get; set; } = string.Empty;

        // 説明
        public string Description { get; set; } = string.Empty;

        // プロンプト
        public string Prompt { get; set; } = string.Empty;


        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;

        // プロンプト結果の種類
        public PromptResultTypeEnum PromptResultType {
            get {
                ExtendedProperties.TryGetValue("prompt_result_type", out object? value);
                // valueのtypeを確認
                LogWrapper.Debug($"prompt_result_type: {value?.GetType()}");
                if (value is Decimal intValue) {
                    return (PromptResultTypeEnum)intValue;
                }
                return PromptResultTypeEnum.TextContent;
            }
            set {
                ExtendedProperties["prompt_result_type"] = (Decimal)value;
                SaveExtendedPropertiesJson();
            }
        }

        // チャットタイプ
        public OpenAIExecutionModeEnum ChatMode {
            get {
                ExtendedProperties.TryGetValue("chat_mode", out object? value);
                if (value is Decimal intValue) {
                    return (OpenAIExecutionModeEnum)intValue;
                }
                return OpenAIExecutionModeEnum.Normal;
            }
            set {
                ExtendedProperties["chat_mode"] = (Decimal)value;
                SaveExtendedPropertiesJson();
            }
        }

        // 分割モード
        public SplitModeEnum SplitMode {
            get {
                ExtendedProperties.TryGetValue("split_mode", out object? value);
                if (value is Decimal decValue) {
                    return (SplitModeEnum)decValue;
                }
                return SplitModeEnum.None;
            }
            set {
                ExtendedProperties["split_mode"] = (Decimal)value;
                SaveExtendedPropertiesJson();
            }
        }

        // SplitTokenCount
        public int SplitTokenCount {
            get {
                ExtendedProperties.TryGetValue("split_token_count", out object? value);
                if (value is int intValue) {
                    return intValue;
                }
                return 8000; // デフォルト値
            }
            set {
                ExtendedProperties["split_token_count"] = value;
                SaveExtendedPropertiesJson();
            }
        }


        // ベクトルDBを使用する
        public RAGModeEnum RAGMode {
            get {
                ExtendedProperties.TryGetValue("rag_mode", out object? value);
                if (value is Decimal decValue) {
                    return (RAGModeEnum)decValue;
                }
                return RAGModeEnum.None;
            }
            set {
                ExtendedProperties["rag_mode"] = (Decimal)value;
                SaveExtendedPropertiesJson();
            }
        }

        // タグ一覧を参照する
        public bool UseTagList {
            get {
                ExtendedProperties.TryGetValue("use_taglist", out object? value);
                if (value is bool boolValue) {
                    return boolValue;
                }
                return false;
            }
            set {
                ExtendedProperties["use_taglist"] = value;
                SaveExtendedPropertiesJson();
            }
        }

        // プロンプトの出力タイプ
        public PromptOutputTypeEnum PromptOutputType {
            get {
                ExtendedProperties.TryGetValue("prompt_output_type", out object? value);
                if (value is Decimal intValue) {
                    return (PromptOutputTypeEnum)intValue;
                }
                return PromptOutputTypeEnum.NewContent;
            }
            set {
                ExtendedProperties["prompt_output_type"] = (Decimal)value;
                SaveExtendedPropertiesJson();
            }
        }

        // PromptInputName
        public string PromptInputName {
            get {
                ExtendedProperties.TryGetValue("PromptInputName", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                ExtendedProperties["PromptInputName"] = value;
                SaveExtendedPropertiesJson();
            }
        }

        public Dictionary<string, object?> ExtendedProperties { get; set; } = [];


        public string ExtendedPropertiesJson { get; set; } = "{}";


        public void SaveExtendedPropertiesJson() {
            ExtendedPropertiesJson = JsonSerializer.Serialize(ExtendedProperties, JsonUtil.JsonSerializerOptions);
        }

        // SaveAsync
        public void SaveAsync() {
            PromptItemRequest request = new(this);
            PythonExecutor.PythonAIFunctions.UpdatePromptItemAsync(request);
        }


        // DeleteAsync
        public void DeleteAsync() {
            PromptItemRequest request = new(this);
            PythonExecutor.PythonAIFunctions.DeletePromptItemAsync(request);

        }
        // FromDict
        public static PromptItem FromDict(Dictionary<string, object> dict) {
            PromptItem item = new() {
                Id = dict["id"]?.ToString() ?? "",
                Name = dict["name"]?.ToString() ?? "",
                Description = dict["description"]?.ToString() ?? "",
                Prompt = dict["prompt"]?.ToString() ?? "",
                PromptTemplateType = (PromptTemplateTypeEnum)(dict["prompt_template_type"] is Decimal dec ? dec : 0),
            };
            // 拡張プロパティを設定
            if (dict.ContainsKey("extended_properties_json")) {
                item.ExtendedPropertiesJson = dict["extended_properties_json"].ToString() ?? "{}";
                item.ExtendedProperties = JsonUtil.ParseJson(item.ExtendedPropertiesJson);
            }
            return item;
        }

        // タグ一覧を参照するプロンプトを生成する.
        public static async Task<string> GenerateTagListPrompt() {
            // タグ一覧を取得
            List<TagItem> tagItems = await TagItem.GetTagItemsAsync();
            // タグ一覧をカンマ区切りで取得
            string tagList = string.Join(", ", tagItems.Select(x => x.Tag));

            string prompt = PromptStringResourceJa.Instance.TagListPrompt(tagList);
            return prompt;
        }

        // PromptItemを取得
        public static async Task<PromptItem?> GetPromptItemById(string id) {
            var items = await GetPromptItems();
            var item = items.FirstOrDefault(x => x.Id == id);
            if (item == null) {
                return null;
            }
            return item;

        }
        // 名前を指定してPromptItemを取得
        public static async Task<PromptItem?> GetPromptItemByName(string name) {
            using PythonAILibDBContext db = new();
            var items = await GetPromptItems();
            var item = items.FirstOrDefault(x => x.Name == name);
            if (item == null) {
                return null;
            }
            return item;
        }

        // システム定義のPromptItemを取得
        public static async Task<List<PromptItem>> GetSystemPromptItems() {
            using PythonAILibDBContext db = new();
            var items = await GetPromptItems();
            items = items.Where(x => (x.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined || x.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined)).ToList();
            return items;
        }

        // システム定義以外のPromptItemを取得
        public static async Task<List<PromptItem>> GetUserDefinedPromptItems() {
            var items = await GetPromptItems();
            items = items.Where(x => x.PromptTemplateType == PromptTemplateTypeEnum.UserDefined).ToList();
            return items;
        }
        private static bool isInitialized = false;

        private static List<PromptItem> _items = [];

        // List<PromptItem>を取得
        public static async Task<List<PromptItem>> GetPromptItems() {
            if (!isInitialized) {
                await LoadItemsAsync();
            }
            return _items;
        }

        public static async Task LoadItemsAsync() {
            // 修正: 非同期メソッドで 'await' を使用
            _items = await Task.Run(() => PythonExecutor.PythonAIFunctions.GetPromptItemsAsync());
            if (_items == null) {
                throw new Exception(PythonAILibStringResources.Instance.PromptItemsNotLoaded);
            }
            isInitialized = true;
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

                    CreateChatResultAsync(item, promptItem.Name).Wait();
                    // SaveAsync
                    await item.Save();
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

            PromptItem promptItem = await PromptItem.GetPromptItemByName(promptName) ?? throw new Exception("PromptItem not found");
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
            var headerText = await item.GetHeaderTextAsync();
            contentText = headerText + "\n" + contentText;

            // PromptTemplateTextの設定。 UseTagListがTrueの場合は、全タグ情報を追加する
            if (promptItem.UseTagList) {
                string tagListPrompt = await GenerateTagListPrompt();
                contentText = contentText + "\n" + tagListPrompt;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            var vectorDBItems = await item.GetVectorDBProperties();
            ObservableCollection<VectorSearchItem> vectorSearchProperties = promptItem.RAGMode != RAGModeEnum.None ? vectorDBItems : [];

            // ChatRequestContextを作成
            ChatSettings chatSettings = new() {
                VectorSearchRequests = vectorSearchProperties.Select(x => new VectorSearchRequest(x) { Query = contentText }).ToList(),
                RAGMode = promptItem.RAGMode,
                PromptTemplateText = promptItem.Prompt,
                SplitMode = promptItem.SplitMode,
            };


            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TextContent) {
                string result = await ChatUtil.CreateTextChatResult(chatSettings, promptItem, contentText);
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
                Dictionary<string, dynamic?> response = await ChatUtil.CreateTableChatResult(chatSettings, promptItem, contentText);
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
                List<string> response = await ChatUtil.CreateListChatResult(chatSettings, promptItem, contentText);
                // PromptOutputTypeがOverwriteTagsの場合はTagsに結果を保存
                if (promptItem.PromptOutputType == PromptOutputTypeEnum.AppendTags) {
                    // タグ一覧を取得
                    List<TagItem> tagItems = await TagItem.GetTagItemsAsync();
                    foreach (var tag in response) {
                        item.Tags.Add(tag);
                        // タグ一覧に存在しない場合は追加
                        if (tagItems.Any(x => x.Tag == tag) == false) {
                            // タグ一覧に追加
                            TagItem tagItem = new(tag) {
                                Tag = tag,
                                IsPinned = false,
                            };
                            await tagItem.SaveAsync();
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
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                var items = await GetPromptItems();
                PromptItem? promptItem = items.FirstOrDefault(x => x.Name == SystemDefinedPromptNames.TitleGeneration.ToString());
                if (promptItem == null) {
                    LogWrapper.Error("PromptItem not found");
                    return;
                }
                await CreateChatResultAsync(item, promptItem.Name);
                return;
            }
            // ContentTypeがFiles,の場合
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
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
            ChatSettings chatSettings = new() {
                RAGMode = RAGModeEnum.None,
            };

            Dictionary<string, dynamic?> response = await ChatUtil.CreateDictionaryChatResult(chatSettings, new PromptItem() {
                ChatMode = OpenAIExecutionModeEnum.Normal,
                // ベクトルDBを使用する
                RAGMode = RAGModeEnum.NormalSearch,
                Prompt = PromptStringResourceJa.Instance.DocumentReliabilityDictionaryPrompt
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

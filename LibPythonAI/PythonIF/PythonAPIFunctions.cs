using System.Net.Http;
using System.Text;
using System.Text.Json;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Statistics;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using SocketIOClient;

namespace LibPythonAI.PythonIF {

    public class PythonAPIFunctions : IPythonAIFunctions {

        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        private static HttpClient CreateHttpClient() {
            HttpClient client = new(new HttpClientHandler() {
                UseProxy = false,
                // 最大接続数を設定
                MaxConnectionsPerServer = 2,
            });
            // タイムアウトを設定
            client.Timeout = TimeSpan.FromMinutes(5);

            return client;
        }

        private string base_url;
        public PythonAPIFunctions(string base_url) {
            this.base_url = base_url;
        }

        // HttpClient
        public static async Task<string> PostAsync(string endpoint, string requestJson) {

            if (string.IsNullOrEmpty(endpoint)) {
                throw new ArgumentNullException(nameof(endpoint));
            }
            if (string.IsNullOrEmpty(requestJson)) {
                throw new ArgumentNullException(nameof(requestJson));
            }
            //HTTP　POST要求を送信する
            var data = new StringContent(requestJson, Encoding.UTF8, mediaType: "application/json");
            using HttpClient client = CreateHttpClient();
            HttpResponseMessage response = await client.PostAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        // テスト用
        public string HelloWorld() {
            string base_url = $"{this.base_url}/hello_world";
            return PostAsync(base_url, "{}").Result;
        }

        // テスト用
        public static void ShutdownServer(string url) {
            // ClientでPOSTを実行
            using HttpClient client = CreateHttpClient();
            client.PostAsync(url, new StringContent("{}", Encoding.UTF8, mediaType: "application/json"));

        }

        // ContentFolder
        public void UpdateContentFoldersForVectorSearch(List<ContentFolderRequest> folders) {
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    ContentFolderRequestsInstance = folders
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                // LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateContentFoldersExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_content_folders";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeleteContentFoldersForVectorSearch(List<ContentFolderRequest> folders) {
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    ContentFolderRequestsInstance = folders
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                // LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteContentFoldersExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_content_folders";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // SearchRule
        public async Task<List<SearchRule>> GetSearchRulesAsync() {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetSearchRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_search_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // search_rulesを取得
                dynamic dictList = resultDict["search_rules"] ?? "[]";
                List<SearchRule> searchRules = [];
                foreach (var item in dictList) {
                    // SearchRuleを取得
                    SearchRule? searchRule = SearchRule.FromDict(item);
                    if (searchRule != null) {
                        searchRules.Add(searchRule);
                    }
                }
                return searchRules;
            });
            return task;
        }

        public void UpdateSearchRuleAsync(SearchRuleRequest request) {
            Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    SearchRuleRequestsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateSearchRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_search_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeleteSearchRuleAsync(SearchRuleRequest request) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    SearchRuleRequestsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteSearchRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_search_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }

            });
        }


        // AutoProcessItem
        public async Task<List<AutoProcessItem>> GetAutoProcessItemsAsync() {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetAutoProcessItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_auto_process_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // auto_process_itemsを取得
                dynamic dictList = resultDict["auto_process_items"] ?? "[]";
                List<AutoProcessItem> autoProcessItems = [];
                foreach (var item in dictList) {
                    // AutoProcessItemを取得
                    AutoProcessItem? autoProcessItem = AutoProcessItem.FromDict(item);
                    if (autoProcessItem != null) {
                        autoProcessItems.Add(autoProcessItem);
                    }
                }
                return autoProcessItems;
            });
            return task;
        }

        public void UpdateAutoProcessItemAsync(AutoProcessItemRequest request) {
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    AutoProcessItemsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateAutoProcessItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_auto_process_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeleteAutoProcessItemAsync(AutoProcessItemRequest request) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    AutoProcessItemsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteAutoProcessItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_auto_process_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // AutoProcessRule
        public async Task<List<AutoProcessRule>> GetAutoProcessRulesAsync() {
            // Task.Runを使用して非同期に実行
            var task = await Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetAutoProcessRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_auto_process_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // auto_process_rulesを取得
                dynamic dictList = resultDict["auto_process_rules"] ?? "[]";
                List<AutoProcessRule> autoProcessRules = [];
                foreach (var item in dictList) {
                    // AutoProcessRuleを取得
                    AutoProcessRule? autoProcessRule = AutoProcessRule.FromDict(item);
                    if (autoProcessRule != null) {
                        autoProcessRules.Add(autoProcessRule);
                    }
                }
                return autoProcessRules;
            });
            return task;
        }

        public void UpdateAutoProcessRuleAsync(AutoProcessRuleRequest rule) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    AutoProcessRulesInstance = [rule]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateAutoProcessRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_auto_process_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeleteAutoProcessRuleAsync(AutoProcessRuleRequest rule) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    AutoProcessRulesInstance = [rule]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteAutoProcessRulesExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_auto_process_rules";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }


        public async Task<List<PromptItem>> GetPromptItemsAsync() {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetPromptItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_prompt_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // prompt_itemsを取得
                dynamic dictList = resultDict["prompt_items"] ?? "[]";
                List<PromptItem> promptItems = [];
                foreach (var item in dictList) {
                    // PromptItemを取得
                    PromptItem? promptItem = PromptItem.FromDict(item);
                    if (promptItem != null) {
                        promptItems.Add(promptItem);
                    }
                }
                return promptItems;
            });
            return task;
        }

        public void UpdatePromptItemAsync(PromptItemRequest request) {

            Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    PromptItemsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdatePromptItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_prompt_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeletePromptItemAsync(PromptItemRequest request) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    PromptItemsInstance = [request]
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletePromptItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_prompt_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // GetTagItemsAsync
        public async Task<List<TagItem>> GetTagItemsAsync() {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetTagItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_tag_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // tag_itemsを取得
                dynamic dictList = resultDict["tag_items"] ?? "[]";
                List<TagItem> tagItems = [];
                foreach (var item in dictList) {
                    // TagItemを取得
                    TagItem? tagItem = TagItem.FromDict(item);
                    if (tagItem != null) {
                        tagItems.Add(tagItem);
                    }
                }

                return tagItems;
            });
            return task;
        }
        // UpdateTagItemsAsync
        public void UpdateTagItemsAsync(List<TagItem> tagItems) {

            Task.Run(async () => {
                RequestContainer requestContainer = new() {
                    TagItemsInstance = tagItems
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateTagItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_tag_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // DeleteTagItemsAsync
        public void DeleteTagItemsAsync(List<TagItem> tagItems) {
            Task.Run(async () => {
                RequestContainer requestContainer = new() {
                    TagItemsInstance = tagItems
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteTagItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_tag_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // GetTokenCount
        public async Task<long> GetTokenCount(ChatRequestContext chatRequestContext, TokenCountRequest tokenCountRequest) {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                    TokenCountRequestInstance = tokenCountRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetTokenCountExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                long totalTokens = 0;
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_token_count";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
                // total_tokensを取得
                if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                    if (totalTokensValue is decimal totalTokensDecimal) {
                        totalTokens = decimal.ToInt64(totalTokensDecimal);
                    }
                }
                return totalTokens;
            });
            return task;
        }

        // 通常のOpenAIChatを実行する
        public async Task<ChatResponse> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {

            var task = await Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    ChatRequestInstance = chatRequest,
                    RequestContextInstance = chatRequestContext,
                };
                // RequestContainerをJSON文字列に変換
                string requestContextJson = requestContainer.ToJson();

                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.OpenAIExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {requestContextJson}");

                // ChatResultを作成
                ChatResponse chatResult = new();
                // PostAsyncを実行する
                string endpoint = $"{base_url}/openai_chat";
                string resultString = await PostAsync(endpoint, requestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                chatResult.LoadFromJson(resultString);

                // Errorがある場合は例外をスローする
                if (!string.IsNullOrEmpty(chatResult.Error)) {
                    throw new Exception(chatResult.Error);
                }
                // StatisticManagerにトークン数を追加
                MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, chatRequest.Model);
                return chatResult;
            });
            return task;
        }

        // AutoGenのGroupChatを実行する
        public async Task<ChatResponse> AutoGenGroupChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration) {
            var task = await Task.Run(async () => {
                // ChatRequestから最後のユーザー発言を取得
                ChatMessage? lastUserRoleMessage = chatRequest.GetLastSendItem() ?? new ChatMessage("", "");
                string inputText = lastUserRoleMessage.Content;
                // messageが空の場合はLogWrapper.Errorを呼び出す
                if (string.IsNullOrEmpty(inputText)) {
                    LogWrapper.Error("Message is empty.");
                }
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                    ChatRequestInstance = chatRequest,
                };
                // RequestContainerをJSON文字列に変換
                string requestContextJson = requestContainer.ToJson();


                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {requestContextJson}");

                ChatResponse chatResult = new();
                string endpoint = $"{base_url}/autogen_group_chat";
                // endpoint を  host:port 部分と path 部分に分割
                Uri uri = new(endpoint);
                string host = uri.GetLeftPart(UriPartial.Authority);
                string path = uri.PathAndQuery;
                SocketIOOptions options = new() {
                    AutoUpgrade = false,
                };

                bool finished = false;
                var client = new SocketIOClient.SocketIO(host, options);

                // responseイベントを受信した場合
                client.On("response", response => {
                    // responseをログに出力
                    string resultString = response.GetValue<string>();
                    LogWrapper.Debug(resultString);

                    // responseをDictionaryに変換
                    Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                    // messageを取得
                    if (resultDict.TryGetValue("message", out dynamic? messageValue)) {
                        iteration(messageValue);
                    }

                    // logを取得
                    if (resultDict.TryGetValue("log", out dynamic? logValue)) {
                        LogWrapper.Debug(logValue);
                    }

                    // total_tokensを取得
                    if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                        if (totalTokensValue is decimal totalTokens) {
                            chatResult.TotalTokens = decimal.ToInt64(totalTokens);
                        }
                    }

                    // Errorがある場合はLogWrapper.Errorを呼び出す
                    if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                        LogWrapper.Error(errorValue?.ToString());
                    }
                });

                //Errorイベントを受信した場合
                client.On("error", response => {
                    LogWrapper.Error(response.GetValue<string>());
                    // 切断
                    client.DisconnectAsync();
                    finished = true;
                });

                // closeイベントを受信した場合
                client.On("close", response => {
                    LogWrapper.Info("Connection closed");
                    client.DisconnectAsync();
                    finished = true;
                });
                client.OnConnected += async (sender, e) => {
                    await client.EmitAsync("autogen_chat", requestContextJson);
                };
                // サーバーに接続
                await client.ConnectAsync();
                // 終了するまで待機
                while (!finished) {
                    await Task.Delay(1000);
                }

                return chatResult;
            });
            return task;

        }

        public void CancelAutoGenChat(string sessionToken) {
            // SessionToken: sessionTokenを持つJSON文字列を作成
            string sessionTokenJson = JsonSerializer.Serialize(new { session_token = sessionToken }, JsonUtil.JsonSerializerOptions);
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo}:{sessionTokenJson}");
            // cancel_request
            // PostAsyncを実行する
            string endpoint = $"{base_url}/cancel_autogen_chat";
            string resultString = PostAsync(endpoint, sessionTokenJson).Result;

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

        }
        public async Task<string> ExtractFileToTextAsync(string path) {
            var task = await Task.Run(async () => {
                // FileRequestを作成
                FileRequest fileRequest = new() {
                    FilePath = path
                };
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    FileRequestInstance = fileRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();


                PythonScriptResult result = new();
                // PostAsyncを実行する
                string endpoint = $"{base_url}/extract_text_from_file";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    throw new Exception(result.Error);
                }
                return result.Output;
            });
            return task;
        }

        public async Task<string> ExtractBase64ToText(string base64, string extension) {
            var task = await Task.Run(async () => {
                // FileRequestを作成
                FileRequest fileRequest = new() {
                    Base64Data = base64,
                    Extension = extension
                };
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    FileRequestInstance = fileRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                // Log出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");


                // ResultContainerを作成
                PythonScriptResult result = new();
                // PostAsyncを実行する
                string endpoint = $"{base_url}/extract_base64_to_text";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    throw new Exception(result.Error);
                }
                return result.Output;
            });
            return task;
        }

        public void UpdateVectorDBItem(VectorDBItem item) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {

                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    VectorDBItemRequestInstance = new VectorDBItemRequest(item)
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateVectorDBItemExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/update_vector_db_item";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // ベクトルDBを削除する
        public void DeleteVectorDBItem(VectorDBItem item) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    VectorDBItemRequestInstance = new VectorDBItemRequest(item)
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteVectorDBItemExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/delete_vector_db_item";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // public List<VectorDBItem> GetVectorDBItemsAsync();
        public async Task<List<VectorDBItem>> GetVectorDBItemsAsync() {
            // Task.Runを使用して非同期に実行
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new();
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemsExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_vector_db_items";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }

                // VectorDBItemのリストを取得
                dynamic dictList = resultDict["vector_db_items"] ?? "[]";
                List<VectorDBItem> vectorDBItems = [];
                foreach (var item in dictList) {
                    // VectorDBItemを取得
                    VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(item).CreateVectorDBItem();
                    if (vectorDBItem != null) {
                        vectorDBItems.Add(vectorDBItem);
                    }
                }
                return vectorDBItems;
            });
            return task;
        }
        // public VectorDBItem? GetVectorDBItemById(string id);
        public async Task<VectorDBItem?> GetVectorDBItemById(string id) {
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    VectorDBItemRequestInstance = new VectorDBItemRequest(new VectorDBItem() { Id = id })
                };

                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemByIdExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_vector_db_item_by_id";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }

                // VectorDBItemを取得
                VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(resultDict["vector_db_item"] ?? "[]").CreateVectorDBItem();
                return vectorDBItem;
            });
            return task;
        }
        // public VectorDBItem? GetVectorDBItemByName(string name);
        public async Task<VectorDBItem?> GetVectorDBItemByName(string name) {
            // Task.Runを使用して非同期に実行
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    VectorDBItemRequestInstance = new VectorDBItemRequest(new VectorDBItem() { Name = name })
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GetVectorDBItemByNameExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_vector_db_item_by_name";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }


                // VectorDBItemを取得
                VectorDBItem? vectorDBItem = VectorDBItemResponse.FromDict(resultDict["vector_db_item"] ?? "[]").CreateVectorDBItem();
                return vectorDBItem;
            });
            return task;
        }

        public async Task<List<VectorEmbeddingItem>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query) {

            // Task.Runを使用して非同期に実行
            var task = await Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();

                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.VectorSearchExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.VectorSearchRequest}:{query}");

                // vector_search

                // PostAsyncを実行する
                string endpoint = $"{base_url}/vector_search";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                // resultStringからDictionaryに変換する。
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

                if (resultDict == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }

                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }

                List<VectorEmbeddingItem> vectorSearchResults = new();
                List<string> textResults = new();
                // documentsがある場合は取得
                var documents = resultDict.GetValueOrDefault("documents") as List<dynamic> ?? throw new Exception(StringResources.OpenAIResponseEmpty);
                if (documents == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                foreach (var item in documents) {
                    VectorSearchResponse? VectorEmbeddingItem = VectorSearchResponse.FromDict(item);
                    if (VectorEmbeddingItem != null) {
                        vectorSearchResults.Add(VectorEmbeddingItem.CreateVectorEmbeddingItem());
                    }
                }
                return vectorSearchResults;
            });

            return task;
        }


        public void DeleteEmbeddingsByFolderAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                    EmbeddingRequestInstance = embeddingRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeleteEmbeddingsByFolderExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // delete_embeddings_by_folder
                // endpointを作成
                string endpoint = $"{base_url}/delete_embeddings_by_folder";
                // PostAsyncを実行する
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        public void DeleteEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成

                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                    EmbeddingRequestInstance = embeddingRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();


                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateEmbeddingExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // delete_embeddings
                // endpointを作成
                string endpoint = $"{base_url}/delete_embeddings";
                // PostAsyncを実行する
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");


                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });

        }

        public void UpdateEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // RequestContainerを作成

                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                    EmbeddingRequestInstance = embeddingRequest
                };

                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();


                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UpdateEmbeddingExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");
                // update_embeddings
                // endpointを作成
                string endpoint = $"{base_url}/update_embeddings";
                // PostAsyncを実行する
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });
        }

        // ExportToExcelを実行する
        public void ExportToExcelAsync(string filePath, CommonDataTable data) {
            // Task.Runを使用して非同期に実行
            Task.Run(async () => {
                // ExcelRequestを作成
                ExcelRequest excelRequest = new(filePath, data);
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    ExcelRequestInstance = excelRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();

                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.ExportToExcelExecute);
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo}:{chatRequestContextJson}");

                // export_to_excel
                // PostAsyncを実行する
                string endpoint = $"{base_url}/export_to_excel";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);

                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");

                Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }
            });

        }

        // ImportFromExcelを実行する
        public async Task<CommonDataTable> ImportFromExcel(string filePath) {
            var task = await Task.Run(async () => {
                // FileRequestを作成
                FileRequest fileRequest = new() {
                    FilePath = filePath
                };
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    FileRequestInstance = fileRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();


                CommonDataTable result = new([]);
                // import_from_excel

                // PostAsyncを実行する
                string endpoint = $"{base_url}/import_from_excel";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, JsonUtil.JsonSerializerOptions);
                if (resultDict == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                // Errorがある場合は例外をスローする
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    throw new Exception(errorValue);
                }

                // documents を取得
                JsonElement? documentsObject = (JsonElement)resultDict["rows"];
                if (documentsObject == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }

                // JSON文字列からList<List<string>>に変換する。
                if (string.IsNullOrEmpty(resultString) == false) {
                    result = CommonDataTable.FromJson(documentsObject.ToString() ?? "[]");
                }
                return result;
            });
            return task;
        }

        // GetMimeType
        public async Task<string> GetMimeType(string filePath) {

            var task = await Task.Run(async () => {
                FileRequest fileRequest = new() {
                    FilePath = filePath
                };
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    FileRequestInstance = fileRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();

                // ResultContainerを作成
                PythonScriptResult result = new();
                // get_mime_type
                // PostAsyncを実行する
                string endpoint = $"{base_url}/get_mime_type";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);
                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    throw new Exception(result.Error);
                }
                return result.Output;
            });
            return task;
        }

        // public string ExtractWebPage(string url);
        public async Task<string> ExtractWebPage(string url) {
            // Task.Runを使用して非同期に実行
            var task = await Task.Run(async () => {
                // FileRequestを作成
                WebRequest webRequest = new(url);
                // RequestContainerを作成
                RequestContainer requestContainer = new() {
                    WebRequestInstance = webRequest
                };
                // RequestContainerをJSON文字列に変換
                string chatRequestContextJson = requestContainer.ToJson();
                // Log出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.RequestInfo} {chatRequestContextJson}");

                // ResultContainerを作成
                PythonScriptResult result = new();
                // extract_webpage
                // PostAsyncを実行する
                string endpoint = $"{base_url}/extract_webpage";
                string resultString = await PostAsync(endpoint, chatRequestContextJson);

                // resultStringをログに出力
                LogWrapper.Debug($"{PythonAILibStringResourcesJa.Instance.Response}:{resultString}");
                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    throw new Exception(result.Error);
                }
                return result.Output;
            });
            return task;
        }
    }
}

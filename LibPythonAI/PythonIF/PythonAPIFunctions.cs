using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Statistics;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;
using SocketIOClient;

namespace PythonAILib.PythonIF {

    public class PythonAPIFunctions : IPythonAIFunctions {


        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

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
        public async Task UpdateContentFoldersForVectorSearch(List<ContentFolderRequest> folders) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = folders
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        public async Task DeleteContentFoldersForVectorSearch(List<ContentFolderRequest> folders) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = folders
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_content_folders";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }



        // GetTagItemsAsync
        public async Task<List<TagItem>> GetTagItemsAsync() {
            // RequestContainerを作成
            RequestContainer requestContainer = new();
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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
        }
        // UpdateTagItemsAsync
        public async Task UpdateTagItemsAsync(List<TagItem> tagItems) {

            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // DeleteTagItemsAsync
        public async Task DeleteTagItemsAsync(List<TagItem> tagItems) {
            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_tag_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // GetTokenCount
        public async Task<long> GetTokenCount(ChatRequestContext chatRequestContext, string inputText) {

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                TokenCountRequestInstance = new(inputText)
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.GetTokenCountExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");

            long totalTokens = 0;
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_token_count";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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
        }

        // 通常のOpenAIChatを実行する
        public async Task<ChatResult> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {


            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ChatRequestInstance = chatRequest,
                RequestContextInstance = chatRequestContext,
            };
            // RequestContainerをJSON文字列に変換
            string requestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.OpenAIExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            // ChatResultを作成
            ChatResult chatResult = new();
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/openai_chat";
            string resultString = await PostAsync(endpoint, requestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            chatResult.LoadFromJson(resultString);

            // Errorがある場合は例外をスローする
            if (!string.IsNullOrEmpty(chatResult.Error)) {
                throw new Exception(chatResult.Error);
            }
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, chatRequestContext.OpenAIProperties.OpenAICompletionModel);
            return chatResult;
        }

        // AutoGenのGroupChatを実行する
        public async Task<ChatResult> AutoGenGroupChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration) {

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


            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            ChatResult chatResult = new();
            string endpoint = $"{this.base_url}/autogen_group_chat";
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

        }

        public void CancelAutoGenChat(string sessionToken) {
            // SessionToken: sessionTokenを持つJSON文字列を作成
            string sessionTokenJson = JsonSerializer.Serialize(new { session_token = sessionToken }, jsonSerializerOptions);
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{sessionTokenJson}");
            // cancel_request
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/cancel_autogen_chat";
            string resultString = PostAsync(endpoint, sessionTokenJson).Result;

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

        }
        public async Task<string> ExtractFileToTextAsync(string path) {

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
            string endpoint = $"{this.base_url}/extract_text_from_file";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public async Task<string> ExtractBase64ToText(string base64, string extension) {
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
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");


            // ResultContainerを作成
            PythonScriptResult result = new();
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/extract_base64_to_text";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public async Task UpdateVectorDBItem(VectorDBItem item) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemInstance = item
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBItemExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_vector_db_item";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // ベクトルDBを削除する
        public async Task DeleteVectorDBItem(VectorDBItem item) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemInstance = item
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteVectorDBItemExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_vector_db_item";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // public List<VectorDBItem> GetVectorDBItemsAsync();
        public async Task<List<VectorDBItem>> GetVectorDBItemsAsync() {
            // RequestContainerを作成
            RequestContainer requestContainer = new();
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetVectorDBItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_vector_db_items";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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
                VectorDBItem? vectorDBItem = VectorDBItem.FromDict(item);
                if (vectorDBItem != null) {
                    vectorDBItems.Add(vectorDBItem);
                }
            }
            return vectorDBItems;
        }
        // public VectorDBItem? GetVectorDBItemById(string id);
        public VectorDBItem? GetVectorDBItemById(string id) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemInstance = new VectorDBItem() { Id = id }
            };

            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetVectorDBItemByIdExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_vector_db_item_by_id";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // VectorDBItemを取得
            VectorDBItem? vectorDBItem = VectorDBItem.FromDict(resultDict["vector_db_item"] ?? "[]");
            return vectorDBItem;
        }
        // public VectorDBItem? GetVectorDBItemByName(string name);
        public VectorDBItem? GetVectorDBItemByName(string name) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                VectorDBItemInstance = new VectorDBItem() { Name = name }
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetVectorDBItemByNameExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_vector_db_item_by_name";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }


            // VectorDBItemを取得
            VectorDBItem? vectorDBItem = VectorDBItem.FromDict(resultDict["vector_db_item"] ?? "[]");
            return vectorDBItem;
        }

        public async Task<List<VectorEmbedding>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query) {

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.VectorSearchExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.VectorSearchRequest}:{query}");

            // vector_search

            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/vector_search";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            if (resultDict == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

            // VectorSearchResultのリストを作成
            List<VectorEmbedding> vectorSearchResults = [];


            // documentsがある場合は取得
            if (resultDict.ContainsKey("documents")) {
                var documents = resultDict["documents"];
                if (documents == null) {
                    throw new Exception(StringResources.OpenAIResponseEmpty);
                }
                foreach (var item in documents) {
                    // VectorEmbeddingを取得
                    VectorEmbedding? vectorEmbedding = VectorEmbedding.FromDict(item);
                    if (vectorEmbedding != null) {
                        vectorSearchResults.Add(vectorEmbedding);
                    }
                }
            }

            return vectorSearchResults;

        }


        public async Task DeleteEmbeddingsByFolderAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteEmbeddingsByFolderExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // delete_embeddings_by_folder
            // endpointを作成
            string endpoint = $"{this.base_url}/delete_embeddings_by_folder";
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

        }

        public async Task DeleteEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // RequestContainerを作成

            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateEmbeddingExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // delete_embeddings
            // endpointを作成
            string endpoint = $"{this.base_url}/delete_embeddings";
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");


            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        public async Task UpdateEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest) {

            // RequestContainerを作成

            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };

            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateEmbeddingExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // update_embeddings
            // endpointを作成
            string endpoint = $"{this.base_url}/update_embeddings";
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // ExportToExcelを実行する
        public async Task ExportToExcelAsync(string filePath, CommonDataTable data) {
            // ExcelRequestを作成
            ExcelRequest excelRequest = new(filePath, data);
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ExcelRequestInstance = excelRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.ExportToExcelExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{chatRequestContextJson}");

            // export_to_excel
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/export_to_excel";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

        }

        // ImportFromExcelを実行する
        public async Task<CommonDataTable> ImportFromExcel(string filePath) {
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
            string endpoint = $"{this.base_url}/import_from_excel";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, jsonSerializerOptions);
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
        }

        // GetMimeType
        public async Task<string> GetMimeType(string filePath) {

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
            string endpoint = $"{this.base_url}/get_mime_type";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        // public string ExtractWebPage(string url);
        public async Task<string> ExtractWebPage(string url) {
            // FileRequestを作成
            WebRequest webRequest = new(url);
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                WebRequestInstance = webRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");

            // ResultContainerを作成
            PythonScriptResult result = new();
            // extract_webpage
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/extract_webpage";
            string resultString = await PostAsync(endpoint, chatRequestContextJson);

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public async Task<List<AutoGenLLMConfig>> GetAutoGenLLMConfigListAsync() {

            string endpoint = $"{this.base_url}/get_autogen_llm_config_list";
            string resultString = await PostAsync(endpoint, "{}");

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // autogen_llm_config_listを取得   
            List<AutoGenLLMConfig> autogenLLMConfigList = [];
            // llm_config_listがある場合は取得
            if (resultDict.TryGetValue("llm_config_list", out dynamic? dictList)) {
                if (dictList != null) {

                    foreach (var item in dictList) {
                        // AutoGenLLMConfigを取得
                        AutoGenLLMConfig? autogenLLMConfig = AutoGenLLMConfig.FromDict(item);
                        if (autogenLLMConfig != null) {
                            autogenLLMConfigList.Add(autogenLLMConfig);
                        }
                    }
                }
            }
            return autogenLLMConfigList;

        }

        public async Task<AutoGenLLMConfig?> GetAutoGenLLMConfigAsync(string name) {

            // AutoGenLLMConfigを作成
            AutoGenLLMConfig autogenLLMConfig = new() {
                Name = name
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenLLMConfigInstance = autogenLLMConfig
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            string endpoint = $"{this.base_url}/get_autogen_llm_config";

            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{requestJson}");
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // AutoGenLLMConfigを取得
            AutoGenLLMConfig? autogenLLMConfigResult;
            if (resultDict.TryGetValue("llm_config", out dynamic? autogenLLMConfigValue)) {
                autogenLLMConfigResult = AutoGenLLMConfig.FromDict(autogenLLMConfigValue);
            } else {
                autogenLLMConfigResult = null;
            }
            return autogenLLMConfigResult;
        }

        public async Task UpdateAutogenLLMConfigAsync(AutoGenLLMConfig config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenLLMConfigInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateAutogenLLMConfigExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_autogen_llm_config";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

        }

        public async Task DeleteAutogenLLMConfigAsync(AutoGenLLMConfig config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenLLMConfigInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteAutogenLLMConfigExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_autogen_llm_config";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        // AutoGenTool
        public async Task<List<AutoGenTool>> GetAutoGenToolListAsync() {
            string endpoint = $"{this.base_url}/get_autogen_tool_list";
            string resultString = await PostAsync(endpoint, "{}");
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // tool_listを取得
            List<AutoGenTool> autogenToolList = [];
            // llm_config_listがある場合は取得
            if (resultDict.TryGetValue("tool_list", out dynamic? dictList)) {
                if (dictList != null) {
                    foreach (var item in dictList) {
                        // AutoGenToolを取得
                        AutoGenTool? autogenTool = AutoGenTool.FromDict(item);
                        if (autogenTool != null) {
                            autogenToolList.Add(autogenTool);
                        }
                    }
                }
            }
            return autogenToolList;
        }

        public async Task<AutoGenTool?> GetAutoGenToolAsync(string name) {
            // AutoGenToolを作成
            AutoGenTool autogenTool = new() {
                Name = name
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenToolInstance = autogenTool
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();


            string endpoint = $"{this.base_url}/get_autogen_tool";
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{requestJson}");
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // AutoGenToolを取得
            AutoGenTool? autogenToolResult;
            if (resultDict.TryGetValue("tool", out dynamic? autogenToolValue)) {
                autogenToolResult = AutoGenTool.FromDict(autogenToolValue);
            } else {
                autogenToolResult = null;
            }

            return autogenToolResult;
        }

        public async Task UpdateAutoGenToolAsync(AutoGenTool config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenToolInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateAutoGenToolExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_autogen_tool";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        public async Task DeleteAutoGenToolAsync(AutoGenTool config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenToolInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteAutoGenToolExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_autogen_tool";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        // AutoGenAgent
        public async Task<List<AutoGenAgent>> GetAutoGenAgentListAsync() {
            string endpoint = $"{this.base_url}/get_autogen_agent_list";
            string resultString = await PostAsync(endpoint, "{}");
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            var dictList = resultDict["agent_list"] ?? "[]";
            // autogen_llm_config_listを取得
            List<AutoGenAgent> autogenAgentList = [];
            foreach (var item in dictList) {
                // AutoGenAgentを取得
                AutoGenAgent? autogenAgent = AutoGenAgent.FromDict(item);
                if (autogenAgent != null) {
                    autogenAgentList.Add(autogenAgent);
                }
            }
            return autogenAgentList;
        }

        public async Task<AutoGenAgent> GetAutoGenAgentAsync(string name) {
            // AutoGenAgentを作成
            AutoGenAgent autogenAgent = new() {
                Name = name
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenAgentInstance = autogenAgent
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            string endpoint = $"{this.base_url}/get_autogen_agent";
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{requestJson}");
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // AutoGenAgentを取得
            AutoGenAgent? autogenAgentResult = AutoGenAgent.FromDict(resultDict["agent"]);
            return autogenAgentResult;

        }

        public async Task UpdateAutoGenAgentAsync(AutoGenAgent config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenAgentInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateAutoGenAgentExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_autogen_agent";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        public async Task DeleteAutoGenAgentAsync(AutoGenAgent config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenAgentInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteAutoGenAgentExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_autogen_agent";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        // AutoGenGroupChat
        public async Task<List<AutoGenGroupChat>> GetAutoGenGroupChatListAsync() {
            string endpoint = $"{this.base_url}/get_autogen_group_chat_list";
            string resultString = await PostAsync(endpoint, "{}");
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            var dictList = resultDict["group_chat_list"] ?? "[]";
            // autogen_llm_config_listを取得
            List<AutoGenGroupChat> autogenGroupChatList = [];
            foreach (var item in dictList) {
                // AutoGenGroupChatを取得
                AutoGenGroupChat? autogenGroupChat = AutoGenGroupChat.FromDict(item);
                if (autogenGroupChat != null) {
                    autogenGroupChatList.Add(autogenGroupChat);
                }
            }
            return autogenGroupChatList;
        }

        public async Task<AutoGenGroupChat> GetAutoGenGroupChatAsync(string name) {
            // AutoGenGroupChatを作成
            AutoGenGroupChat autogenGroupChat = new() {
                Name = name
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenGroupChatInstance = autogenGroupChat
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();
            string endpoint = $"{this.base_url}/get_autogen_group_chat";
            // Log出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo}:{requestJson}");
            // PostAsyncを実行する
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // AutoGenGroupChatを取得
            AutoGenGroupChat? autogenGroupChatResult = AutoGenGroupChat.FromDict(resultDict["group_chat"]);
            return autogenGroupChatResult;
        }

        public async Task UpdateAutoGenGroupChatAsync(AutoGenGroupChat config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenGroupChatInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateAutoGenGroupChatExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_autogen_group_chat";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }

        public async Task DeleteAutoGenGroupChatAsync(AutoGenGroupChat config) {
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                AutoGenGroupChatInstance = config
            };
            // RequestContainerをJSON文字列に変換
            string requestJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteAutoGenGroupChatExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_autogen_group_chat";
            string resultString = await PostAsync(endpoint, requestJson);
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
        }


    }
}

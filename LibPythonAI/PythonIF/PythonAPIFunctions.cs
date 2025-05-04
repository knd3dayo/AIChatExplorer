using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Statistics;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
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
        public List<ContentFolderWrapper> GetRootContentFolders(List<string> folderTypeStrings) {

            List<ContentFolderRequest> contentFolderRequests = [];
            foreach (string folderTypeString in folderTypeStrings) {
                // ContentFolderRequestを作成
                ContentFolderRequest contentFolderRequest = new() {
                    FolderTypeString = folderTypeString
                };

                contentFolderRequests.Add(contentFolderRequest);
            }
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ContentFolderRequestsInstance = contentFolderRequests
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetRootContentFoldersExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_root_content_folders";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
            // ContentFolderWrapperのリストを取得
            dynamic dictList = resultDict["content_folders"] ?? "[]";
            List<ContentFolderWrapper> contentFolders = [];
            foreach (var item in dictList) {
                // ContentFolderWrapperを取得
                ContentFolderWrapper? contentFolder = ContentFolderWrapper.FromDict(item);
                if (contentFolder != null) {
                    contentFolders.Add(contentFolder);
                }
            }
            return contentFolders;

        }

        // GetTagItems
        public List<TagItem> GetTagItems() {
            // RequestContainerを作成
            RequestContainer requestContainer = new();
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_tag_items";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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
        // UpdateTagItems
        public void UpdateTagItems(List<TagItem> tagItems) {

            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/update_tag_items";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // DeleteTagItems
        public void DeleteTagItems(List<TagItem> tagItems) {
            RequestContainer requestContainer = new() {
                TagItemsInstance = tagItems
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteTagItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/delete_tag_items";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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
        public long GetTokenCount(ChatRequestContext chatRequestContext, string inputText) {

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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

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
        public ChatResult OpenAIChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {

            // VectorSearchRequestsを作成
            List<VectorSearchRequest> vectorSearchRequests = [];
            foreach (VectorDBProperty vectorDBProperty in chatRequestContext.VectorDBProperties) {
                // VectorSearchRequestを作成
                string? name = VectorDBItem.GetItemById(vectorDBProperty.VectorDBItemId)?.Name;
                if (string.IsNullOrEmpty(name)) {
                    throw new Exception(StringResources.PropertyNotSet("VectorDBItem.Name"));
                }
                VectorSearchRequest vectorSearchRequest = new(name, chatRequest.ContentText, vectorDBProperty.TopK, vectorDBProperty.FolderId, vectorDBProperty.ContentType);
                vectorSearchRequests.Add(vectorSearchRequest);
            }

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ChatRequestInstance = chatRequest,
                RequestContextInstance = chatRequestContext,
                VectorSearchRequestsInstance = vectorSearchRequests
            };
            // RequestContainerをJSON文字列に変換
            string requestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.OpenAIExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            // ChatResultを作成
            ChatResult chatResult = new();
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/openai_chat";
            string resultString = PostAsync(endpoint, requestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            chatResult.LoadFromJson(resultString);

            // Errorがある場合は例外をスローする
            if ( ! string.IsNullOrEmpty(chatResult.Error)) {
                throw new Exception(chatResult.Error);
            }
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, chatRequestContext.OpenAIProperties.OpenAICompletionModel);
            return chatResult;
        }

        // AutoGenのGroupChatを実行する
        public ChatResult AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration) {

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
                AutogenRequestInstance = new(inputText),
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

            Task.Run(async () => {
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


            }).Wait();
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
        public string ExtractFileToText(string path) {

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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        public string ExtractBase64ToText(string base64, string extension) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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

        public void UpdateVectorDBItem(VectorDBItem item) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // ベクトルDBを削除する
        public void DeleteVectorDBItem(VectorDBItem item) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);

            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // public List<VectorDBItem> GetVectorDBItems();
        public List<VectorDBItem> GetVectorDBItems() {
            // RequestContainerを作成
            RequestContainer requestContainer = new();
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.GetVectorDBItemsExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/get_vector_db_items";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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

        public List<VectorDBEmbedding> VectorSearch(ChatRequestContext chatRequestContext, string query) {

            // VectorSearchRequestsを作成
            List<VectorSearchRequest> vectorSearchRequests = [];
            foreach (VectorDBProperty vectorDBProperty in chatRequestContext.VectorDBProperties) {
                // VectorSearchRequestを作成
                string? name = VectorDBItem.GetItemById(vectorDBProperty.VectorDBItemId)?.Name;
                if (string.IsNullOrEmpty(name)) {
                    throw new Exception(StringResources.PropertyNotSet("VectorDBItem.Name"));
                }
                VectorSearchRequest vectorSearchRequest = new(name, query, vectorDBProperty.TopK, vectorDBProperty.FolderId, vectorDBProperty.ContentType);
                vectorSearchRequests.Add(vectorSearchRequest);
            }

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                VectorSearchRequestsInstance = vectorSearchRequests
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.VectorSearchExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.VectorSearchRequest}:{query}");

            // vector_search
            // VectorSearchResultのリストを作成
            List<VectorDBEmbedding> vectorSearchResults = [];

            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/vector_search";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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

            // documentsがある場合は取得
            if (resultDict.ContainsKey("documents")) {
                var documents = resultDict["documents"];
                foreach (var item in documents) {
                    // VectorDBEmbeddingを取得
                    VectorDBEmbedding? vectorDBEmbedding = VectorDBEmbedding.FromDict(item);
                    if (vectorDBEmbedding != null) {
                        vectorSearchResults.Add(vectorDBEmbedding);
                    }
                }
                // List<VectorSearchResult>に変換
               //  vectorSearchResults = VectorDBEmbedding.FromDictList(documents);
            }

            return vectorSearchResults;

        }

        // 指定されたベクトルDBのコレクションを削除する
        public void DeleteVectorDBCollection(ChatRequestContext chatRequestContext) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;

            // chatRequestContext.VectorDBProperties[0]
            VectorDBItem? vectorDBItem = VectorDBItem.GetItemById(chatRequestContext.VectorDBProperties[0].VectorDBItemId);
            if (vectorDBItem == null) {
                throw new Exception(StringResources.PropertyNotSet("VectorDBItem"));
            }
            string name = vectorDBItem.Name;
            string model = chatRequestContext.OpenAIProperties.OpenAIEmbeddingModel;
            VectorDBEmbedding embedding = chatRequestContext.VectorDBProperties[0].VectorMetadata;

            // EmbeddingRequestを作成
            EmbeddingRequest embeddingRequest = new(name, model, embedding);
            // RequestContainerを作成

            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                EmbeddingRequestInstance = embeddingRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteVectorDBCollectionExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // DeleteVectorDBIndexExecuteを呼び出す
            // endpointを作成
            string endpoint = $"{this.base_url}/delete_embeddings";
            PythonScriptResult result = new();
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");


            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }


        public void DeleteEmbeddings(ChatRequestContext chatRequestContext) {

            // chatRequestContext.VectorDBProperties[0]
            VectorDBItem? vectorDBItem = VectorDBItem.GetItemById(chatRequestContext.VectorDBProperties[0].VectorDBItemId);
            if (vectorDBItem == null) {
                throw new Exception(StringResources.PropertyNotSet("VectorDBItem"));
            }
            string name = vectorDBItem.Name;
            string model = chatRequestContext.OpenAIProperties.OpenAIEmbeddingModel;
            VectorDBEmbedding embedding = chatRequestContext.VectorDBProperties[0].VectorMetadata;

            // EmbeddingRequestを作成
            EmbeddingRequest embeddingRequest = new(name, model, embedding);
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");


            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        public void UpdateEmbeddings(ChatRequestContext chatRequestContext) {

            // chatRequestContext.VectorDBProperties[0]
            VectorDBItem? vectorDBItem = VectorDBItem.GetItemById(chatRequestContext.VectorDBProperties[0].VectorDBItemId);
            if (vectorDBItem == null) {
                throw new Exception(StringResources.PropertyNotSet("VectorDBItem"));
            }
            string name = vectorDBItem.Name;
            string model = chatRequestContext.OpenAIProperties.OpenAIEmbeddingModel;
            VectorDBEmbedding embedding = chatRequestContext.VectorDBProperties[0].VectorMetadata;

            // EmbeddingRequestを作成
            EmbeddingRequest embeddingRequest = new(name, model, embedding);
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }
        }

        // ExportToExcelを実行する
        public void ExportToExcel(string filePath, CommonDataTable data) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // Errorがある場合は例外をスローする
            if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                throw new Exception(errorValue);
            }

        }

        // ImportFromExcelを実行する
        public CommonDataTable ImportFromExcel(string filePath) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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
        public string GetMimeType(string filePath) {

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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                throw new Exception(result.Error);
            }
            return result.Output;
        }

        // public string ExtractWebPage(string url);
        public string ExtractWebPage(string url) {
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
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

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


    }
}

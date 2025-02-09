using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.PythonIF;
using Python.Runtime;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Statistics;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace PythonAILib.PythonIF {

    public class PythonAPIFunctions : IPythonAIFunctions {

        private readonly Dictionary<string, PyModule> PythonModules = [];

        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        private static readonly HttpClient Client = new HttpClient();

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
            HttpResponseMessage response = await Client.PostAsync(endpoint, data);
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
            Client.PostAsync(url, new StringContent("{}", Encoding.UTF8, mediaType: "application/json"));

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

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                ChatRequestInstance = chatRequest,
                RequestContextInstance = chatRequestContext
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

            // JSON文字列からDictionaryに変換する。
            Dictionary<string, dynamic?> resultDict = JsonUtil.ParseJson(resultString);
            // contentを取得
            if (resultDict.TryGetValue("output", out dynamic? outputValue)) {
                string output = outputValue?.ToString() ?? "";
                // ChatResultに設定
                chatResult.Output = output;
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

            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, chatRequestContext.OpenAIProperties.OpenAICompletionModel);
            return chatResult;
        }

        // AutoGenのGroupChatを実行する
        public  ChatResult AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration) {

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
                AutogenRequestInstance = new(inputText)
            };
            // RequestContainerをJSON文字列に変換
            string requestContextJson = requestContainer.ToJson();


            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            ChatResult chatResult = new();
            string endpoint = $"{this.base_url}/autogen_group_chat";

            var client = new SocketIOClient.SocketIO(endpoint);
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
            // closeイベントを受信した場合
            client.On("close", response => {
                LogWrapper.Info("Connection closed");
            });
            // サーバーに接続
             client.EmitAsync("connect", requestContextJson);

            return chatResult;

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
                LogWrapper.Error(result.Error);
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
                LogWrapper.Error(result.Error);
            }
            return result.Output;
        }


        public List<VectorDBEntry> VectorSearch(ChatRequestContext chatRequestContext, string query) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;

            // QueryRequestを作成
            QueryRequest queryRequest = new(query);
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                QueryRequestInstance = queryRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.VectorSearchExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.VectorSearchRequest}:{query}");

            // vector_search
            // VectorSearchResultのリストを作成
            List<VectorDBEntry> vectorSearchResults = [];

            // PostAsyncを実行する
            string endpoint = $"{this.base_url}/vector_search";
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
            // resultStringをログに出力
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.Response}:{resultString}");
            // resultStringからDictionaryに変換する。
            Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, jsonSerializerOptions);
            if (resultDict == null) {
                throw new Exception(StringResources.OpenAIResponseEmpty);
            }
            // documentsがある場合は取得
            if (resultDict.ContainsKey("documents")) {
                JsonElement? documentsObject = (JsonElement)resultDict["documents"];
                // List<VectorSearchResult>に変換
                vectorSearchResults = VectorDBEntry.FromJson(documentsObject.ToString() ?? "[]");
            }

            return vectorSearchResults;

        }

        private void ExecutePythonScriptWrapper(string function_name, Func<dynamic, string> pythonFunction, PythonScriptResult result) {

        }

        public void UpdateVectorDBCollection(ChatRequestContext chatRequestContext) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;
            // ChatRequestContextをJSON文字列に変換
            string chatRequestContextJson = chatRequestContext.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBCollectionExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // update_collection
            PythonScriptResult result = new();
            try {
                // PostAsyncを実行する
                string endpoint = $"{this.base_url}/update_collection";
                string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

                LogWrapper.Debug(resultString);
                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    LogWrapper.Error(result.Error);
                }
            } catch (PythonException e) {
                // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                if (e.Message.Contains("Unsupported file type")) {
                    throw new UnsupportedFileTypeException(e.Message);
                }
                LogWrapper.Error($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        public string GetVectorDBDescription(string catalogDBURL, string vectorDBURL, string collectionName, string folderId) {

            // CatalogRequestを作成
            CatalogRequest catalogRequest = new(catalogDBURL, vectorDBURL, collectionName, folderId);
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                CatalogRequestInstance = catalogRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            // get_catalog_description
            PythonScriptResult result = new();
            // PostAsyncを実行する
            try {

                string endpoint = $"{this.base_url}/get_catalog_description";
                string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

                LogWrapper.Debug(resultString);
                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    LogWrapper.Error(result.Error);
                }
                return result.Output;

            } catch (PythonException e) {
                // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                if (e.Message.Contains("Unsupported file type")) {
                    throw new UnsupportedFileTypeException(e.Message);
                }
                LogWrapper.Error($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        public string UpdateVectorDBDescription(string catalogDBURL, string vectorDBURL, string collectionName, string folderId, string description) {

            // CatalogRequestを作成
            CatalogRequest catalogRequest = new(catalogDBURL, vectorDBURL, collectionName, folderId) {
                Description = description
            };
            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                CatalogRequestInstance = catalogRequest
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBDescription);
            // update_catalog_description
            PythonScriptResult result = new();
            try {
                // PostAsyncを実行する
                string endpoint = $"{this.base_url}/update_catalog_description";
                string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

                LogWrapper.Debug(resultString);
                // resultStringからDictionaryに変換する。
                result.LoadFromJson(resultString);
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (!string.IsNullOrEmpty(result.Error)) {
                    LogWrapper.Error(result.Error);
                }

                return result.Output;
            } catch (PythonException e) {
                // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                if (e.Message.Contains("Unsupported file type")) {
                    throw new UnsupportedFileTypeException(e.Message);
                }
                LogWrapper.Error($"{e.Message}\n{e.StackTrace}");
                throw;
            }
        }

        // 指定されたベクトルDBのコレクションを削除する
        public void DeleteVectorDBCollection(ChatRequestContext chatRequestContext) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteVectorDBCollectionExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // DeleteVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper("delete_collection", (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);
        }

        private void UpdateEmbeddings(ChatRequestContext chatRequestContext, string function_name) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBIndexExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // UpdateVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper(function_name, (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);

        }

        public void DeleteEmbeddings(ChatRequestContext chatRequestContext) {

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBIndexExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // delete_embeddings
            // endpointを作成
            string endpoint = $"{this.base_url}/delete_embeddings";
            // PostAsyncを実行する
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;

        }

        public void UpdateEmbeddings(ChatRequestContext chatRequestContext) {

            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;

            // RequestContainerを作成
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext
            };
            // RequestContainerをJSON文字列に変換
            string chatRequestContextJson = requestContainer.ToJson();


            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBIndexExecute);
            LogWrapper.Debug($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // update_embeddings
            // endpointを作成
            string endpoint = $"{this.base_url}/update_embeddings";
            // PostAsyncを実行する
            string resultString = PostAsync(endpoint, chatRequestContextJson).Result;
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
                LogWrapper.Error(result.Error);
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
                LogWrapper.Error(result.Error);
            }
            return result.Output;
        }


    }
}

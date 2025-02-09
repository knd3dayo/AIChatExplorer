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

    public class PythonNetFunctions : IPythonAIFunctions {

        private readonly Dictionary<string, PyModule> PythonModules = [];

        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        public PyModule GetPyModule(string scriptPath) {
            if (PythonModules.TryGetValue(scriptPath, out PyModule? value)) {
                return value;
            }

            PyModule pyModule = Py.CreateScope();
            string script = PythonExecutor.LoadPythonScript(scriptPath);
            pyModule.Exec(script);
            // PythonModulesに、scriptPathが存在しない場合は追加
            PythonModules[scriptPath] = pyModule;

            return pyModule;
        }

        public object ExecPythonScript(string scriptPath, Func<PyModule, object> action) {
            // Pythonスクリプトを実行する
            using (Py.GIL()) {
                // scriptPathからPyModuleを取得
                PyModule pyModule = GetPyModule(scriptPath);
                return action(pyModule);
            }
        }

        public dynamic GetPythonFunction(PyModule ps, string function_name) {
            // Pythonスクリプトの関数を呼び出す
            dynamic? function_object = (ps?.Get(function_name))
                ?? throw new Exception(StringResources.FunctionNotFound(function_name));
            return function_object;
        }

        public static string CreatePythonExceptionMessage(PythonException e) {
            string pythonErrorMessage = e.Message;
            string message = StringResources.PythonExecuteError + "\n";
            if (pythonErrorMessage.Contains("No module named")) {
                message += StringResources.ModuleNotFound + "\n";
            }
            message += StringResources.PythonExecuteErrorDetail(e);
            return message;
        }

        // テスト用
        public string HelloWorld() {

            // Pythonスクリプトを実行する
            string result = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "hello_world";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // hello_world関数を呼び出す
                PyIterable iterator = function_object();
                // iteratorから文字列を取得
                string result = "";
                foreach (PyObject item in iterator) {
                    result += item.ToString();
                }
                return result ?? "";
            });
            return result;
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");

            long totalTokens = 0;
            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "get_token_count");

                // get_token_count関数を呼び出す
                string resultString = function_object(chatRequestContextJson);
                return resultString;
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            //OpenAIChatExecuteを呼び出す
            ChatResult result = OpenAIChatExecute("openai_chat", (function_object) => {
                return function_object(requestContextJson);
            });
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(result.TotalTokens, chatRequestContext.OpenAIProperties.OpenAICompletionModel);
            return result;
        }

        public IEnumerable<ChatResult> OpenAIChatBatch(List<(ChatRequestContext, ChatRequest)> requests) {
            throw new System.NotImplementedException();
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
                AutogenRequestInstance = new(inputText)
            };
            // RequestContainerをJSON文字列に変換
            string requestContextJson = requestContainer.ToJson();


            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {requestContextJson}");

            ChatResult chatResult = new();

            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "5autogen_group_chat";
                dynamic function_object = GetPythonFunction(ps, function_name);

                // hello_world関数を呼び出す
                PyIterable iterator = function_object(requestContextJson);
                Dictionary<string, dynamic?> resultDict = [];
                // iteratorから文字列を取得
                foreach (PyObject item in iterator) {
                    string itemString = item.ToString() ?? "{}";
                    resultDict = JsonUtil.ParseJson(itemString);
                    // messageを取得
                    if (resultDict.TryGetValue("message", out dynamic? messageValue)) {
                        iteration(messageValue);
                    }
                    // logを取得
                    if (resultDict.TryGetValue("log", out dynamic? logValue)) {
                        LogWrapper.Info(logValue);
                    }
                }
                // total_tokensを取得.
                if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                    if (totalTokensValue is decimal totalTokens) {
                        chatResult.TotalTokens = decimal.ToInt64(totalTokens);
                    }
                }
                // Errorがある場合はLogWrapper.Errorを呼び出す
                if (resultDict.TryGetValue("error", out dynamic? errorValue)) {
                    LogWrapper.Error(errorValue?.ToString());
                }
                return resultDict;
            });
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
            string resultString = (string) ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text_from_file";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text関数を呼び出す
                try {
                    string resultString = function_object(chatRequestContextJson);
                    return resultString;

                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw;
                }
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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

            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_base64_to_text";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text関数を呼び出す
                try {
                    string resultString = function_object(chatRequestContextJson);
                    return resultString;
                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw;
                }
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");

            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                LogWrapper.Error(result.Error);
            }
            return result.Output;
        }

        private ChatResult OpenAIChatExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // ChatResultを作成
            ChatResult chatResult = new();
            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                // run_openai_chat関数を呼び出す。戻り値は{ "output": "レスポンス" , "log": "ログ" }の形式のJSON文字列
                string resultString = pythonFunction(function_object);
                return resultString;
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");

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
            return chatResult;
        }


        private List<VectorDBEntry> VectorSearchExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // VectorSearchResultのリストを作成
            List<VectorDBEntry> vectorSearchResults = [];

            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                string resultString = pythonFunction(function_object);
                return resultString;
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.VectorSearchRequest}:{query}");

            // VectorSearch関数を呼び出す
            return VectorSearchExecute("vector_search", (function_object) => {
                string resultString = function_object(chatRequestContextJson);
                return resultString;
            });
        }

        private void ExecutePythonScriptWrapper(string function_name, Func<dynamic, string> pythonFunction, PythonScriptResult result) {

            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);
                // update_vector_db_index関数を呼び出す
                try {
                    string resultString = pythonFunction(function_object);
                    return resultString;

                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    LogWrapper.Error($"{e.Message}\n{e.StackTrace}");
                    throw;
                }
            });
            LogWrapper.Info(resultString);
            // resultStringからDictionaryに変換する。
            result.LoadFromJson(resultString);
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (!string.IsNullOrEmpty(result.Error)) {
                LogWrapper.Error(result.Error);
            }
        }

        public void UpdateVectorDBCollection(ChatRequestContext chatRequestContext) {
            // ベクトルDB更新処理用にUseVectorDB=Trueに設定
            chatRequestContext.UseVectorDB = true;
            // ChatRequestContextをJSON文字列に変換
            string chatRequestContextJson = chatRequestContext.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBCollectionExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // DeleteVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper("update_collection", (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);
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
            // DeleteVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper("get_catalog_description", (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);

            return result.Output;
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
            // DeleteVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper("update_catalog_description", (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);

            return result.Output;
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo} {chatRequestContextJson}");
            // UpdateVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper(function_name, (function_object) => {
                return function_object(chatRequestContextJson);
            }, result);

        }

        public void DeleteEmbeddings(ChatRequestContext chatRequestContext) {

            string function_name = "delete_embeddings";
            UpdateEmbeddings(chatRequestContext, function_name);

        }

        public void UpdateEmbeddings(ChatRequestContext chatRequestContext) {

            string function_name;
            function_name = "update_embeddings";
            UpdateEmbeddings(chatRequestContext, function_name);
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
            LogWrapper.Info($"{PythonAILibStringResources.Instance.RequestInfo}:{chatRequestContextJson}");

            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "export_to_excel");
                // export_to_excel関数を呼び出す
                function_object(chatRequestContextJson);
                return true;
            });
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
            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "import_from_excel");
                // import_from_excel関数を呼び出す
                string resultString = function_object(chatRequestContextJson);
                return resultString;

            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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

            string function_name = "get_mime_type";

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

            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                resultString = function_object(chatRequestContextJson);
                return resultString;

            });
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
            // Pythonスクリプトを実行する
            string resultString = (string)ExecPythonScript(PythonExecutor.OpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "extract_webpage");
                // extract_webpage関数を呼び出す
                string resultString = function_object(chatRequestContextJson);
                return resultString;
            });
            // resultStringをログに出力
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");
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

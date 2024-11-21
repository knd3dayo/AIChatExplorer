using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Python.Runtime;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Statistics;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
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

        public void ExecPythonScript(string scriptPath, Action<PyModule> action) {
            // Pythonスクリプトを実行する
            using (Py.GIL()) {
                // scriptPathからPyModuleを取得
                PyModule pyModule = GetPyModule(scriptPath);
                action(pyModule);
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

        // IPythonFunctionsのメソッドを実装
        public string ExtractFileToText(string path) {
            // ResultContainerを作成
            string result = "";

            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text_from_file";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text関数を呼び出す
                try {
                    result = function_object(path);
                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw;
                }
            });
            return result;
        }


        // テスト用
        public string HelloWorld() {
            string result = "";
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "hello_world";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // hello_world関数を呼び出す
                PyIterable iterator = function_object();
                // iteratorから文字列を取得
                foreach (PyObject item in iterator) {
                    result += item.ToString();
                }

            });
            return result;
        }




        public string ExtractBase64ToText(string base64, string extenstion) {

            // ResultContainerを作成
            string result = "";

            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_base64_to_text";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text関数を呼び出す
                try {
                    result = function_object(base64, extenstion);
                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw;
                }
            });
            return result;
        }

        public void OpenAIEmbedding(OpenAIProperties props, string text) {

            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "openai_embedding";
                dynamic function_object = GetPythonFunction(ps, function_name);
                string propsJson = props.ToJson();

                LogWrapper.Info(PythonAILibStringResources.Instance.EmbeddingExecute);
                LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propsJson}");
                LogWrapper.Info($"{PythonAILibStringResources.Instance.Text}:{text}");

                // open_ai_chat関数を呼び出す
                function_object(text, propsJson);
                // System.Windows.MessageBox.Show(result);
            });
        }

        private ChatResult OpenAIChatExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // ChatResultを作成
            ChatResult chatResult = new();
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                // run_openai_chat関数を呼び出す。戻り値は{ "output": "レスポンス" , "log": "ログ" }の形式のJSON文字列
                string resultString = pythonFunction(function_object);

                // resultStringをログに出力
                LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");

                // JSON文字列からDictionaryに変換する。
                Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, jsonSerializerOptions) ?? throw new Exception(StringResources.OpenAIResponseEmpty);
                // contentを取得
                if (resultDict.TryGetValue("output", out dynamic? outputValue)) {
                    string? output = outputValue.ToString();
                    // ChatResultに設定
                    chatResult.Output = output ?? "";
                }
                // total_tokensを取得
                if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                    // JsonElementである、totalTokensValueを、longに変換
                    long totalTokens = totalTokensValue?.GetInt64() ?? 0;
                    chatResult.TotalTokens = totalTokens;
                }

            });
            return chatResult;

        }

        // 通常のOpenAIChatを実行する
        public ChatResult OpenAIChat(OpenAIProperties props, ChatRequest chatRequest) {

            // ChatHistoryとContentなどからリクエストを作成
            Dictionary<string, object> chatRequestDict = chatRequest.ToDict();
            // ChatRequestをJSON文字列に変換
            string chat_history_json = JsonSerializer.Serialize(chatRequestDict, jsonSerializerOptions);
            string propsJson = props.ToJson();

            LogWrapper.Info(PythonAILibStringResources.Instance.OpenAIExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propsJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.ChatHistory}:{chat_history_json}");

            //OpenAIChatExecuteを呼び出す
            ChatResult result = OpenAIChatExecute("run_openai_chat", (function_object) => {
                return function_object(propsJson, chat_history_json);
            });
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(result.TotalTokens, props.OpenAICompletionModel);

            return result;

        }

        private ChatResult LangChainChatExecute(string functionName, Func<dynamic, string> pythonFunction) {
            ChatResult chatResult = new();

            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                string function_name = functionName;
                dynamic function_object = GetPythonFunction(ps, function_name);

                string resultString = pythonFunction(function_object);

                // resultStringをログに出力
                LogWrapper.Info($"{PythonAILibStringResources.Instance.Response}:{resultString}");

                // JSON文字列からDictionaryに変換する。
                Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(resultString, jsonSerializerOptions) ?? throw new Exception(StringResources.OpenAIResponseEmpty);
                // outputを取得
                if (resultDict.TryGetValue("output", out dynamic? outputValue)) {
                    string? output = outputValue.ToString();
                    // ChatResultに設定
                    chatResult.Output = output ?? "";
                }
                // page_content_listを取得
                if (resultDict.TryGetValue("page_content_list", out dynamic? pageContentListValue)) {
                    chatResult.PageContentList = pageContentListValue ?? new List<Dictionary<string, string>>();
                }
                // page_source_listを取得
                if (resultDict.TryGetValue("page_source_list", out dynamic? pageSourceListValue)) {
                    chatResult.PageSourceList = pageSourceListValue ?? new List<Dictionary<string, string>>();
                }
                // total_tokensを取得.
                if (resultDict.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                    // JsonElementである、totalTokensValueを、longに変換
                    long totalTokens = totalTokensValue?.GetInt64() ?? 0;
                    chatResult.TotalTokens = totalTokens;
                }

            });
            return chatResult;

        }
        public ChatResult LangChainChat(OpenAIProperties openAIProperties, ChatRequest chatRequest) {

            string prompt = chatRequest.CreatePromptText();
            Dictionary<string, object> chatRequestDict = chatRequest.ToDict();
            // ChatRequestをJSON文字列に変換
            string chatHistoryJson = JsonSerializer.Serialize(chatRequestDict, jsonSerializerOptions);
            // Pythonスクリプトの関数を呼び出す
            ChatResult chatResult = new();

            // propsをJSON文字列に変換
            string propsJson = openAIProperties.ToJson();

            // vectorDBItemsをJSON文字列に変換
            string vectorDBItemsJson = VectorDBItem.ToJson(chatRequest.VectorDBItems);

            LogWrapper.Info(PythonAILibStringResources.Instance.LangChainExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propsJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Prompt}:{prompt}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.ChatHistory}:{chatHistoryJson}");

            // LangChainChat関数を呼び出す
            chatResult = LangChainChatExecute("run_langchain_chat", (function_object) => {
                return function_object(propsJson, vectorDBItemsJson, chatHistoryJson);
            });
            // StatisticManagerにトークン数を追加
            MainStatistics.GetMainStatistics().AddTodayTokens(chatResult.TotalTokens, openAIProperties.OpenAICompletionModel);
            return chatResult;
        }

        // AutoGenのGroupChatを実行する
        public ChatResult AutoGenGroupChat(OpenAIProperties openAIProperties,ChatRequest chatRequest,  Action<string> iteration) {
            // propsをJSON文字列に変換
            string propsJson = openAIProperties.ToJson();
            // vectorDBItemsをJSON文字列に変換
            string vectorDBItemsJson = VectorDBItem.ToJson(chatRequest.VectorDBItems);
            // workDirを取得
            string workDir = chatRequest.WorkDir;
            // messageを取得
            string message = chatRequest.CreatePromptText();

            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propsJson}");

            ChatResult chatResult = new();
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "run_autogen_group_chat";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // hello_world関数を呼び出す
                PyIterable iterator = function_object(propsJson, vectorDBItemsJson, workDir, message);
                Dictionary<string, dynamic?> keyValuePairs = new();
                // iteratorから文字列を取得
                foreach (PyObject item in iterator) {
                    string itemString = item.ToString() ?? "{}";
                    keyValuePairs = JsonUtil.ParseJson(itemString);
                    // messageを取得
                    if (keyValuePairs.TryGetValue("message", out dynamic? messageValue)) {
                        iteration(messageValue);
                    }
                    // logを取得
                    if (keyValuePairs.TryGetValue("log", out dynamic? logValue)) {
                        LogWrapper.Info(logValue);
                    }
                }
                // 最終処理
                // page_content_listを取得
                if (keyValuePairs.TryGetValue("page_content_list", out dynamic? pageContentListValue)) {
                    chatResult.PageContentList = pageContentListValue ?? new List<Dictionary<string, string>>();
                }
                // page_source_listを取得
                if (keyValuePairs.TryGetValue("page_source_list", out dynamic? pageSourceListValue)) {
                    chatResult.PageSourceList = pageSourceListValue ?? new List<Dictionary<string, string>>();
                }
                // total_tokensを取得.
                if (keyValuePairs.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                    // JsonElementである、totalTokensValueを、longに変換
                    long totalTokens = totalTokensValue?.GetInt64() ?? 0;
                    chatResult.TotalTokens = totalTokens;
                }
            });
            return chatResult;

        }

        private List<VectorSearchResult> VectorSearchExecute(string function_name, Func<dynamic, string> pythonFunction) {
            // VectorSearchResultのリストを作成
            List<VectorSearchResult> vectorSearchResults = [];

            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                string resultString = pythonFunction(function_object);

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
                    vectorSearchResults = VectorSearchResult.FromJson(documentsObject.ToString() ?? "[]");
                }

            });
            return vectorSearchResults;
        }

        public List<VectorSearchResult> VectorSearch(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, VectorSearchRequest vectorSearchRequest) {
            // propsをJSON文字列に変換
            string propsJson = openAIProperties.ToJson();
            // vectorDBItemsをJSON文字列に変換
            string vectorDBItemsJson = VectorDBItem.ToJson(vectorDBItems);
            // vectorSearchRequestをJSON文字列に変換
            string vectorSearchRequestJson = vectorSearchRequest.ToJson();
            LogWrapper.Info(PythonAILibStringResources.Instance.VectorSearchExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propsJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.VectorDBItems}:{vectorDBItemsJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.VectorSearchRequest}:{vectorSearchRequestJson}");

            // VectorSearch関数を呼び出す
            return VectorSearchExecute("vector_search", (function_object) => {
                string resultString = function_object(propsJson, vectorDBItemsJson, vectorSearchRequestJson);
                return resultString;
            });
        }

        private void ExecutePythonScriptWrapper(string function_name, Func<dynamic, string> pythonFunction, PythonScriptResult result) {

            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);
                // update_vector_db_index関数を呼び出す
                try {
                    string resultString = pythonFunction(function_object);
                    LogWrapper.Info(resultString);
                    // resultStringからDictionaryに変換する。
                    result.LoadFromJson(resultString);

                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    LogWrapper.Error($"{e.Message}\n{e.StackTrace}");
                }
            });
        }
        // 指定されたベクトルDBのインデックスを削除する
        public void DeleteVectorDBCollection(OpenAIProperties props, VectorDBItem vectorDBItem) {
            // OpenAIPropertiesをJSON文字列に変換
            string propJson = props.ToJson();
            // VectorDBItemをJSON文字列に変換
            string vectorDBItemJson = VectorDBItem.ToJson([vectorDBItem]);

            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteVectorDBCollectionExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propJson}");
            // DeleteVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper("delete_collection", (function_object) => {
                return function_object(propJson, vectorDBItemJson);
            }, result);
        }
        public void UpdateVectorDBIndex(OpenAIProperties props, VectorDBItem vectorDBItem, string vectorTargetJson, string function_name) {

            // OpenAIPropertiesをJSON文字列に変換
            string propJson = props.ToJson();
            // VectorDBItemをJSON文字列に変換
            string vectorDBItemJson = VectorDBItem.ToJson([vectorDBItem]);

            LogWrapper.Info(PythonAILibStringResources.Instance.UpdateVectorDBIndexExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo} {propJson}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.PropertyInfo}:{vectorTargetJson}");
            // UpdateVectorDBIndexExecuteを呼び出す
            PythonScriptResult result = new();
            ExecutePythonScriptWrapper(function_name, (function_object) => {
                return function_object(propJson, vectorDBItemJson, vectorTargetJson);
            }, result);

        }

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItem vectorDBItem) {
            string function_name;
            if (contentInfo.Mode == VectorDBUpdateMode.update) {
                function_name = "update_content_index";
            } else if (contentInfo.Mode == VectorDBUpdateMode.delete) {
                function_name = "delete_index";
            } else {
                throw new Exception(PythonAILibStringResources.Instance.InvalidMode);
            }
            // contentInfoをJSON文字列に変換
            string contentInfoJson = contentInfo.ToJson();
            UpdateVectorDBIndex(props, vectorDBItem, contentInfoJson, function_name);
        }


        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItem vectorDBItem) {
            string function_name = "";
            if (imageInfo.Mode == VectorDBUpdateMode.update) {
                function_name = "update_content_index";
            } else if (imageInfo.Mode == VectorDBUpdateMode.delete) {
                function_name = "delete_index";
            } else {
                throw new Exception(PythonAILibStringResources.Instance.InvalidMode);
            }
            // imageInfoをJSON文字列に変換
            string imageInfoJson = imageInfo.ToJson();
            UpdateVectorDBIndex(props, vectorDBItem, imageInfoJson, function_name);
        }


        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem) {

            // workingDirPathとFileStatusのPathを結合する。ファイルが存在しない場合は例外をスロー
            if (File.Exists(gitFileInfo.AbsolutePath)) {
                LogWrapper.Info($"{StringResources.FileNotFound} : {gitFileInfo.AbsolutePath}");
                throw new FileNotFoundException(StringResources.FileNotFound + gitFileInfo.AbsolutePath);
            }
            string function_name = "";
            if (gitFileInfo.Mode == VectorDBUpdateMode.update) {
                function_name = "update_file_index";
            } else if (gitFileInfo.Mode == VectorDBUpdateMode.delete) {
                function_name = "delete_index";
            } else {
                throw new Exception(PythonAILibStringResources.Instance.InvalidMode);
            }
            // gitFileInfoをJSON文字列に変換
            string gitFileInfoJson = gitFileInfo.ToJson();
            UpdateVectorDBIndex(props, vectorDBItem, gitFileInfoJson, function_name);
        }

        // ExportToExcelを実行する
        public void ExportToExcel(string filePath, CommonDataTable data) {
            // dataをJSON文字列に変換
            string dataJson = CommonDataTable.ToJson(data);
            LogWrapper.Info(PythonAILibStringResources.Instance.ExportToExcelExecute);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.FilePath}:{filePath}");
            LogWrapper.Info($"{PythonAILibStringResources.Instance.Data}:{dataJson}");

            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "export_to_excel");
                // export_to_excel関数を呼び出す
                function_object(filePath, dataJson);
            });
        }

        // ImportFromExcelを実行する
        public CommonDataTable ImportFromExcel(string filePath) {
            // ResultContainerを作成
            CommonDataTable result = new([]);
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, "import_from_excel");
                // import_from_excel関数を呼び出す
                string resultString = function_object(filePath);

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
            });
            return result;
        }

        // GetMimeType
        public string GetMimeType(string filePath) {

            string function_name = "get_mime_type";
            string? contentType = "";
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonOpenAIScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic function_object = GetPythonFunction(ps, function_name);

                contentType = function_object(filePath);

            });
            return contentType ?? "";
        }
    }
}

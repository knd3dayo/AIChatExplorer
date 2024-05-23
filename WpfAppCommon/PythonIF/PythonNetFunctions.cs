using System.Drawing;
using System.IO;
using Python.Runtime;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.PythonIF {
    public enum SpacyEntityNames {

        PERSON,
        ORG,
        GPE,
        LOC,
        PRODUCT,
        EVENT,
        WORK_OF_ART,
        LAW,
        LANGUAGE,
        DATE,
        TIME,
        PERCENT,
        MONEY,
        QUANTITY,
        ORDINAL,
        CARDINAL
    }
}

namespace WpfAppCommon.PythonIF {
    public class PythonTask(Action action) : Task(action) {

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    }
    public class PythonNetFunctions : IPythonFunctions {

        private readonly Dictionary<string, PyModule> PythonModules = [];

        private static StringResources StringResources { get; } = StringResources.Instance;

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

        private void InitPythonNet(string pythonDLLPath) {
            // Pythonスクリプトを実行するための準備

            // 既に初期化されている場合は初期化しない
            if (PythonEngine.IsInitialized) {
                return;
            }

            // PythonDLLのパスを設定
            Runtime.PythonDLL = pythonDLLPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL)) {
                string message = StringResources.PythonDLLNotFound;
                Tools.Error(message + Runtime.PythonDLL);
                return;
            }

            try {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

            } catch (TypeInitializationException e) {
                string message = StringResources.PythonInitFailed + e.Message;
                Tools.Error(message);
            }
        }

        public PythonNetFunctions(string pythonDLLPath) {
            InitPythonNet(pythonDLLPath);
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
                ?? throw new ThisApplicationException(StringResources.FunctionNotFound(function_name));
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
        public string ExtractText(string path) {
            // ResultContainerを作成
            string result = "";

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text";
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

        // IPythonFunctionsのメソッドを実装
        // データをマスキングする
        public string GetMaskedString(string SpacyModel, string text) {
            List<string> beforeTextList = [text];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        // マスキングされたデータを元に戻す
        public string GetUnmaskedString(string SpacyModel, string maskedText) {
            List<string> beforeTextList = [maskedText];
            MaskedData maskedData = GetMaskedData(SpacyModel, beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        public MaskedData GetMaskedData(string SpacyModel, List<string> beforeTextList) {

            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SpacyModel)) {
                throw new ThisApplicationException(StringResources.SpacyModelNameNotSet);
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };

            MaskedData actionResult = new(beforeTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "mask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(beforeTextList, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ThisApplicationException(StringResources.MaskingResultNotFound);
                }
                PyObject? textDictObject = resultDict.GetItem("TEXT") ?? throw new ThisApplicationException(StringResources.MaskingResultFailed);

                PyDict textDict = textDictObject.As<PyDict>();
                PyList? afterList = textDict.GetItem("AFTER").As<PyList>();
                foreach (PyObject item in afterList) {
                    string? text = item.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities;
                    try {
                        entities = resultDict.GetItem(entityNameString);
                    } catch (PythonException) {
                        entities = null;
                        return;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }

        // GetUnMaskedDataの実装
        public MaskedData GetUnMaskedData(string SpacyModel, List<string> maskedTextList) {

            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
            MaskedData actionResult = new(maskedTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "unmask_data";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = function_object(actionResult, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ThisApplicationException(StringResources.UnmaskingResultNotFound);
                }

                PyObject? textListObject = resultDict.GetItem("TEXT") ?? throw new ThisApplicationException(StringResources.UnmaskingResultFailed);
                PyList textList = textListObject.As<PyList>();
                foreach (PyObject item in textList) {
                    PyObject afterTextObject = item.GetItem("AFTER");
                    string? text = afterTextObject.ToString();
                    if (text == null) {
                        continue;
                    }
                    actionResult.AfterTextList.Add(text);
                }
                // SpacyEntitiesNames毎に処理
                foreach (SpacyEntityNames entityName in Enum.GetValues(typeof(SpacyEntityNames))) {
                    string entityNameString = entityName.ToString();
                    PyObject? entities = resultDict.GetItem(entityNameString);
                    if (entities == null) {
                        continue;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

            });
            return actionResult;
        }
        public string ExtractTextFromImage(Image image, string tesseractExePath) {
            // Pythonスクリプトを実行する
            string result = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_text_from_image";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // extract_text_from_image関数を呼び出す
                ImageConverter imageConverter = new();
                object? bytesObject = imageConverter.ConvertTo(image, typeof(byte[]))
                ?? throw new ThisApplicationException(StringResources.ImageByteFailed);
                byte[] bytes = (byte[])bytesObject;
                result = function_object(bytes, tesseractExePath);
            });
            return result;
        }

        private List<MaskedEntity> GetMaskedEntities(string label, PyDict pyDict) {

            List<MaskedEntity> result = [];
            foreach (var key in pyDict.Keys()) {
                PyObject? entity = pyDict.GetItem(key);
                if (entity == null) {
                    continue;
                }
                string? keyString = key.ToString();
                if (keyString == null) {
                    continue;
                }
                string? entityString = entity.ToString();
                if (entityString == null) {
                    continue;
                }
                MaskedEntity maskedEntity = new() {
                    Label = label,
                    Before = keyString,
                    After = entityString
                };
                result.Add(maskedEntity);
            }
            return result;
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory, IEnumerable<VectorDBItem> vectorDBItems) {
            return LangChainChat(prompt, chatHistory, ClipboardAppConfig.CreateOpenAIProperties(), vectorDBItems);
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems) {
            // Pythonスクリプトの関数を呼び出す
            ChatResult chatResult = new();

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                string function_name = "langchain_chat";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // chatHistoryをJSON文字列に変換
                string chatItemsJSon = ChatItem.ToJson(chatHistory);
                // VectorDBItemsのサイズが0の場合は例外をスロー
                if (!vectorDBItems.Any()) {
                    throw new ThisApplicationException(StringResources.VectorDBItemsEmpty);
                }
                // VectorDBItemのリストをJSON文字列に変換
                string vectorDBItemsJson = VectorDBItem.ToJson(vectorDBItems);

                Tools.Info("LangChain実行");
                Tools.Info($"プロンプト:{prompt}");
                // open_ai_chat関数を呼び出す
                PyDict pyDict = function_object(props, vectorDBItemsJson, prompt, chatItemsJSon);
                // outputを取得
                string? resultString = pyDict["output"].ToString() ?? throw new ThisApplicationException(StringResources.OpenAIResponseEmpty);
                // verboseを取得
                string? verbose = pyDict.GetItem("verbose")?.ToString();
                if (verbose != null) {
                    chatResult.Verbose = verbose;
                    Tools.Info($"verbose:{verbose}");
                }
                // logを取得
                string? log = pyDict.GetItem("log")?.ToString();
                if (log != null) {
                    Tools.Info($"log:{log}");
                }
                // referenced_contentsを取得
                PyList? referencedContents = pyDict.GetItem("page_content_list") as PyList;
                if (referencedContents != null) {
                    List<Dictionary<string, string>> referencedContentsList = [];
                    foreach (PyDict item in referencedContents.Cast<PyDict>()) {
                        Dictionary<string, string> dict = [];
                        foreach (var key in item.Keys()) {
                            PyObject? entity = item.GetItem(key);
                            if (entity == null) {
                                continue;
                            }
                            string? keyString = key.ToString();
                            if (keyString == null) {
                                continue;
                            }
                            string? entityString = entity.ToString();
                            if (entityString == null) {
                                continue;
                            }
                            dict[keyString] = entityString;
                        }
                        referencedContentsList.Add(dict);
                    }
                    chatResult.ReferencedContents = referencedContentsList;
                }
                // referenced_file_pathを取得
                PyList? referencedFilePath = pyDict.GetItem("page_source_list") as PyList;
                if (referencedFilePath != null) {
                    List<string> referencedFilePathList = [];
                    foreach (PyObject item in referencedFilePath) {
                        string? itemString = item.ToString();
                        if (itemString == null) {
                            continue;
                        }
                        referencedFilePathList.Add(itemString);
                    }
                    chatResult.ReferencedFilePath = referencedFilePathList;


                }
                // ChatResultに設定
                chatResult.Response = resultString;
                Tools.Info($"レスポンス:{resultString}");
            });
            return chatResult;

        }
        public void OpenAIEmbedding(string text) {

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "openai_embedding";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // open_ai_chat関数を呼び出す
                function_object(text, WpfAppCommon.Model.ClipboardAppConfig.CreateOpenAIProperties());
                // System.Windows.MessageBox.Show(result);
            });
        }

        // IPythonFunctionsのメソッドを実装
        // スクリプトの内容とJSON文字列を引数に取り、結果となるJSON文字列を返す
        public string RunScript(string script, string input) {
            string resultString = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                string function_name = "run_script";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // run_script関数を呼び出す
                resultString = function_object(script, input);
            });
            return resultString;

        }

        // IPythonFunctionsのメソッドを実装
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {

            HashSet<string> actionResult = [];
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // SPACY_MODEL_NAMEが空の場合は例外をスロー
                if (string.IsNullOrEmpty(SpacyModel)) {
                    throw new ThisApplicationException(StringResources.SpacyModelNameNotSet);
                }

                Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
                // 結果用のDictionaryを作成
                // Pythonスクリプトの関数を呼び出す
                string function_name = "extract_entity";
                dynamic function_object = GetPythonFunction(ps, function_name);
                PyIterable pyIterable = function_object(text, dict);
                // PythonのリストをC#のHashSetに変換
                foreach (PyObject item in pyIterable) {
                    string? entity = item.ToString();
                    if (entity != null) {
                        actionResult.Add(entity);
                    }
                }
            });
            return actionResult;
        }

        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            return OpenAIChat(prompt, chatHistory, ClipboardAppConfig.CreateOpenAIProperties());
        }

        // 通常のOpenAIChatを実行する
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {
            // ChatResultを作成
            ChatResult chatResult = new();
            // promptからChatItemを作成
            ChatItem chatItem = new(ChatItem.UserRole, prompt);
            // chatHistoryをコピーしてChatItemを追加
            List<ChatItem> chatHistoryList = new(chatHistory) {
                chatItem
            };
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "openai_chat";
                dynamic function_object = GetPythonFunction(ps, function_name);
                string json_string = ChatItem.ToJson(chatHistoryList);
                Tools.Info("OpenAI実行");
                Tools.Info($"プロンプト:{prompt}");
                // open_ai_chat関数を呼び出す
                string resultString = function_object(props, json_string);
                // ChatResultに設定
                chatResult.Response = resultString;
                Tools.Info($"レスポンス:{resultString}");

            });
            return chatResult;

        }

        // OpenAIChatWithVisionを実行する
        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames) {
            return OpenAIChatWithVision(prompt, imageFileNames, ClipboardAppConfig.CreateOpenAIProperties());
        }

        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames, Dictionary<string, string> props) {

            // ChatResultを作成
            ChatResult chatResult = new();

            // chatHistoryをコピーしてChatItemを追加
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "openai_chat_with_vision";
                dynamic function_object = GetPythonFunction(ps, function_name);

                // open_ai_chat関数を呼び出す
                string resultString = function_object(props, prompt, imageFileNames);
                // ChatResultに設定
                chatResult.Response = resultString;
            });
            return chatResult;
        }

        // テスト用
        public string HelloWorld() {
            string result = "";
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                string function_name = "hello_world";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // hello_world関数を呼び出す
                result = function_object();

            });
            return result;
        }

        public int UpdateVectorDBIndex(FileStatus fileStatus, string workingDirPath, string repositoryURL, VectorDBItem vectorDBItem) {
            int tokenCount = 0;
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // FileStatus.StatusがAdded、Modifiedのの場合は第1引数に"update"、Deletedの場合は"delete"を渡す
                // それ以外はなにもしない。
                string mode = "";
                if (fileStatus.Status == FileStatusEnum.Added || fileStatus.Status == FileStatusEnum.Modified) {
                    mode = "update";
                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    mode = "delete";
                } else {
                    return;
                }
                // workingDirPathとFileStatusのPathを結合する。ファイルが存在しない場合は例外をスロー
                string filePath = Path.Combine(workingDirPath, fileStatus.Path);
                if (!File.Exists(filePath)) {
                    Tools.Info($"{StringResources.FileNotFound} : {filePath}");
                }
                // propsにVectorDBURLを追加
                var props = ClipboardAppConfig.CreateOpenAIProperties();
                props["VectorDBType"] = vectorDBItem.VectorDBTypeString;
                props["VectorDBURL"] = vectorDBItem.VectorDBURL;

                // Pythonスクリプトの関数を呼び出す
                string function_name = "update_index";
                dynamic function_object = GetPythonFunction(ps, function_name);
                // update_vector_db_index関数を呼び出す
                try {
                    tokenCount = function_object(
                        props,
                        mode,
                        new PyString(workingDirPath),
                        new PyString(fileStatus.Path), repositoryURL);
                } catch (PythonException e) {
                    // エラーメッセージを表示 Unsupported file typeが含まれる場合は例外をスロー
                    if (e.Message.Contains("Unsupported file type")) {
                        throw new UnsupportedFileTypeException(e.Message);
                    }
                    throw   ;
                }
            });
            return tokenCount;
        }


    }
}

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
                string message = "PythonDLLが見つかりません。";
                message += "\n" + "PythonDLLのパスを確認してください:";
                Tools.Error(message + Runtime.PythonDLL);
                return;
            }

            try {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

            } catch (TypeInitializationException e) {
                string message = "Pythonの初期化に失敗しました。" + e.Message;
                message += "\n" + "PythonDLLのパスを確認してください。";
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



        public static string CreatePythonExceptionMessage(PythonException e) {
            string pythonErrorMessage = e.Message;
            string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
            if (pythonErrorMessage.Contains("No module named")) {
                message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
            }
            message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);
            return message;
        }

        // IPythonFunctionsのメソッドを実装
        public string ExtractText(string path) {
            // ResultContainerを作成
            string result = "";

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? extract_text = (ps?.Get("extract_text")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、extract_text関数が見つかりません");
                // extract_text関数を呼び出す
                result = extract_text(path);

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
                throw new ThisApplicationException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };

            MaskedData actionResult = new(beforeTextList);
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                dynamic? mask_data = (ps?.Get("mask_data")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、mask_data関数が見つかりません");
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = mask_data(beforeTextList, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ThisApplicationException("マスキング結果がありません");
                }
                PyObject? textDictObject = resultDict.GetItem("TEXT") ?? throw new ThisApplicationException("マスキングした文字列取得に失敗しました");
                // 
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
                dynamic? unmask_data = (ps?.Get("unmask_data")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、unmask_data関数が見つかりません");
                // 結果用のDictionaryを作成
                PyDict resultDict = new();
                resultDict = unmask_data(actionResult, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ThisApplicationException("マスキング解除結果がありません");
                }
                PyObject? textListObject = resultDict.GetItem("TEXT") ?? throw new ThisApplicationException("マスキング解除した文字列取得に失敗しました");
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
                dynamic? extract_text_from_image = (ps?.Get("extract_text_from_image")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、" +
                    "extract_text_from_image関数が見つかりません");
                // extract_text_from_image関数を呼び出す
                ImageConverter imageConverter = new();
                object? bytesObject = imageConverter.ConvertTo(image, typeof(byte[])) ?? throw new ThisApplicationException("画像のバイト列に変換できません");
                byte[] bytes = (byte[])bytesObject;
                result = extract_text_from_image(bytes, tesseractExePath);
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

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            return LangChainChat(prompt, chatHistory, ClipboardAppConfig.CreateOpenAIProperties());
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {
            // Pythonスクリプトの関数を呼び出す
            ChatResult chatResult = new();

            ExecPythonScript(PythonExecutor.QAChatScript, (ps) => {
                dynamic? langchain_chat = (ps?.Get("langchain_chat")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、openai_chat関数が見つかりません");
                // chatHistoryをJSON文字列に変換
                string json_string = ChatItem.ToJson(chatHistory);

                // open_ai_chat関数を呼び出す
                PyDict pyDict = langchain_chat(props, prompt, json_string);
                // outputを取得
                string? resultString = pyDict["output"].ToString() ?? throw new ThisApplicationException("OpenAIの応答がありません");
                // verboseを取得
                string? verbose = pyDict.GetItem("verbose")?.ToString();
                if (verbose != null) {
                    chatResult.Verbose = verbose;
                }

                // referenced_contentsを取得
                PyList? referencedContents = pyDict.GetItem("page_content_list") as PyList;
                if (referencedContents != null) {
                    List<string> referencedContentsList = [];
                    foreach (PyObject item in referencedContents) {
                        string? itemString = item.ToString();
                        if (itemString == null) {
                            continue;
                        }
                        referencedContentsList.Add(itemString);
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
            });
            return chatResult;

        }
        public void OpenAIEmbedding(string text) {

            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? open_ai_embedding = (ps?.Get("openai_embedding")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、openai_embedding関数が見つかりません");
                // open_ai_chat関数を呼び出す
                open_ai_embedding(text, WpfAppCommon.Model.ClipboardAppConfig.CreateOpenAIProperties());
                // System.Windows.MessageBox.Show(result);
            });
        }

        // IPythonFunctionsのメソッドを実装
        // スクリプトの内容とJSON文字列を引数に取り、結果となるJSON文字列を返す
        public string RunScript(string script, string input) {
            string resultString = "";
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {

                // Pythonスクリプトの関数を呼び出す
                dynamic? run_script = (ps?.Get("run_script")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、run_script関数が見つかりません");
                // run_script関数を呼び出す
                resultString = run_script(script, input);
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
                    throw new ThisApplicationException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
                }

                Dictionary<string, string> dict = new() {
                            { "SpacyModel", SpacyModel }
                        };
                // 結果用のDictionaryを作成
                // Pythonスクリプトの関数を呼び出す
                dynamic? extract_entity = (ps?.Get("extract_entity")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、extract_entity関数が見つかりません");
                PyIterable pyIterable = extract_entity(text, dict);
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

        public void SaveFaissIndex() {
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? save_faiss_index = (ps?.Get("save_faiss_index")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、save_faiss_index関数が見つかりません");
                // save_faiss_index関数を呼び出す
                save_faiss_index();
            });
        }

        public void LoadFaissIndex() {
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? load_faiss_index = (ps?.Get("load_faiss_index")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、load_faiss_index関数が見つかりません");
                // load_faiss_index関数を呼び出す
                load_faiss_index();
            });
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
            ExecPythonScript(PythonExecutor.QAChatScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? openai_chat = (ps?.Get("openai_chat")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、openai_chat関数が見つかりません");
                string json_string = ChatItem.ToJson(chatHistoryList);

                // open_ai_chat関数を呼び出す
                string resultString = openai_chat(props, json_string);
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
                dynamic? HelloWorld = (ps?.Get("hello_world")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、hello_world関数が見つかりません");
                // hello_world関数を呼び出す
                result = HelloWorld();

            });
            return result;
        }

        public int UpdateVectorDBIndex(FileStatus fileStatus, string workingDirPath, string repositoryURL) {
            int tokenCount = 0;
            // Pythonスクリプトを実行する
            ExecPythonScript(PythonExecutor.WpfAppCommonUtilsScript, (ps) => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? update_index = (ps?.Get("update_index")) ?? throw new ThisApplicationException("Pythonスクリプトファイルに、update_vector_db_index関数が見つかりません");
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
                    throw new ThisApplicationException("ファイルが存在しません:" + filePath);
                }
                // update_vector_db_index関数を呼び出す
                tokenCount = update_index(ClipboardAppConfig.CreateOpenAIProperties(), mode, workingDirPath, fileStatus.Path, repositoryURL);
            });
            return tokenCount;
        }


    }
}

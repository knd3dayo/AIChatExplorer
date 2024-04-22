using System.Collections.Concurrent;
using System.Drawing;
using Python.Runtime;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using System.IO;
using QAChat.Model;

namespace ClipboardApp.PythonIF {

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

    public class PythonTask(Action action) : Task(action) {

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    }
    public class PythonNetFunctions : IPythonFunctions {
        private PyModule? ps;
        private BlockingCollection<PythonTask> blockingCollection = new BlockingCollection<PythonTask>();
        private QAChat.PythonIF.PythonNetFunctions QAChatPythonNetFunctions;
        private void InitPythonNet() {
            // Pythonスクリプトを実行するための準備
            // PythonDLLのパスを設定
            Runtime.PythonDLL = Properties.Settings.Default.PythonDllPath;

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

        public PythonNetFunctions() {
            InitPythonNet();
            QAChatPythonNetFunctions = new QAChat.PythonIF.PythonNetFunctions();

            Task consumerThread = new(() => {
                // 初期化エラー時にcommandを受け付けないようにする
                bool initError = false;
                using (Py.GIL()) {

                    try {
                        ps = Py.CreateScope();
                        string script = PythonExecutor.LoadPythonScript(PythonExecutor.ClipboardAppUtilsScript);
                        ps.Exec(script);
                    } catch (PythonException e) {
                        string message = CreatePythonExceptionMessage(e);
                        Tools.Error("Python機能初期化時にエラーが発生しました\n" + message);
                        initError = true;
                    }
                }
                while (true) {
                    if (blockingCollection.IsCompleted) {
                        break;
                    }
                    PythonTask command = blockingCollection.Take();
                    if (initError) {
                        // 初期化エラー時はコマンドをキャンセルする
                        Tools.Warn("Python機能初期化時にエラーが発生したため、Pythonコマンドをキャンセルします");
                        command.CancellationTokenSource.Cancel();
                    } else {
                        try {
                            command.Start();
                        } catch (PythonException e) {
                            string message = CreatePythonExceptionMessage(e);
                            Tools.Error("Pythonコマンド実行時にエラーが発生しました\n" + message);
                        } catch (Exception e) {
                            Tools.Error("Pythonコマンド実行時にエラーが発生しました\n" + e.Message);
                        }
                    }
                }
            });
            consumerThread.Start();
        }

        public void PostPythonAction(Action action) {
            // Pythonスクリプトを実行する
            PythonTask command = new PythonTask(() => {
                using (Py.GIL()) {
                    action();
                }
            });
            // CancellationTokenを設定
            CancellationToken token = command.CancellationTokenSource.Token;
            blockingCollection.Add(command);
            try {
                command.Wait(token);
            } catch (OperationCanceledException e) {
                Tools.Warn("Pythonコマンドがキャンセルされました");
                throw new ClipboardAppException("Pythonコマンドがキャンセルされました", e);

            } catch (PythonException e) {
                throw new ClipboardAppException("Pythonコマンド実行時にエラーが発生しました", e);
            }
        }
        public class ResultContainer {
            public object Result { get; set; }
            public ResultContainer(object result) {
                Result = result;
            }
            public ResultContainer() {
                Result = new object();
            }
        }

        private Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("OpenAIEmbeddingModel", Properties.Settings.Default.OpenAIEmbeddingModel);
            dict.Add("AzureOpenAI", Properties.Settings.Default.AzureOpenAI.ToString());
            if (Properties.Settings.Default.OpenAICompletionBaseURL != "") {
                dict.Add("OpenAICompletionBaseURL", Properties.Settings.Default.OpenAICompletionBaseURL);
            }
            if (Properties.Settings.Default.OpenAIEmbeddingBaseURL != "") {
                dict.Add("OpenAIEmbeddingBaseURL", Properties.Settings.Default.OpenAIEmbeddingBaseURL);
            }
            return dict;
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
            ResultContainer resultContainer = new ResultContainer(path);

            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? extract_text = ps?.Get("extract_text");
                // extract_textが呼び出せない場合は例外をスロー
                if (extract_text == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにextract_text関数が見つかりません");
                }
                // extract_text関数を呼び出す
                string result = extract_text(path);
                resultContainer.Result = (object)result;

            });
            return (string)resultContainer.Result;

        }

        // IPythonFunctionsのメソッドを実装
        // データをマスキングする
        public string GetMaskedString(string text) {
            List<string> beforeTextList = new List<string>() { text };
            MaskedData maskedData = GetMaskedData(beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        // マスキングされたデータを元に戻す
        public string GetUnmaskedString(string maskedText) {
            List<string> beforeTextList = new List<string>() { maskedText };
            MaskedData maskedData = GetMaskedData(beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }

        public MaskedData GetMaskedData(List<string> beforeTextList) {
            // PropertiesからSPACY_MODEL_NAMEを取得
            string SpacyModel = Properties.Settings.Default.SpacyModel;
            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SpacyModel)) {
                throw new ClipboardAppException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SpacyModel }
                        };

            ResultContainer resultContainer = new ResultContainer(new MaskedData(beforeTextList));

            PostPythonAction(() => {
                MaskedData actionResult = (MaskedData)resultContainer.Result;

                // Pythonスクリプトの関数を呼び出す
                dynamic? mask_data = ps?.Get("mask_data");
                // mask_dataが呼び出せない場合は例外をスロー
                if (mask_data == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルに、mask_data関数が見つかりません");
                }
                // 結果用のDictionaryを作成
                PyDict resultDict = new PyDict();
                resultDict = mask_data(beforeTextList, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ClipboardAppException("マスキング結果がありません");
                }
                PyObject? textDictObject = resultDict.GetItem("TEXT");
                if (textDictObject == null) {
                    throw new ClipboardAppException("マスキングした文字列取得に失敗しました");
                }
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
                    }catch (PythonException) {
                        entities = null;
                        return;
                    }
                    PyDict entityDict = entities.As<PyDict>();
                    List<MaskedEntity> maskedEntities = GetMaskedEntities(entityNameString, entityDict);
                    actionResult.Entities.UnionWith(maskedEntities);
                }

                resultContainer.Result = (object)actionResult;
            });
            return (MaskedData)resultContainer.Result;
        }

        // GetUnMaskedDataの実装
        public MaskedData GetUnMaskedData(List<string> maskedTextList) {
            // PropertiesからSPACY_MODEL_NAMEを取得
            string SpacyModel = Properties.Settings.Default.SpacyModel;
            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SpacyModel)) {
                throw new ClipboardAppException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
            Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SpacyModel }
                        };
            ResultContainer resultContainer = new ResultContainer(new MaskedData(maskedTextList));
            PostPythonAction(() => {
                MaskedData actionResult = (MaskedData)resultContainer.Result;
                // Pythonスクリプトの関数を呼び出す
                dynamic? unmask_data = ps?.Get("unmask_data");
                // unmask_dataが呼び出せない場合は例外をスロー
                if (unmask_data == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルに、unmask_data関数が見つかりません");
                }
                // 結果用のDictionaryを作成
                PyDict resultDict = new PyDict();
                resultDict = unmask_data(actionResult, dict);
                // resultDictが空の場合は例外をスロー
                if (resultDict == null || resultDict.Any() == false) {
                    throw new ClipboardAppException("マスキング解除結果がありません");
                }
                PyObject? textListObject = resultDict.GetItem("TEXT");
                if (textListObject == null) {
                    throw new ClipboardAppException("マスキング解除した文字列取得に失敗しました");
                }
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

                resultContainer.Result = (object)actionResult;
            });
            return (MaskedData)resultContainer.Result;
        }
        public string ExtractTextFromImage(System.Drawing.Image image) {
            // Pythonスクリプトを実行する
            ResultContainer resultContainer = new ResultContainer("");
            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? extract_text_from_image = ps?.Get("extract_text_from_image");
                // extract_text_from_imageが呼び出せない場合は例外をスロー
                if (extract_text_from_image == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにextract_text_from_image関数が見つかりません");
                }
                // extract_text_from_image関数を呼び出す
                ImageConverter imageConverter = new ImageConverter();
                object? bytesObject = imageConverter.ConvertTo(image, typeof(byte[]));
                if (bytesObject == null) {
                    throw new ClipboardAppException("画像のバイト列に変換できません");
                }
                byte[] bytes = (byte[])bytesObject;
                string result = extract_text_from_image(bytes);
                resultContainer.Result = (object)result;
            });
            return (string)resultContainer.Result;
        }

        private List<MaskedEntity> GetMaskedEntities(string label, PyDict pyDict) {

            List<MaskedEntity> result = new List<MaskedEntity>();
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
                MaskedEntity maskedEntity = new MaskedEntity();
                maskedEntity.Label = label;
                maskedEntity.Before = keyString;
                maskedEntity.After = entityString;
                result.Add(maskedEntity);
            }
            return result;
        }
        // IPythonFunctionsのメソッドを実装
        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory) {

            return QAChatPythonNetFunctions.LangChainOpenAIChat(prompt, chatHistory);
        }
        public void OpenAIEmbedding(string text) {
            ResultContainer resultContainer = new ResultContainer();
            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? open_ai_embedding = ps?.Get("openai_embedding");
                // open_ai_chatが呼び出せない場合は例外をスロー
                if (open_ai_embedding == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルに、openai_embedding関数が見つかりません");
                }
                // open_ai_chat関数を呼び出す
                open_ai_embedding(text, CreateOpenAIProperties());
                // System.Windows.MessageBox.Show(result);
            });
        }

        // IPythonFunctionsのメソッドを実装
        // ★スクリプトを実行する
        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            if (scriptItem == null) {
                throw new ClipboardAppException("スクリプトが指定されていません");
            }
            if (clipboardItem == null) {
                throw new ClipboardAppException("クリップボードアイテムが指定されていません");
            }
            if (string.IsNullOrEmpty(scriptItem.Content)) {
                throw new ClipboardAppException("スクリプトが空です");
            }
            if (string.IsNullOrEmpty(clipboardItem.Content)) {
                throw new ClipboardAppException("クリップボードアイテムの内容が空です");
            }
            ResultContainer resultContainer = new ResultContainer();
            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? run_script = ps?.Get("run_script");
                // run_scriptが呼び出せない場合は例外をスロー
                if (run_script == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにrun_script関数が見つかりません");
                }
                // run_script関数を呼び出す
                run_script(scriptItem.Content, clipboardItem.Content);
            });
        }

        // IPythonFunctionsのメソッドを実装
        public HashSet<string> ExtractEntity(string text) {
            // Pythonスクリプトを実行する
            ResultContainer resultContainer = new ResultContainer(new HashSet<string>());
            PostPythonAction(() => {

                // PropertiesからSPACY_MODEL_NAMEを取得
                string SpacyModel = Properties.Settings.Default.SpacyModel;
                // SPACY_MODEL_NAMEが空の場合は例外をスロー
                if (string.IsNullOrEmpty(SpacyModel)) {
                    throw new ClipboardAppException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
                }

                HashSet<string> actionResult = (HashSet<string>)resultContainer.Result;

                Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SpacyModel }
                        };
                // 結果用のDictionaryを作成
                // Pythonスクリプトの関数を呼び出す
                dynamic? extract_entity = ps?.Get("extract_entity");
                // extract_entityが呼び出せない場合は例外をスロー
                if (extract_entity == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにextract_entity関数が見つかりません");
                }
                PyIterable pyIterable = extract_entity(text, dict);
                // PythonのリストをC#のHashSetに変換
                foreach (PyObject item in pyIterable) {
                    string? entity = item.ToString();
                    if (entity != null) {
                        actionResult.Add(entity);
                    }
                }
                resultContainer.Result = (object)actionResult;

            });
            return (HashSet<string>)resultContainer.Result;

        }

        public void SaveFaissIndex() {
            ResultContainer resultContainer = new ResultContainer();
            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? save_faiss_index = ps?.Get("save_faiss_index");
                // save_faiss_indexが呼び出せない場合は例外をスロー
                if (save_faiss_index == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにsave_faiss_index関数が見つかりません");
                }
                // save_faiss_index関数を呼び出す
                save_faiss_index();
            });
        }

        public void LoadFaissIndex() {
            // Pythonスクリプトを実行する
            ResultContainer resultContainer = new ResultContainer();
            PostPythonAction(() => {
                // Pythonスクリプトの関数を呼び出す
                dynamic? load_faiss_index = ps?.Get("load_faiss_index");
                // load_faiss_indexが呼び出せない場合は例外をスロー
                if (load_faiss_index == null) {
                    throw new ClipboardAppException("Pythonスクリプトファイルにload_faiss_index関数が見つかりません");
                }
                // load_faiss_index関数を呼び出す
                load_faiss_index();
            });
        }
    }
}

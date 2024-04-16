using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using WpfApp1.Utils;

namespace WpfApp1.Model {
    public class PythonNetFunctions : IPythonFunctions{
        private PyModule? ps;
        private BlockingCollection<Task> blockingCollection = new BlockingCollection<Task>();

        public PythonNetFunctions() {

            Task consumerAThread = new Task(() => {
                using (Py.GIL()) {
                    ps = Py.CreateScope();
                    string script = PythonExecutor.LoadPythonScript(PythonExecutor.ClipboardAppUtilsScript);
                    ps.Exec(script);
                }
                while (true) {
                    if (blockingCollection.IsCompleted) {
                        break;
                    }
                    Task command = blockingCollection.Take();
                    command.Start();
                }
            });
            consumerAThread.Start();
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
            string result = "";
            Task command = new Task(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? extract_text = ps?.Get("extract_text");
                    // extract_textが呼び出せない場合は例外をスロー
                    if (extract_text == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにextract_text関数が見つかりません");
                    }
                    // extract_text関数を呼び出す
                    try {
                        result = extract_text(path);

                    }catch(PythonException e) {
                        throw new ThisApplicationException(CreatePythonExceptionMessage(e));
                    }
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;

        }

        // IPythonFunctionsのメソッドを実装
        // データをマスキングする
        public string GetMaskedString(string text) {
            List<string> beforeTextList = new List<string>() { text };
            MaskedData maskedData = GetMaskedData(beforeTextList);
            string result = maskedData.AfterTextList[0];
            return result;
        }
        public MaskedData GetMaskedData(List<string> beforeTextList) {
            // PropertiesからSPACY_MODEL_NAMEを取得
            string SPACY_MODEL_NAME = Properties.Settings.Default.SpacyModel;
            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SPACY_MODEL_NAME)) {
                throw new ThisApplicationException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            MaskedData result = new MaskedData(beforeTextList);

            Task command = new Task(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? mask_data = ps?.Get("mask_data");
                    // mask_dataが呼び出せない場合は例外をスロー
                    if (mask_data == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルに、mask_data関数が見つかりません");
                    }
                    // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
                    Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SPACY_MODEL_NAME }
                        };
                    try {
                        // 結果用のDictionaryを作成
                        PyDict resultDict = new PyDict();
                        resultDict = mask_data(beforeTextList, dict);
                        // resultDictが空の場合は例外をスロー
                        if (resultDict == null || resultDict.Any() == false) {
                            throw new ThisApplicationException("マスキング結果がありません");
                        }
                        PyObject? textListObject = resultDict.GetItem("TEXT");
                        if (textListObject == null) {
                            throw new ThisApplicationException("マスキングした文字列取得に失敗しました");
                        }
                        PyList textList = textListObject.As<PyList>();
                        foreach (PyObject item in textList) {
                            PyObject afterTextObject = item.GetItem("AFTER");
                            string? text = afterTextObject.ToString();
                            if (text == null) {
                                continue;
                            }
                            result.AfterTextList.Add(text);
                        }
                        // PERSONを取得
                        PyObject? entities = resultDict.GetItem("PERSON");
                        if (entities == null) {
                            throw new ThisApplicationException("PERSONがありません");
                        }
                        PyDict personPyDict = entities.As<PyDict>();
                        List<MaskedEntity> personEntities = GetMaskedEntities("PERSON", personPyDict);
                        result.Entities.UnionWith(personEntities);
                        // ORGを取得
                        entities = resultDict.GetItem("ORG");
                        if (entities == null) {
                            throw new ThisApplicationException("ORGがありません");
                        }
                        PyDict orgPyDict = entities.As<PyDict>();
                        List<MaskedEntity> orgEntities = GetMaskedEntities("ORG", orgPyDict);
                        result.Entities.UnionWith(orgEntities);

                    } catch (PythonException e) {
                        throw new ThisApplicationException(CreatePythonExceptionMessage(e));
                    }
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;

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
            public string OpenAIChat(List<JSONChatItem> jSONChatItems) {
            // Pythonスクリプトを実行する
            string result = "";
            Task command = new Task(() => {

                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? open_ai_chat = ps?.Get("openai_chat");
                    // open_ai_chatが呼び出せない場合は例外をスロー
                    if (open_ai_chat == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにopenai_chat関数が見つかりません");
                    }
                    // open_ai_chat関数を呼び出す
                    string json_string = ClipboardItem.ToJson(jSONChatItems);
                    result = open_ai_chat(json_string, CreateOpenAIProperties());
                    // System.Windows.MessageBox.Show(result);
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;

        }
        private Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("AzureOpenAI", Properties.Settings.Default.AzureOpenAI.ToString());
            dict.Add("AzureOpenAIEndpoint", Properties.Settings.Default.AzureOpenAIEndpoint);
            if (Properties.Settings.Default.OpenAIBaseURL != "") {
                dict.Add("AzureOpenAIKey", Properties.Settings.Default.OpenAIBaseURL);
            }
            return dict;
        }

        // IPythonFunctionsのメソッドを実装
        // ★スクリプトを実行する
        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            if (scriptItem == null) {
                throw new ThisApplicationException("スクリプトが指定されていません");
            }
            if (clipboardItem == null) {
                throw new ThisApplicationException("クリップボードアイテムが指定されていません");
            }
            if (string.IsNullOrEmpty(scriptItem.Content)) {
                throw new ThisApplicationException("スクリプトが空です");
            }
            if (string.IsNullOrEmpty(clipboardItem.Content)) {
                throw new ThisApplicationException("クリップボードアイテムの内容が空です");
            }
            // Pythonスクリプトを実行する
            Task command = new Task(() => {
                using (Py.GIL()) {

                    // Pythonスクリプトの関数を呼び出す
                    dynamic? func_name = ps?.Get("execute");
                    // extract_textが呼び出せない場合は例外をスロー
                    if (func_name == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルに、execute関数が見つかりません");
                    }
                    // ClipboardItemをJSON文字列に変換
                    string json_string = ClipboardItem.ToJson(clipboardItem);
                    // extract_text関数を呼び出す
                    try {
                        string result = func_name(json_string);

                        ClipboardItem? resultItem = ClipboardItem.FromJson(result);
                        if (resultItem == null) {
                            throw new ThisApplicationException("Pythonの処理結果のJSON文字列をClipboardItemに変換できませんでした");
                        }
                        // resultItemの結果をClipboardItemにコピー
                        resultItem.CopyTo(clipboardItem);
                    } catch (PythonException e) {
                        throw new ThisApplicationException(CreatePythonExceptionMessage(e));
                    }
                }

            });
            blockingCollection.Add(command);
            command.Wait();
        }


        // IPythonFunctionsのメソッドを実装
        public HashSet<string> ExtractEntity(string text) {
            // Pythonスクリプトを実行する
            HashSet<string> result = new HashSet<string>();
            Task command = new Task(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? extract_entity = ps?.Get("extract_entity");
                    // extract_entityが呼び出せない場合は例外をスロー
                    if (extract_entity == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにextract_entity関数が見つかりません");
                    }
                    // Pythonの関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
                    // PropertiesからSPACY_MODEL_NAMEを取得
                    string SPACY_MODEL_NAME = Properties.Settings.Default.SpacyModel;
                    Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SPACY_MODEL_NAME }
                        };
                    // extract_entity関数を呼び出す
                    try {

                        PyIterable pyIterable = extract_entity(text, dict);
                        // PythonのリストをC#のHashSetに変換
                        foreach (PyObject item in pyIterable) {
                            string? entity = item.ToString();
                            if (entity == null) {
                                continue;
                            }
                            result.Add(entity);
                        }

                    } catch (PythonException e) {
                        throw new ThisApplicationException(CreatePythonExceptionMessage(e));
                    }
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;
        }

    }
}

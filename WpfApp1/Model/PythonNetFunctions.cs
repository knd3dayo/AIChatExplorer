using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python.Runtime;
using WpfApp1.Utils;
using WpfApp1.View.OpenAIView;

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
        public string MaskData(string text) {
            // PropertiesからSPACY_MODEL_NAMEを取得
            string SPACY_MODEL_NAME = Properties.Settings.Default.SpacyModel;
            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SPACY_MODEL_NAME)) {
                throw new ThisApplicationException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            string result = "";
            Task command = new Task(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? mask_data = ps?.Get("mask_data");
                    // mask_dataが呼び出せない場合は例外をスロー
                    if (mask_data == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにmask_data関数が見つかりません");
                    }
                    // mask_data関数を呼び出す. 引数としてTextとSPACY_MODEL_NAMEを渡す
                    Dictionary<string, string> dict = new Dictionary<string, string> {
                            { "SpacyModel", SPACY_MODEL_NAME }
                        };
                    // 結果用のDictionaryを作成
                    PyDict resultDict = new PyDict();
                    try {
                        resultDict = mask_data(text, dict);
                        PyObject? textDic = resultDict.GetItem("TEXT");
                        if (textDic == null) {
                            throw new ThisApplicationException("マスキングできませんでした");
                        }
                        PyObject? afterText = textDic.GetItem("AFTER");
                        if (afterText == null) {
                            throw new ThisApplicationException("マスキングできませんでした");
                        }
                        string? afterTextString = afterText.ToString();
                        if (afterTextString == null) {
                            throw new ThisApplicationException("マスキングできませんでした");
                        }
                        result = afterTextString;

                    } catch (PythonException e) {
                        throw new ThisApplicationException(CreatePythonExceptionMessage(e));
                    }
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;
        }

        // IPythonFunctionsのメソッドを実装
        public string OpenAIChat(List<JSONChatItem> jSONChatItems) {
            // Pythonスクリプトを実行する
            string result = "";
            Task command = new Task(() => {

                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? open_ai_chat = ps?.Get("open_ai_chat");
                    // open_ai_chatが呼び出せない場合は例外をスロー
                    if (open_ai_chat == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにopenai_chat関数が見つかりません");
                    }
                    // open_ai_chat関数を呼び出す
                    string json_string = ClipboardItem.ToJson(jSONChatItems);
                    bool json_mode = false;
                    bool azure_open_ai = Properties.Settings.Default.AzureOpenAI;
                    string open_ai_api_key = Properties.Settings.Default.OpenAIKey;
                    string chat_model_name = Properties.Settings.Default.OpenAICompletionModel;
                    string azure_open_ai_endpoint = Properties.Settings.Default.AzureOpenAIEndpoint;
                    result = open_ai_chat(json_string, json_mode, azure_open_ai, open_ai_api_key, chat_model_name, azure_open_ai_endpoint);
                    // System.Windows.MessageBox.Show(result);
                }
            });
            blockingCollection.Add(command);
            command.Wait();
            return result;

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
        public List<string> ExtractEntity(string text) {
            // Pythonスクリプトを実行する
            List<string> result = new List<string>();
            Task command = new Task(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? extract_entity = ps?.Get("extract_entity");
                    // extract_entityが呼び出せない場合は例外をスロー
                    if (extract_entity == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルにextract_entity関数が見つかりません");
                    }
                    // extract_entity関数を呼び出す
                    try {

                        result = extract_entity(text);

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

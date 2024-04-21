﻿using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using Python.Runtime;
using QAChat.Model;
using QAChat.Utils;

namespace QAChat.PythonIF {

    public class PythonTask(Action action) : Task(action) {

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    }
    public class PythonNetFunctions  {
        private PyModule? ps;
        private BlockingCollection<PythonTask> blockingCollection = new BlockingCollection<PythonTask>();

        public PythonNetFunctions() {

            Task consumerThread = new(() => {
                // 初期化エラー時にcommandを受け付けないようにする
                bool initError = false;
                using (Py.GIL()) {

                    try {
                        ps = Py.CreateScope();
                        string script = PythonExecutor.LoadPythonScript(PythonExecutor.RetrievalScript);
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

        private Dictionary<string, string> CreateOpenAIProperties() {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("OpenAIKey", Properties.Settings.Default.OpenAIKey);
            dict.Add("OpenAICompletionModel", Properties.Settings.Default.OpenAICompletionModel);
            dict.Add("OpenAIEmbeddingModel", Properties.Settings.Default.OpenAIEmbeddingModel);
            dict.Add("AzureOpenAI", Properties.Settings.Default.AzureOpenAI.ToString());
            dict.Add("AzureOpenAIEndpoint", Properties.Settings.Default.AzureOpenAIEndpoint);
            if (Properties.Settings.Default.OpenAIBaseURL != "") {
                dict.Add("OpenAIBaseURL", Properties.Settings.Default.OpenAIBaseURL);
            }
            dict.Add("VectorDBURL", Properties.Settings.Default.VectorDBURL);
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
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            // ChatResultを作成
            ChatResult chatResult = new ChatResult();
            // Pythonスクリプトを実行する
            PythonTask command = new PythonTask(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? langchain_chat = ps?.Get("langchain_chat");
                    // open_ai_chatが呼び出せない場合は例外をスロー
                    if (langchain_chat == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルに、openai_chat関数が見つかりません");
                    }
                    // open_ai_chat関数を呼び出す
                    string json_string = ChatItem.ToJson(chatHistory);

                    // Pythonのoutput: str , referenced_contents: List[str], referenced_file_path: List[str]を持つdictを返す
                    PyDict pyDict = langchain_chat(CreateOpenAIProperties(), prompt, json_string);
                    // outputを取得
                    string? resultString = pyDict["output"].ToString();
                    if (resultString == null) {
                        throw new ThisApplicationException("OpenAIの応答がありません");
                    }
                    // verboseを取得
                    string? verbose = pyDict.GetItem("verbose")?.ToString();
                    if (verbose != null) {
                        chatResult.Verbose = verbose;
                    }

                    // referenced_contentsを取得
                    PyList? referencedContents = pyDict.GetItem("page_content_list") as PyList;
                    if (referencedContents != null) {
                        List<string> referencedContentsList = new List<string>();
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
                        List<string> referencedFilePathList = new List<string>();
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
                }
            });
            // CancellationTokenを設定
            CancellationToken token = command.CancellationTokenSource.Token;
            blockingCollection.Add(command);
            try {
                command.Wait(token);
                return chatResult;

            } catch (OperationCanceledException e) {
                Tools.Warn("Pythonコマンドがキャンセルされました");
                throw new ThisApplicationException("Pythonコマンドがキャンセルされました", e);

            } catch (PythonException e) {
                throw new ThisApplicationException("Pythonコマンド実行時にエラーが発生しました", e);
            }

        }


    }
}

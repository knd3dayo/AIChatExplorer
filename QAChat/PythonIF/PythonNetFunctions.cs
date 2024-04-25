using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using Python.Runtime;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.PythonIF {

    public class PythonTask(Action action) : Task(action) {

        public CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    }
    public class PythonNetFunctions {
        private PyModule? ps;
        private BlockingCollection<PythonTask> blockingCollection = new BlockingCollection<PythonTask>();

        public PythonNetFunctions() {

            Task consumerThread = new(() => {
                // 初期化エラー時にcommandを受け付けないようにする
                bool initError = false;
                using (Py.GIL()) {

                    try {
                        ps = Py.CreateScope();
                        string script = PythonExecutor.LoadPythonScript(PythonExecutor.QAChatScript);
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



        public static string CreatePythonExceptionMessage(PythonException e) {
            string pythonErrorMessage = e.Message;
            string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
            if (pythonErrorMessage.Contains("No module named")) {
                message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
            }
            message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);
            return message;
        }
        // LangChainOpenAIChatを実行する
        public ChatResult LangChainOpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            // ChatResultを作成
            ChatResult chatResult = new ChatResult();
            // Pythonスクリプトを実行する
            PythonTask command = new PythonTask(() => {
                using (Py.GIL()) {
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
        // 通常のOpenAIChatを実行する
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            // ChatResultを作成
            ChatResult chatResult = new ChatResult();
            // promptからChatItemを作成
            ChatItem chatItem = new ChatItem(ChatItem.UserRole, prompt);
            // chatHistoryをコピーしてChatItemを追加
            List<ChatItem> chatHistoryList = new List<ChatItem>(chatHistory);
            chatHistoryList.Add(chatItem);

            // Pythonスクリプトを実行する
            PythonTask command = new PythonTask(() => {
                using (Py.GIL()) {
                    // Pythonスクリプトの関数を呼び出す
                    dynamic? openai_chat = ps?.Get("openai_chat");
                    // open_ai_chatが呼び出せない場合は例外をスロー
                    if (openai_chat == null) {
                        throw new ThisApplicationException("Pythonスクリプトファイルに、openai_chat関数が見つかりません");
                    }
                    string json_string = ChatItem.ToJson(chatHistoryList);

                    // open_ai_chat関数を呼び出す
                    string resultString = openai_chat(QAChatProperties.CreateOpenAIProperties(), json_string);
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

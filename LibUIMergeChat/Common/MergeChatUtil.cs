using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folders.Base;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUIPythonAI.Resource;
using LibUIPythonAI.ViewModel.Common;

namespace LibUIMergeChat.Common {
    public class MergeChatUtil {

        public static async Task<ChatResponse> MergeChat(
            ChatRequestContext context, List<ContentItemWrapper> items, string preProcessPrompt, string postProcessPrompt, string sessionToken, List<ContentItemDataDefinition>? targetDataList = null) {
            // プリプロセスのリクエストを作成。 items毎にリクエストを作成
            List<ChatResponse> preProcessResults = PreProcess(items, context, preProcessPrompt, sessionToken, targetDataList);

            // ポストプロセスのリクエストを作成。 プリプロセスの結果を結合してリクエストを作成
            ChatResponse? postProcessResult = await PostProcess(preProcessResults, context, postProcessPrompt, sessionToken);
            // ChatUtil.ExecuteChatを実行
            if (postProcessResult == null) {
                return new ChatResponse();
            }
            return postProcessResult;

        }
        // ContentItemとExportImportItemを受け取り、対象データを取得する
        private static string GetTargetData(ContentItemWrapper contentItem, List<ContentItemDataDefinition>? targetDataList) {
            string targetData = "";
            if (targetDataList == null) {
                return contentItem.Content;
            }

            foreach (var item in targetDataList) {
                if (item.IsChecked) {
                    if (item.Name == "Properties") {
                        targetData += contentItem.HeaderText + "\n";
                    }
                    if (item.Name == "Text") {
                        targetData += contentItem.Content + "\n";
                    }
                    // PromptItemのリスト要素毎に処理を行う
                    if (item.IsPromptItem) {
                        string promptResult = contentItem.PromptChatResult.GetTextContent(item.Name);
                        targetData += promptResult + "\n";
                    }

                }
            }
            return targetData;
        }

        private static List<ChatResponse> PreProcess(List<ContentItemWrapper> items, ChatRequestContext context, string preProcessPrompt, string sessionToken, List<ContentItemDataDefinition>? targetDataList) {
            List<ChatResponse> preProcessResults = [];
            if (!string.IsNullOrEmpty(preProcessPrompt)) {
                object lockObject = new();
                int start_count = 0;
                int count = items.Count;

                // Parallel.ForEを使って、items毎にプリプロセスを実行
                ParallelOptions parallelOptions = new() {
                    // 20並列
                    MaxDegreeOfParallelism = 4
                };
                Parallel.For(0, count, parallelOptions, async (i) => {
                    string message = $"{CommonStringResources.Instance.MergeChatPreprocessingInProgress} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    ContentItemWrapper item = items[i];
                    ChatSettings chatSettings = new() {
                        PromptTemplateText = preProcessPrompt,
                        SplitMode = context.SplitMode,
                        SplitTokenCount = context.SplitTokenCount,
                        RAGMode = context.RAGMode,
                        VectorSearchRequests = context.VectorSearchRequests,
                        AutoGenPropsRequest = context.AutoGenPropsRequest,
                    };
                    ChatRequestContext preProcessRequestContext = new(chatSettings);

                    string contentText = GetTargetData(item, targetDataList);
                    if (string.IsNullOrEmpty(contentText)) {
                        return;
                    }
                    ChatRequest preProcessRequest = new() {
                        ContentText = contentText,
                    };
                    ChatResponse? preProcessResult = await Task.Run(async () => {
                        return await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, preProcessRequest, preProcessRequestContext, (text) => { });
                    });
                    if (preProcessResult == null) {
                        return;
                    }
                    lock (lockObject) {
                        preProcessResults.Add(preProcessResult);
                    }
                });
            } else {
                foreach (var item in items) {
                    string contentText = GetTargetData(item, targetDataList);
                    if (string.IsNullOrEmpty(contentText)) {
                        continue;
                    }
                    ChatResponse chatResult = new() {
                        Output = contentText,
                    };
                    preProcessResults.Add(chatResult);
                }
            }
            StatusText.Instance.UpdateInProgress(false);
            return preProcessResults;
        }


        private static async Task<ChatResponse?> PostProcess(List<ChatResponse> preProcessResults, ChatRequestContext context, string postProcessPrompt, string sessionToken) {
            if (preProcessResults.Count == 0) {
                LogWrapper.Info("PreProcessResults is empty.");
                return null;
            }
            string preProcessResultText = "";
            foreach (var result in preProcessResults) {
                preProcessResultText += result.Output + "\n";
            }
            if (string.IsNullOrEmpty(postProcessPrompt)) {
                return new ChatResponse() {
                    Output = preProcessResultText,
                };
            }
            ChatSettings chatSettings = new() {
                PromptTemplateText = postProcessPrompt,
                SplitMode = context.SplitMode,
                SplitTokenCount = context.SplitTokenCount,
                RAGMode = context.RAGMode,
                VectorSearchRequests = context.VectorSearchRequests,
                AutoGenPropsRequest = context.AutoGenPropsRequest,
            };
            ChatRequestContext postProcessRequestContext = new(chatSettings);

            ChatRequest postProcessRequest = new() {
                ContentText = preProcessResultText,
            };
            ChatResponse? postProcessResult = await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, postProcessRequest, postProcessRequestContext, (text) => { });
            return postProcessResult;
        }

    }
}

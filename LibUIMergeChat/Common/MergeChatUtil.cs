using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUIPythonAI.Resource;
using LibUIPythonAI.ViewModel.Common;

namespace LibUIMergeChat.Common {
    public class MergeChatUtil {
        public static async Task<ChatResponse> MergeChat(
            ChatRequestContext context, List<ContentItemWrapper> items, string preProcessPrompt, string postProcessPrompt, string sessionToken, List<ContentItemDataDefinition>? targetDataList = null)
        {
            // プリプロセスのリクエストを作成。 items毎にリクエストを作成
            List<ChatResponse> preProcessResults = await PreProcess(items, context, preProcessPrompt, sessionToken, targetDataList);

            // ポストプロセスのリクエストを作成。 プリプロセスの結果を結合してリクエストを作成
            ChatResponse? postProcessResult = await PostProcess(preProcessResults, context, postProcessPrompt, sessionToken);
            // ChatUtil.ExecuteChatを実行
            return postProcessResult ?? new ChatResponse();
        }
        // ContentItemとExportImportItemを受け取り、対象データを取得する
        private static async Task<string> GetTargetData(ContentItemWrapper contentItem, List<ContentItemDataDefinition>? targetDataList)
        {
            if (targetDataList == null)
            {
                return contentItem.Content;
            }

            var sb = new System.Text.StringBuilder();
            foreach (var item in targetDataList)
            {
                if (item.IsChecked)
                {
                    if (item.Name == "Properties")
                    {
                        var headerText = await contentItem.GetHeaderTextAsync();
                        sb.AppendLine(headerText);
                    }
                    if (item.Name == "Text")
                    {
                        sb.AppendLine(contentItem.Content);
                    }
                    // PromptItemのリスト要素毎に処理を行う
                    if (item.IsPromptItem)
                    {
                        string promptResult = contentItem.PromptChatResult.GetTextContent(item.Name);
                        sb.AppendLine(promptResult);
                    }
                }
            }
            return sb.ToString();
        }

        private static async Task<List<ChatResponse>> PreProcess(List<ContentItemWrapper> items, ChatRequestContext context, string preProcessPrompt, string sessionToken, List<ContentItemDataDefinition>? targetDataList)
        {
            var preProcessResults = new List<ChatResponse>();

            if (!string.IsNullOrEmpty(preProcessPrompt))
            {
                int count = items.Count;
                int completed = 0;
                var tasks = items.Select(async item =>
                {
                    string message = $"{CommonStringResources.Instance.MergeChatPreprocessingInProgress} ({++completed}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    string contentText = await GetTargetData(item, targetDataList);
                    if (string.IsNullOrEmpty(contentText)) return null;

                    var chatSettings = new ChatSettings
                    {
                        PromptTemplateText = preProcessPrompt,
                        SplitMode = context.SplitMode,
                        SplitTokenCount = context.SplitTokenCount,
                        RAGMode = context.RAGMode,
                        VectorSearchRequests = context.VectorSearchRequests,
                    };
                    var preProcessRequestContext = new ChatRequestContext(chatSettings);

                    var preProcessRequest = new ChatRequest { ContentText = contentText };
                    var preProcessResult = await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, preProcessRequest, preProcessRequestContext, _ => { });
                    return preProcessResult;
                });

                var results = await Task.WhenAll(tasks);
                preProcessResults.AddRange(results.Where(r => r != null)!);
            }
            else
            {
                foreach (var item in items)
                {
                    string contentText = await GetTargetData(item, targetDataList);
                    if (string.IsNullOrEmpty(contentText)) continue;
                    preProcessResults.Add(new ChatResponse { Output = contentText });
                }
            }
            StatusText.Instance.UpdateInProgress(false);
            return preProcessResults;
        }


        private static async Task<ChatResponse?> PostProcess(List<ChatResponse> preProcessResults, ChatRequestContext context, string postProcessPrompt, string sessionToken)
        {
            if (preProcessResults.Count == 0)
            {
                LogWrapper.Info("PreProcessResults is empty.");
                return null;
            }
            var sb = new System.Text.StringBuilder();
            foreach (var result in preProcessResults)
            {
                sb.AppendLine(result.Output);
            }
            string preProcessResultText = sb.ToString();
            if (string.IsNullOrEmpty(postProcessPrompt))
            {
                return new ChatResponse()
                {
                    Output = preProcessResultText,
                };
            }
            ChatSettings chatSettings = new()
            {
                PromptTemplateText = postProcessPrompt,
                SplitMode = context.SplitMode,
                SplitTokenCount = context.SplitTokenCount,
                RAGMode = context.RAGMode,
                VectorSearchRequests = context.VectorSearchRequests,
            };
            ChatRequestContext postProcessRequestContext = new(chatSettings);

            ChatRequest postProcessRequest = new()
            {
                ContentText = preProcessResultText,
            };
            ChatResponse? postProcessResult = await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, postProcessRequest, postProcessRequestContext, _ => { });
            return postProcessResult;
        }

    }
}

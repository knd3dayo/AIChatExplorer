using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Utils.Common;
using PythonAILib.Utils.Python;

namespace MergeChat.Common {
    public class MergeChatUtil {

        public static ChatResult MergeChat(
            ChatRequestContext context, List<ContentItem> items, string preProcessPrompt, string postProcessPrompt, List<ExportImportItem>? targetDataList = null) {
            // プリプロセスのリクエストを作成。 items毎にリクエストを作成
            List<ChatResult> preProcessResults = PreProcess(items, context, preProcessPrompt);

            // ポストプロセスのリクエストを作成。 プリプロセスの結果を結合してリクエストを作成
            ChatResult? postProcessResult = PostProcess(preProcessResults, context, postProcessPrompt);
            // ChatUtil.ExecuteChatを実行
            if (postProcessResult == null) {
                return new ChatResult();
            }
            return postProcessResult;

        }
        // ContentItemとExportImportItemを受け取り、対象データを取得する
        private static string GetTargetData(ContentItem contentItem, List<ExportImportItem>? targetDataList) {
            string targetData = "";
            if (targetDataList == null) {
                return contentItem.Content;
            }

            foreach (var item in targetDataList) {
                if (item.IsChecked) {
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

        private static List<ChatResult> PreProcess(List<ContentItem> items, ChatRequestContext context, string preProcessPrompt, List<ExportImportItem>? targetDataList = null) {
            List<ChatResult> preProcessResults = [];
            if (!string.IsNullOrEmpty(preProcessPrompt)) {
                foreach (var item in items) {
                    ChatRequestContext preProcessRequestContext = new() {
                        PromptTemplateText = preProcessPrompt,
                        ChatMode = context.ChatMode,
                        SplitMode = context.SplitMode,
                        SplitTokenCount = context.SplitTokenCount,
                        UseVectorDB = context.UseVectorDB,
                        VectorSearchProperties = context.VectorSearchProperties,
                        AutoGenProperties = context.AutoGenProperties,
                        OpenAIProperties = context.OpenAIProperties,
                    };
                    string contentText = GetTargetData(item, targetDataList);
                    if (string.IsNullOrEmpty(contentText)) {
                        continue;
                    }
                    ChatRequest preProcessRequest = new() {
                        ContentText = contentText,
                    };

                    ChatResult? preProcessResult = ChatUtil.ExecuteChat(preProcessRequest, preProcessRequestContext, (text) => { });
                    if (preProcessResult == null) {
                        continue;
                    }
                    preProcessResults.Add(preProcessResult);
                }
            } else {
                foreach (var item in items) {
                    ChatResult chatResult = new() {
                        Output = item.Content,
                    };
                    preProcessResults.Add(chatResult);
                }
            }
            return preProcessResults;
        }


        private static ChatResult? PostProcess(List<ChatResult> preProcessResults, ChatRequestContext context, string postProcessPrompt) {
            if (preProcessResults.Count == 0) {
                LogWrapper.Info("PreProcessResults is empty.");
                return null;
            }
            string preProcessResultText = "";
            foreach (var result in preProcessResults) {
                preProcessResultText += result.Output + "\n";
            }
            if (string.IsNullOrEmpty(postProcessPrompt)) {
                return new ChatResult() {
                    Output = preProcessResultText,
                };
            }

            ChatRequestContext postProcessRequestContext = new() {
                PromptTemplateText = postProcessPrompt,
                ChatMode = context.ChatMode,
                SplitMode = context.SplitMode,
                SplitTokenCount = context.SplitTokenCount,
                UseVectorDB = context.UseVectorDB,
                VectorSearchProperties = context.VectorSearchProperties,
                AutoGenProperties = context.AutoGenProperties,
                OpenAIProperties = context.OpenAIProperties,
            };
            ChatRequest postProcessRequest = new() {
                ContentText = preProcessResultText,
            };
            ChatResult? postProcessResult = ChatUtil.ExecuteChat(postProcessRequest, postProcessRequestContext, (text) => { });
            return postProcessResult;
        }

    }
}

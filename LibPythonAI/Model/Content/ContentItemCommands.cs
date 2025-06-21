using System.Diagnostics;
using LibPythonAI.Common;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;

namespace LibPythonAI.Model.Content {
    public class ContentItemCommands {



        // Command to open a folder
        public static void OpenFolderInExplorer(ContentItemWrapper contentItem) {
            // Open the folder only if the ContentType is File
            if (contentItem.ContentType != ContentItemTypes.ContentItemTypeEnum.Files) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            string message = $"{PythonAILibStringResourcesJa.Instance.ExecuteOpenFolder} {contentItem.FolderName}";
            LogWrapper.Info(message);

            // Open the folder with Process.Start
            string? folderPath = contentItem.FolderName;
            if (folderPath != null) {
                var p = new Process {
                    StartInfo = new ProcessStartInfo(folderPath) {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
            message = $"{PythonAILibStringResourcesJa.Instance.ExecuteOpenFolderSuccess} {contentItem.FolderName}";
            LogWrapper.Info(message);

        }


        // OpenAIを使用してイメージからテキスト抽出する。
        public static async Task ExtractImageWithOpenAIAsync(ContentItemWrapper item) {
            await ExtractText(item);
        }



        // テキストを抽出する
        public static async Task ExtractText(ContentItemWrapper item) {

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() { };


            try {
                if (item.IsImage()) {
                    string base64 = item.Base64Image;
                    string result = await ChatUtil.ExtractTextFromImage(chatRequestContext, [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        item.Content = result;
                    }
                } else if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                    // ファイル名から拡張子を取得
                    string text = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(item.SourcePath);
                    item.Content = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.UnsupportedFileType);
            }
        }


        public static void ExtractTexts(List<ContentItemWrapper> items, Action beforeAction, Action afterAction) {
            int count = items.Count;
            if (count == 0) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoItemSelected);
                return;
            }
            beforeAction();

            Task.Run(() => {
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new() {
                    // 20並列
                    MaxDegreeOfParallelism = 4
                };
                Parallel.For(0, count, parallelOptions, (i) => {
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    lock (lockObject) {
                        start_count++;
                    }
                    string message = $"{PythonAILibStringResourcesJa.Instance.TextExtractionInProgress} ({start_count}/{count})";
                    LogWrapper.UpdateInProgress(true, message);
                    var item = items[index];

                    if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                        LogWrapper.Info(PythonAILibStringResourcesJa.Instance.CannotExtractTextForNonFileContent);
                        return;
                    }
                    ExtractText(item).Wait();
                    // SaveAsync the item
                    item.Save();
                });
                afterAction();
                LogWrapper.UpdateInProgress(false);
                LogWrapper.Info($"{PythonAILibStringResourcesJa.Instance.TextExtracted}");
            });
        }


        // ベクトルを更新する
        public static void DeleteEmbeddings(List<ContentItemWrapper> items) {

            Task.Run(() => {
                // Parallelによる並列処理。4並列
                ParallelOptions parallelOptions = new() {
                    MaxDegreeOfParallelism = 4
                };
                Parallel.ForEach(items, parallelOptions, (item) => {

                    string? vectorDBItemName = item.GetMainVectorSearchItem().VectorDBItemName;
                    if (string.IsNullOrEmpty(vectorDBItemName)) {
                        LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                        return;
                    }
                    VectorEmbeddingItem vectorDBEntry = new(item.Id.ToString(), item.GetFolder().ContentFolderPath);
                    vectorDBEntry.SetMetadata(item);

                    VectorEmbeddingItem.DeleteEmbeddings(vectorDBItemName, vectorDBEntry);
                });
            });
        }

        // Embeddingを更新する
        public static void UpdateEmbeddings(List<ContentItemWrapper> items, Action beforeAction, Action afterAction) {
            beforeAction();
            Task.Run(() => {
                // Parallelによる並列処理。4並列
                ParallelOptions parallelOptions = new() {
                    MaxDegreeOfParallelism = 4
                };
                Parallel.ForEach(items, parallelOptions, (item) => {
                    // VectorDBItemを取得
                    string? vectorDBItemName = item.GetMainVectorSearchItem().VectorDBItemName;
                    if (string.IsNullOrEmpty(vectorDBItemName)) {
                        LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                        return;
                    }
                    // IPythonAIFunctions.ClipboardInfoを作成
                    VectorEmbeddingItem vectorDBEntry = new(item.Id.ToString(), item.GetFolder().ContentFolderPath);
                    vectorDBEntry.SetMetadata(item);

                    VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, vectorDBEntry);
                    // ベクトル化日時を更新
                    item.VectorizedAt = DateTime.Now;
                });
                // Execute if obj is an Action
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GenerateVectorCompleted);
                afterAction();
            });

        }

        public static void CreateAutoTitle(ContentItemWrapper item) {
            // TextとImageの場合
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text || item.ContentType == ContentItemTypes.ContentItemTypeEnum.Image) {
                item.Description = $"{item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                item.Description = $"{item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += $" {PythonAILibStringResourcesJa.Instance.File}:" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += $" {PythonAILibStringResourcesJa.Instance.File}:" + item.Content;
                }
            }
        }

        public static async Task SaveChatHistoryAsync(ContentItemWrapper contentItem, ContentFolderWrapper chatFolder) {

            contentItem.Save();
            // チャット履歴用のItemの設定
            ContentItemWrapper chatHistoryItem = contentItem.Copy();
            chatHistoryItem.Entity.FolderId = chatFolder.Id;
            // 更新日時を更新
            chatHistoryItem.Entity.UpdatedAt = DateTime.Now;

            // ChatItemsTextをContentに設定
            chatHistoryItem.Content = contentItem.ChatItemsText;

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;

            if (configParams.AutoTitle()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                CreateAutoTitle(chatHistoryItem);

            } else if (configParams.AutoTitleWithOpenAI()) {

                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                await PromptItem.CreateAutoTitleWithOpenAIAsync(chatHistoryItem);
            }

            chatHistoryItem.Save();

        }

    }
}

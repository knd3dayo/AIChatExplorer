using System.Diagnostics;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.Prompt;
using LibMain.Model.VectorDB;
using LibMain.PythonIF;
using LibMain.PythonIF.Request;
using LibMain.Resources;
using LibMain.Utils.Common;
using LibMain.Utils.Python;

namespace LibMain.Model.Content {
    public class ContentItemCommands {



        // Command to open a folder
        public static void OpenFolderInExplorer(ContentItem contentItem) {
            // Open the folder only if the ContentType is File
            if (contentItem.ContentType != ContentItemTypes.ContentItemTypeEnum.Files) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            string message = $"{PythonAILibStringResourcesJa.Instance.ExecuteOpenFolder} {contentItem.FolderName}";
            LogWrapper.Info(message);

            // Open the folder with Process.Start
            string? folderPath = contentItem.FolderName;
            try {
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
            } catch (Exception ex) {
                LogWrapper.Error($"{PythonAILibStringResourcesJa.Instance.ExecuteOpenFolderFailed} {ex.Message}");
            }
        }


        // OpenAIを使用してイメージからテキスト抽出する。
        public static async Task ExtractImageWithOpenAIAsync(ContentItem item) {
            await ExtractTextAsync(item);
        }



        // テキストを抽出する
        public static async Task ExtractTextAsync(ContentItem item) {

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatSettings chatSettings = new() ;


            try {
                if (item.IsImage()) {
                    string base64 = item.Base64Image;
                    string result = await ChatUtil.ExtractTextFromImage(chatSettings, [base64]);
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


        public static async Task ExtractTextsAsync(List<ContentItem> items) {
            int count = items.Count;
            if (count == 0) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoItemSelected);
                return;
            }

            int start_count = 0;
            object lockObject = new();
            var tasks = items.Select(async item => {
                lock (lockObject) {
                    start_count++;
                }
                string message = $"{PythonAILibStringResourcesJa.Instance.TextExtractionInProgress} ({start_count}/{count})";
                LogWrapper.UpdateInProgress(true, message);
                if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.CannotExtractTextForNonFileContent);
                    return;
                }
                await ExtractTextAsync(item);
                await item.SaveAsync();
            });
            await Task.WhenAll(tasks);

            LogWrapper.UpdateInProgress(false);
            LogWrapper.Info($"{PythonAILibStringResourcesJa.Instance.TextExtracted}");
        }


        // ベクトルを更新する
        public static async Task DeleteEmbeddingsAsync(List<ContentItem> items) {
            var tasks = items.Select(async item => {
                var vectorDBItem = await item.GetMainVectorSearchItemAsync();
                string? vectorDBItemName = vectorDBItem.VectorDBItemName;
                if (string.IsNullOrEmpty(vectorDBItemName)) {
                    LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                    return;
                }
                var folder = await item.GetFolderAsync();
                if (folder == null) {
                    return;
                }
                var contentFolderPath = await folder.GetContentFolderPath();
                VectorEmbeddingItem vectorDBEntry = new(item.Id.ToString(), contentFolderPath);
                await vectorDBEntry.SetMetadata(item);
                await VectorEmbeddingItem.DeleteEmbeddingsAsync(vectorDBItemName, vectorDBEntry);
            });
            await Task.WhenAll(tasks);
        }

        // Embeddingを更新する
        public static async Task UpdateEmbeddingsAsync(List<ContentItem> items, Action beforeAction, Action afterAction) {
            beforeAction();
            var tasks = items.Select(async item => {
                // VectorDBItemを取得
                var vectorDBitem = await item.GetMainVectorSearchItemAsync();
                string? vectorDBItemName = vectorDBitem.VectorDBItemName;
                if (string.IsNullOrEmpty(vectorDBItemName)) {
                    LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                    return;
                }
                // IPythonAIFunctions.ClipboardInfoを作成
                var folder = await item.GetFolderAsync();
                var contentFolderPath = await folder.GetContentFolderPath();
                VectorEmbeddingItem vectorDBEntry = new(item.Id.ToString(), contentFolderPath);
                await vectorDBEntry.SetMetadata(item);
                VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, vectorDBEntry);
                // ベクトル化日時を更新
                item.VectorizedAt = DateTime.Now;
            });
            await Task.WhenAll(tasks);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.GenerateVectorCompleted);
            afterAction();
        }

        public static void CreateAutoTitle(ContentItem item) {
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

        public static async Task SaveChatHistoryAsync(ContentItem contentItem, ContentFolderWrapper chatFolder) {

            await contentItem.SaveAsync();
            // チャット履歴用のItemの設定
            ContentItem chatHistoryItem = contentItem.Copy();
            chatHistoryItem.Entity.FolderId = chatFolder.Id;
            // 更新日時を更新
            chatHistoryItem.Entity.UpdatedAt = DateTime.Now;

            // ChatItemsTextをContentに設定
            chatHistoryItem.Content = contentItem.ChatItemsText;

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;

            if (configParams.IsAutoTitleEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                CreateAutoTitle(chatHistoryItem);

            } else if (configParams.IsAutoTitleWithOpenAIEnabled()) {

                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                await PromptItem.CreateAutoTitleWithOpenAIAsync(chatHistoryItem);
            }

            await chatHistoryItem.SaveAsync();

        }

    }
}

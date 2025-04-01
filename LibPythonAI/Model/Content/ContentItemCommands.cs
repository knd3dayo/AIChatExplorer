using System.Diagnostics;
using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.Content {
    public class ContentItemCommands {



        // Command to open a folder
        public static void OpenFolder(ContentItemWrapper contentItem) {
            // Open the folder only if the ContentType is File
            if (contentItem.ContentType != ContentTypes.ContentItemTypes.Files) {
                LogWrapper.Error(PythonAILibStringResources.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            string message = $"{PythonAILibStringResources.Instance.ExecuteOpenFolder} {contentItem.FolderName}";
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
            message = $"{PythonAILibStringResources.Instance.ExecuteOpenFolderSuccess} {contentItem.FolderName}";
            LogWrapper.Info(message);

        }
        public static void ExecutePromptTemplate(List<ContentItemWrapper> items, PromptItem promptItem, Action beforeAction, Action afterAction) {

            // promptNameからDescriptionを取得
            string description = promptItem.Description;

            LogWrapper.Info(PythonAILibStringResources.Instance.PromptTemplateExecute(description));
            int count = items.Count;
            Task.Run(() => {
                beforeAction();
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new() {
                    MaxDegreeOfParallelism = 4
                };
                Parallel.For(0, count, parallelOptions, (i) => {
                    lock (lockObject) {
                        start_count++;
                    }
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    string message = $"{PythonAILibStringResources.Instance.PromptTemplateInProgress(description)} ({start_count}/{count})";
                    LogWrapper.UpdateInProgress(true, message);
                    ContentItemWrapper item = items[index];

                    ContentItemCommands.CreateChatResult(item, promptItem.Name);
                    // Save
                    item.Save();
                });
                // Execute if obj is an Action
                afterAction();
                LogWrapper.UpdateInProgress(false);
                LogWrapper.Info(PythonAILibStringResources.Instance.PromptTemplateExecuted(description));
            });

        }

        // OpenAIを使用してイメージからテキスト抽出する。
        public static void ExtractImageWithOpenAI(ContentItemWrapper item) {
            ExtractText(item);
        }

        // OpenAIを使用してタイトルを生成する
        public static void CreateAutoTitleWithOpenAI(ContentItemWrapper item) {
            // ContentTypeがTextの場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                using PythonAILibDBContext db = new();
                PromptItemEntity? promptItem = db.PromptItems.FirstOrDefault(x => x.Name == SystemDefinedPromptNames.TitleGeneration.ToString());
                if (promptItem == null) {
                    LogWrapper.Error("PromptItem not found");
                    return;
                }
                CreateChatResult(item, promptItem.Name);
                return;
            }
            // ContentTypeがFiles,の場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Files) {
                // ファイル名をタイトルとして使用
                item.Description += item.FileName;
                return;
            }
            // ContentTypeがImageの場合
            item.Description = "Image";
        }

        // ExecuteSystemDefinedPromptを実行する
        public static void CreateChatResult(ContentItemWrapper item, string promptName) {
            // システム定義のPromptItemを取得
            PromptItem promptItem = PromptItem.GetPromptItemByName(promptName) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            ContentItemCommands.CreateChatResult(item, promptItem);
        }

        // 文章の信頼度を判定する
        public static void CheckDocumentReliability(ContentItemWrapper item) {

            CreateChatResult(item, SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // PromptChatResultからキー：DocumentReliabilityCheck.ToString()の結果を取得
            string result = item.PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // resultがない場合は処理しない
            if (string.IsNullOrEmpty(result)) {
                return;
            }
            // ChatUtl.CreateDictionaryChatResultを実行
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = item.GetFolder().GetVectorSearchProperties(),
                OpenAIProperties = openAIProperties,
                SessionToken = Guid.NewGuid().ToString()
            };

            Dictionary<string, dynamic?> response = ChatUtil.CreateDictionaryChatResult(chatRequestContext, new PromptItem(new LibPythonAI.Data.PromptItemEntity()) {
                ChatMode = OpenAIExecutionModeEnum.Normal,
                // ベクトルDBを使用する
                UseVectorDB = true,
                Prompt = PromptStringResource.Instance.DocumentReliabilityDictionaryPrompt
            }, result);
            // responseからキー：reliabilityを取得
            if (response.ContainsKey("reliability") == false) {
                return;
            }
            dynamic? reliability = response["reliability"];

            int reliabilityValue = int.Parse(reliability?.ToString() ?? "0");

            // DocumentReliabilityにReliabilityを設定
            item.DocumentReliability = reliabilityValue;
            // responseからキー：reasonを取得
            if (response.ContainsKey("reason")) {
                dynamic? reason = response["reason"];
                // DocumentReliabilityReasonにreasonを設定
                item.DocumentReliabilityReason = reason?.ToString() ?? "";
            }
        }

        // PromptItemの内容でチャットを実行して結果をPromptChatResultに保存する
        public static void CreateChatResult(ContentItemWrapper item, PromptItem promptItem) {

            // PromptItemのPromptInputNameがある場合はPromptInputNameのContentを取得
            string contentText;
            if (string.IsNullOrEmpty(promptItem.PromptInputName) == false) {
                // PromptInputNameのContentを取得
                contentText = item.PromptChatResult.GetTextContent(promptItem.PromptInputName);
                // inputContentがない場合は処理しない
                if (string.IsNullOrEmpty(contentText)) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.InputContentNotFound);
                    return;
                }
            } else {
                // Contentがない場合は処理しない
                if (string.IsNullOrEmpty(item.Content)) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.InputContentNotFound);
                    return;
                }
                // ヘッダー情報とコンテンツ情報を結合
                // ★TODO タグ情報を追加する
                contentText = item.HeaderText + "\n" + item.Content;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBProperty> vectorSearchProperties = promptItem.UseVectorDB ? item.GetFolder().GetVectorSearchProperties() : [];
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = vectorSearchProperties,
                OpenAIProperties = openAIProperties,
                PromptTemplateText = promptItem.Prompt,
                ChatMode = promptItem.ChatMode,
                SplitMode = promptItem.SplitMode,
                SessionToken = Guid.NewGuid().ToString()

            };


            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TextContent) {
                string result = ChatUtil.CreateTextChatResult(chatRequestContext, promptItem, contentText);
                if (string.IsNullOrEmpty(result) == false) {
                    // PromptChatResultに結果を保存
                    item.PromptChatResult.SetTextContent(promptItem.Name, result);
                    // PromptOutputTypeがOverwriteTitleの場合はDescriptionに結果を保存
                    if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteTitle) {
                        item.Description = result;
                    }
                    // PromptOutputTypeがOverwriteContentの場合はContentに結果を保存
                    if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteContent) {
                        item.Content = result;
                    }
                }
                return;
            }

            // PromptResultTypeがTableContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TableContent) {
                Dictionary<string, dynamic?> response = ChatUtil.CreateTableChatResult(chatRequestContext, promptItem, contentText);
                // resultからキー:resultを取得
                if (response.ContainsKey("result") == false) {
                    return;
                }
                dynamic? results = response["result"];
                // resultがない場合は処理しない
                if (results == null) {
                    return;
                }
                if (results.Count > 0) {
                    // resultからDynamicDictionaryObjectを作成
                    List<Dictionary<string, object>> resultDictList = [];
                    foreach (var result in results) {
                        resultDictList.Add(result);
                    }
                    // PromptChatResultに結果を保存
                    item.PromptChatResult.SetTableContent(promptItem.Name, resultDictList);
                }
                return;
            }
            // PromptResultTypeがListの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.ListContent) {
                List<string> response = ChatUtil.CreateListChatResult(chatRequestContext, promptItem, contentText);
                if (response.Count > 0) {
                    // PromptChatResultに結果を保存
                    item.PromptChatResult.SetListContent(promptItem.Name, response);
                }
                return;
            }
        }


        // テキストを抽出する
        public static void ExtractText(ContentItemWrapper item) {

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = item.GetFolder().GetVectorSearchProperties(),
                OpenAIProperties = openAIProperties,
                SessionToken = Guid.NewGuid().ToString()

            };


            try {
                if (item.IsImage()) {
                    string base64 = item.Base64Image;
                    string result = ChatUtil.ExtractTextFromImage(chatRequestContext, [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        item.Content = result;
                    }
                } else if (item.ContentType == ContentTypes.ContentItemTypes.Files) {
                    // ファイル名から拡張子を取得
                    string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.SourcePath);
                    item.Content = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(PythonAILibStringResources.Instance.UnsupportedFileType);
            }
        }


        public static void ExtractTexts(List<ContentItemWrapper> items, Action beforeAction, Action afterAction) {
            int count = items.Count;
            if (count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
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
                    string message = $"{PythonAILibStringResources.Instance.TextExtractionInProgress} ({start_count}/{count})";
                    LogWrapper.UpdateInProgress(true, message);
                    var item = items[index];

                    if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                        LogWrapper.Info(PythonAILibStringResources.Instance.CannotExtractTextForNonFileContent);
                        return;
                    }
                    ContentItemCommands.ExtractText(item);
                    // Save the item
                    item.Save();
                });
                afterAction();
                LogWrapper.UpdateInProgress(false);
                LogWrapper.Info($"{PythonAILibStringResources.Instance.TextExtracted}");
            });
        }



        // 自動でコンテキスト情報を付与するコマンド
        public static void CreateAutoBackgroundInfo(ContentItemWrapper item) {
            string contentText = item.Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            // 標準背景情報を生成
            CreateChatResult(item, SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
        }

        // ベクトルを更新する
        public static void DeleteEmbeddings(List<ContentItemWrapper> items) {

            // Parallelによる並列処理。4並列
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 4
            };
            Task.Run(() => {
                Parallel.ForEach(items, parallelOptions, (item) => {
                    // VectorDBItemを取得
                    VectorDBProperty folderVectorDBItem = item.GetMainVectorSearchProperty();
                    // VectorDBPropertyを削除
                    VectorDBProperty.DeleteEmbeddings(folderVectorDBItem);
                });
            });
        }

        // Embeddingを更新する
        public static void UpdateEmbeddings(List<ContentItemWrapper> items, Action beforeAction, Action afterAction) {
            beforeAction();
            // Parallelによる並列処理。4並列
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 4
            };
            Task.Run(() => {
                Parallel.ForEach(items, parallelOptions, (item) => {
                    // VectorDBItemを取得
                    VectorDBProperty folderVectorDBItem = item.GetMainVectorSearchProperty();
                    // IPythonAIFunctions.ClipboardInfoを作成
                    VectorMetadata vectorDBEntry = new(item.Id.ToString());
                    folderVectorDBItem.VectorMetadata = vectorDBEntry;

                    // タイトルとHeaderTextを追加
                    string description = item.Description + "\n" + item.HeaderText;
                    if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                        string sourcePath = item.SourcePath;
                        vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.Clipboard, "", "", "", "");
                    } else {
                        if (item.IsImage()) {
                            // 画像からテキスト抽出
                            vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.File, item.SourcePath, "", "", item.Base64Image);

                        } else {
                            vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.File, item.SourcePath, "", "", "");
                        }
                    }
                    // VectorDBPropertyを更新
                    VectorDBProperty.UpdateEmbeddings(folderVectorDBItem);
                    // ベクトル化日時を更新
                    item.VectorizedAt = DateTime.Now;
                });
                // Execute if obj is an Action
                LogWrapper.Info(PythonAILibStringResources.Instance.GenerateVectorCompleted);
                afterAction();
            });

        }


        public static void CreateAutoTitle(ContentItemWrapper item) {
            // TextとImageの場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Text || item.ContentType == ContentTypes.ContentItemTypes.Image) {
                item.Description = $"{item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ContentTypes.ContentItemTypes.Files) {
                item.Description = $"{item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += $" {PythonAILibStringResources.Instance.File}:" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += $" {PythonAILibStringResources.Instance.File}:" + item.Content;
                }
            }
        }

    }
}

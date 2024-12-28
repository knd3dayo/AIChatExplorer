using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.Content {
    public class ContentItemCommands {

        // OpenAIを使用してイメージからテキスト抽出する。
        public static void ExtractImageWithOpenAI(ContentItem item) {
            ExtractText(item);
        }

        // テキストを抽出」を実行するコマンド
        public static void ExtractTextCommandExecute(ContentItem item) {
            ExtractText(item);
            LogWrapper.Info($"{PythonAILibStringResources.Instance.TextExtracted}");
        }

        // OpenAIを使用してタイトルを生成する
        public static void CreateAutoTitleWithOpenAI(ContentItem item) {
            // ContentTypeがTextの場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                PythonAILibManager libManager = PythonAILibManager.Instance;
                // システム定義のPromptItemを取得
                PromptItem promptItem = libManager.DataFactory.GetPromptCollection<PromptItem>().FindAll().FirstOrDefault(x => x.Name == SystemDefinedPromptNames.TitleGeneration.ToString()) ?? throw new Exception("PromptItem not found");

                CreateChatResult(item, promptItem);
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
        public static void CreateChatResult(ContentItem item, string promptName) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            // システム定義のPromptItemを取得
            PromptItem promptItem = libManager.DataFactory.GetPromptCollection<PromptItem>().FindAll().FirstOrDefault(x => x.Name == promptName) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            ContentItemCommands.CreateChatResult(item, promptItem);
        }
        // 文章の信頼度を判定する
        public static void CheckDocumentReliability(ContentItem item) {

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
                VectorDBItems = item.ReferenceVectorDBItems,
                OpenAIProperties = openAIProperties
            };

            Dictionary<string, dynamic?> response = ChatUtil.CreateDictionaryChatResult(chatRequestContext, new PromptItem() {
                ChatType = OpenAIExecutionModeEnum.OpenAIRAG,
                Prompt = PromptStringResource.Instance.DocumentReliabilityDictionaryPrompt
            }, result);
            // responseからキー：reliabilityを取得
            if (response.ContainsKey("reliability") == false) {
                return;
            }
            dynamic? reliability = response["reliability"];

            int reliabilityValue = int.Parse(reliability?.ToString() ?? "0");

            // DocumentReliabilityにreliabilityを設定
            item.DocumentReliability = reliabilityValue;
            // responseからキー：reasonを取得
            if (response.ContainsKey("reason")) {
                dynamic? reason = response["reason"];
                // DocumentReliabilityReasonにreasonを設定
                item.DocumentReliabilityReason = reason?.ToString() ?? "";
            }
        }

        // PromptItemの内容でチャットを実行して結果をPromptChatResultに保存する
        public static void CreateChatResult(ContentItem item, PromptItem promptItem) {

            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(item.Content)) {
                return;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBItem> vectorDBItems = promptItem.ChatType switch {
                OpenAIExecutionModeEnum.OpenAIRAG => item.ReferenceVectorDBItems,
                OpenAIExecutionModeEnum.LangChain => item.ReferenceVectorDBItems,
                _ => []
            };
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = vectorDBItems,
                OpenAIProperties = openAIProperties
            };
            // ヘッダー情報とコンテンツ情報を結合
            // ★TODO タグ情報を追加する
            string contentText = item.HeaderText + "\n" + item.Content;

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
        public static void ExtractText(ContentItem item) {
            // キャッシュを更新
            UpdateCache(item);

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = item.ReferenceVectorDBItems,
                OpenAIProperties = openAIProperties
            };

            string base64 = item.Base64String;

            try {

                if (ContentTypes.IsImageData(base64)) {
                    string result = ChatUtil.ExtractTextFromImage(chatRequestContext, [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        item.Content = result;
                    }
                } else {
                    // ファイル名から拡張子を取得
                    string extension = Path.GetExtension(item.FilePath);
                    string text = PythonExecutor.PythonAIFunctions.ExtractBase64ToText(base64, extension);
                    item.Content = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(PythonAILibStringResources.Instance.UnsupportedFileType);
            }
        }

        // キャッシュを更新する
        // Base64Stringを参照する場合とテキスト抽出を行う場合にキャッシュを更新する。
        // 対象ファイルがない場合は何もしない。
        public static void UpdateCache(ContentItem item) {
            if (item.FilePath == null || System.IO.File.Exists(item.FilePath) == false) {
                return;
            }
            item.LastModified = new System.IO.FileInfo(item.FilePath).LastWriteTime.Ticks;

            byte[] bytes = System.IO.File.ReadAllBytes(item.FilePath);
            item.CachedBase64String = Convert.ToBase64String(bytes);
        }

        // 自動でコンテキスト情報を付与するコマンド
        public static void CreateAutoBackgroundInfo(ContentItem item) {
            string contentText = item.Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            var task1 = Task.Run(() => {
                // 標準背景情報を生成
                CreateChatResult(item, SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
                return item.PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString()); ;
            });

            // すべてのタスクが完了するまで待機
            Task.WaitAll(task1);
            // 背景情報を更新 taskの結果がNullでない場合は追加
            if (task1.Result != null) {
                item.BackgroundInfo += task1.Result;
            }
        }

        // ベクトルを更新する
        public static void UpdateEmbedding(ContentItem item, VectorDBUpdateMode mode) {

            // VectorDBItemを取得
            VectorDBItem folderVectorDBItem = item.GetMainVectorDBItem();
            // システム共通のベクトルDBを取得
            VectorDBItem systemCommonVectorDBItem = VectorDBItem.GetFolderVectorDBItem();

            if (mode == VectorDBUpdateMode.delete) {
                // IPythonAIFunctions.ClipboardInfoを作成
                VectorDBEntry vectorDBEntry = new(item.Id.ToString());

                // Embeddingを削除
                folderVectorDBItem.DeleteIndex(vectorDBEntry);
                // ★TODO システム共通のベクトルDBにも削除
                systemCommonVectorDBItem.DeleteIndex(vectorDBEntry);
                return;
            }
            if (mode == VectorDBUpdateMode.update) {
                // IPythonAIFunctions.ClipboardInfoを作成
                VectorDBEntry vectorDBEntry = new(item.Id.ToString());
                // タイトルとHeaderTextを追加
                string description = item.Description + "\n" + item.HeaderText;
                if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                    vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.Clipboard, "", "", "", "");
                    // Embeddingを保存
                    folderVectorDBItem.UpdateIndex(vectorDBEntry);
                    // ★TODO システム共通のベクトルDBにも保存
                    systemCommonVectorDBItem.UpdateIndex(vectorDBEntry);

                } else {
                    if (item.IsImage()) {
                        // 画像からテキスト抽出
                        vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.File, item.FilePath, "", "", item.Base64String);
                        // Embeddingを保存
                        folderVectorDBItem.UpdateIndex(vectorDBEntry);
                        // ★TODO システム共通のベクトルDBにも保存
                        systemCommonVectorDBItem.UpdateIndex(vectorDBEntry);

                    } else {
                        vectorDBEntry.UpdateSourceInfo(description, item.Content, VectorSourceType.File, item.FilePath, "", "", "");
                        // Embeddingを保存
                        folderVectorDBItem.UpdateIndex(vectorDBEntry);
                        // ★TODO システム共通のベクトルDBにも保存
                        systemCommonVectorDBItem.UpdateIndex(vectorDBEntry);

                    }
                }
                // ベクトル化日時を更新
                item.VectorizedAt = DateTime.Now;

            }
        }
        // Embeddingを更新する
        public static void UpdateEmbedding(ContentItem item) {
            UpdateEmbedding(item, VectorDBUpdateMode.update);
        }

    }
}

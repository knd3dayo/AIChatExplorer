using System.IO;
using System.Text.Json.Nodes;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace LibPythonAI.Utils.ExportImport {
    public class ImportExportUtil {
        // ExportPromptItemsToExcel
        public static void ExportPromptItemsToExcel(string fileName, List<PromptItem> promptItems) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // PromptItemのリスト要素毎に処理を行う
            foreach (var promptItem in promptItems) {
                List<string> row = [];
                row.Add(promptItem.Name);
                row.Add(promptItem.Description);
                row.Add(promptItem.Prompt);
                row.Add(promptItem.ChatMode.ToString());
                row.Add(promptItem.SplitMode.ToString());
                row.Add(promptItem.UseVectorDB.ToString());
                data.Add(row);
            }
            CommonDataTable dataTable = new(data);
            PythonExecutor.PythonAIFunctions.ExportToExcel(fileName, dataTable);
        }

        // ImportPromptItemsFromExcel
        public static void ImportPromptItemsFromExcel(string fileName) {
            // PythonNetの処理を呼び出す。
            CommonDataTable data = PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }
            foreach (var row in data.Rows) {
                if (row.Count == 0) {
                    continue;
                }
                PromptItemEntity promptItemEntity = new() {
                    Name = row[0],
                    Description = row[1],
                    Prompt = row[2],
                    ChatMode = (OpenAIExecutionModeEnum)Enum.Parse(typeof(OpenAIExecutionModeEnum), row[3]),
                    SplitMode = (SplitOnTokenLimitExceedModeEnum)Enum.Parse(typeof(SplitOnTokenLimitExceedModeEnum), row[4]),
                    UseVectorDB = bool.Parse(row[5])
                };
                PromptItem promptItem = new(promptItemEntity);
                promptItem.Save();
            }
        }

        // --- Export/Import
        public static void ExportToExcel(ContentFolderWrapper fromFolder, string fileName, List<ExportImportItem> items) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ClipboardItemのリスト要素毎に処理を行う
            foreach (var clipboardItem in fromFolder.GetItems<ContentItemWrapper>()) {
                List<string> row = [];
                bool exportTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
                if (exportTitle) {
                    row.Add(clipboardItem.Description);
                }
                bool exportText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;
                if (exportText) {
                    row.Add(clipboardItem.Content);
                }
                // SourcePath
                bool exportSourcePath = items.FirstOrDefault(x => x.Name == "SourcePath")?.IsChecked ?? false;
                if (exportSourcePath) {
                    row.Add(clipboardItem.SourcePath);
                }

                // PromptItemのリスト要素毎に処理を行う
                foreach (var promptItem in items.Where(x => x.IsPromptItem)) {
                    if (promptItem.IsChecked) {
                        string promptResult = clipboardItem.PromptChatResult.GetTextContent(promptItem.Name);
                        row.Add(promptResult);
                    }
                }

                data.Add(row);
            }
            CommonDataTable dataTable = new(data);

            PythonExecutor.PythonAIFunctions.ExportToExcel(fileName, dataTable);
        }
        public static void ImportFromExcel(ContentFolderWrapper fromFolder, string fileName, List<ExportImportItem> items, Action<ContentItemWrapper> afterImport) {

            // PythonNetの処理を呼び出す。
            CommonDataTable data = PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }
            List<string> targetNames = [];

            bool importTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
            if (importTitle) {
                targetNames.Add("Title");
            }
            bool importText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;
            if (importText) {
                targetNames.Add("Text");
            }
            // SourcePath
            bool importSourcePath = items.FirstOrDefault(x => x.Name == "SourcePath")?.IsChecked ?? false;
            if (importSourcePath) {
                targetNames.Add("SourcePath");
            }


            foreach (var row in data.Rows) {
                if (row.Count == 0) {
                    continue;
                }

                ContentItemWrapper item = new(fromFolder.Entity);
                // TitleのIndexが-1以外の場合は、row[TitleのIndex]をTitleに設定。Row.Countが足りない場合は空文字を設定
                int titleIndex = targetNames.IndexOf("Title");
                if (titleIndex != -1) {
                    item.Description = row[titleIndex].Replace("_x000D_", "").Replace("\n", " ");
                }
                // TextのIndexが-1以外の場合は、row[TextのIndex]をContentに設定。Row.Countが足りない場合は空文字を設定
                int textIndex = targetNames.IndexOf("Text");
                if (textIndex != -1) {
                    item.Content = row[textIndex].Replace("_x000D_", "");
                }
                // SourcePathのIndexが-1以外の場合は、row[SourcePathのIndex]をSourcePathに設定。Row.Countが足りない場合は空文字を設定
                int sourcePathIndex = targetNames.IndexOf("SourcePath");
                if (sourcePathIndex != -1) {
                    item.SourcePath = row[sourcePathIndex];
                }
                item.Save();
                afterImport(item);
            }
        }

        public static void ImportFromURLList(List<ContentItemWrapper> items, Action<ContentItemWrapper> afterImport) {
            // Parallel処理
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(items, parallelOptions, async item => {
                // SourceTypeがUrl出ない場合は何もしない
                if (item.SourceType != ContentSourceType.Url) {
                    return;
                }
                // URLを取得
                string url = item.SourcePath;
                string tempFilePath = Path.GetTempFileName();
                try {
                    string data = PythonExecutor.PythonAIFunctions.ExtractWebPage(url);
                    // 一時ファイルのパスを取得します

                    // データを一時ファイルに書き込みます
                    await System.IO.File.WriteAllTextAsync(tempFilePath, data);
                    // 成功メッセージを表示します
                    Console.WriteLine($"データは一時ファイルに保存されました: {tempFilePath}");
                    // 一時ファイルからテキスト抽出
                    string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(tempFilePath);
                    // itemのContentにTextを設定
                    item.Content = text;

                    item.Save();
                    afterImport(item);

                } catch (Exception ex) {
                    LogWrapper.Warn($"エラーが発生しました: {ex.Message}");
                } finally {
                    // 一時ファイルを削除します
                    System.IO.File.Delete(tempFilePath);
                }
            });
        }


        public static void ImportFromURLList(ContentFolderWrapper fromFolder, List<string> urls, Action<ContentItemWrapper> afterImport) {

            // Parallel処理
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };
            Parallel.ForEach(urls, parallelOptions, async url => {

                string tempFilePath = Path.GetTempFileName();
                try {
                    string data = PythonExecutor.PythonAIFunctions.ExtractWebPage(url);
                    // 一時ファイルのパスを取得します

                    // データを一時ファイルに書き込みます
                    await System.IO.File.WriteAllTextAsync(tempFilePath, data);
                    // 成功メッセージを表示します
                    Console.WriteLine($"データは一時ファイルに保存されました: {tempFilePath}");
                    // 一時ファイルからテキスト抽出
                    string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(tempFilePath);

                    // アイテムの作成
                    ContentItemWrapper item = new(fromFolder.Entity) {
                        Content = text,
                        Description = url,
                        SourcePath = url
                    };
                    item.Save();
                    afterImport(item);

                } catch (Exception ex) {
                    LogWrapper.Warn($"エラーが発生しました: {ex.Message}");
                } finally {
                    // 一時ファイルを削除します
                    System.IO.File.Delete(tempFilePath);
                }
            });
        }

        public static void ImportFromURLList(ContentFolderWrapper fromFolder, string filePath, Action<ContentItemWrapper> afterImport) {

            // filePathのファイルが存在しない場合は何もしない
            if (System.IO.File.Exists(filePath) == false) {
                LogWrapper.Error(PythonAILibStringResources.Instance.FileNotFound);
                return;
            }
            // ファイルからすべての行を読み込んでリストに格納します
            List<string> urls = new(System.IO.File.ReadAllLines(filePath));

            ImportFromURLList(fromFolder, urls, afterImport);
        }

        public static void ImportItemsFromJson(ContentFolderWrapper toFolder, string json) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                LogWrapper.Error(PythonAILibStringResources.Instance.FailedToParseJSONString);
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                LogWrapper.Error(PythonAILibStringResources.Instance.FailedToParseJSONString);
                return;
            }

            foreach (JsonObject? jsonValue in jsonArray.Cast<JsonObject?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ContentItemWrapper? item = ContentItemWrapper.FromJson(jsonString);

                if (item == null) {
                    continue;
                }
                item.Entity.FolderId = toFolder.Entity.Id;
                //保存
                item.Save();
            }
        }

    }
}

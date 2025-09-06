using System.IO;
using System.Text.Json.Nodes;
using LibMain.Data;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.Model.File;
using LibMain.Model.Prompt;
using LibMain.PythonIF;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Utils.ExportImport {
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
                row.Add(promptItem.RAGMode.ToString());
                data.Add(row);
            }
            CommonDataTable dataTable = new(data);
            PythonExecutor.PythonAIFunctions.ExportToExcelAsync(fileName, dataTable);
        }

        // ImportPromptItemsFromExcel
        public static async Task ImportPromptItemsFromExcel(string fileName) {
            // PythonNetの処理を呼び出す。
            CommonDataTable data = await PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }
            foreach (var row in data.Rows) {
                if (row.Count == 0) {
                    continue;
                }
                PromptItem promptItem = new() {
                    Name = row[0],
                    Description = row[1],
                    Prompt = row[2],
                    ChatMode = (OpenAIExecutionModeEnum)Enum.Parse(typeof(OpenAIExecutionModeEnum), row[3]),
                    SplitMode = (SplitModeEnum)Enum.Parse(typeof(SplitModeEnum), row[4]),
                    RAGMode =  (RAGModeEnum)Enum.Parse(typeof(RAGModeEnum), row[5])
                };
                promptItem.SaveAsync();
            }
        }

        // --- Export/Import
        public static async Task ExportToExcel(ContentFolderWrapper fromFolder, string fileName, List<ContentItemDataDefinition> items) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ApplicationItemのリスト要素毎に処理を行う
            foreach (var applicationItem in await fromFolder.GetItemsAsync<ContentItem>(isSync: false)) {
                List<string> row = [];
                bool exportTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
                if (exportTitle) {
                    row.Add(applicationItem.Description);
                }
                bool exportText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;
                if (exportText) {
                    row.Add(applicationItem.Content);
                }
                // Path
                bool exportSourcePath = items.FirstOrDefault(x => x.Name == "SourcePath")?.IsChecked ?? false;
                if (exportSourcePath) {
                    row.Add(applicationItem.SourcePath);
                }

                // PromptItemのリスト要素毎に処理を行う
                foreach (var promptItem in items.Where(x => x.IsPromptItem)) {
                    if (promptItem.IsChecked) {
                        string promptResult = applicationItem.PromptChatResult.GetTextContent(promptItem.Name);
                        row.Add(promptResult);
                    }
                }

                data.Add(row);
            }
            CommonDataTable dataTable = new(data);

            await PythonExecutor.PythonAIFunctions.ExportToExcelAsync(fileName, dataTable);
        }
        public static async Task<List<ContentItem>> ImportFromExcel(ContentFolderWrapper fromFolder, string fileName, List<ContentItemDataDefinition> items) {

            List<ContentItem> resultItems = [];

            // PythonNetの処理を呼び出す。
            CommonDataTable data = await PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return resultItems;
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
            // Path
            bool importSourcePath = items.FirstOrDefault(x => x.Name == "SourcePath")?.IsChecked ?? false;
            if (importSourcePath) {
                targetNames.Add("SourcePath");
            }


            foreach (var row in data.Rows) {
                if (row.Count == 0) {
                    continue;
                }

                ContentItem item = new(fromFolder.Entity);
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
                await item.SaveAsync();
                resultItems.Add(item);
            }
            return resultItems;
        }

        public static async Task ImportFromURLListAsync(List<ContentItem> items, Action<ContentItem> afterImport) {
            var tasks = items
                .Where(item => item.SourceType == ContentSourceType.Url)
                .Select(async item => {
                    string url = item.SourcePath;
                    string tempFilePath = Path.GetTempFileName();
                    try {
                        string data = await PythonExecutor.PythonAIFunctions.ExtractWebPage(url);
                        await System.IO.File.WriteAllTextAsync(tempFilePath, data);
                        Console.WriteLine($"データは一時ファイルに保存されました: {tempFilePath}");
                        string text = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(tempFilePath);
                        item.Content = text;
                        await item.SaveAsync();
                        afterImport(item);
                    } catch (Exception ex) {
                        LogWrapper.Warn($"エラーが発生しました: {ex.Message}");
                    } finally {
                        System.IO.File.Delete(tempFilePath);
                    }
                });
            await Task.WhenAll(tasks);
        }


        public static async Task<List<ContentItem>> ImportFromURLListAsync(ContentFolderWrapper fromFolder, List<string> urls) {
            List<ContentItem> resultItems = [];
            var tasks = urls.Select(async url => {
                string tempFilePath = Path.GetTempFileName();
                try {
                    string data = await PythonExecutor.PythonAIFunctions.ExtractWebPage(url);
                    await System.IO.File.WriteAllTextAsync(tempFilePath, data);
                    Console.WriteLine($"データは一時ファイルに保存されました: {tempFilePath}");
                    string text = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(tempFilePath);
                    ContentItem item = new(fromFolder.Entity) {
                        Content = text,
                        Description = url,
                        SourcePath = url
                    };
                    await item.SaveAsync();
                    resultItems.Add(item);
                } catch (Exception ex) {
                    LogWrapper.Warn($"エラーが発生しました: {ex.Message}");
                } finally {
                    System.IO.File.Delete(tempFilePath);
                }
            });
            await Task.WhenAll(tasks);
            return resultItems;
        }

        public static async Task<List<ContentItem>> ImportFromURLListAsync(ContentFolderWrapper fromFolder, string filePath) {
            List<ContentItem> resultItems = [];
            // filePathのファイルが存在しない場合は何もしない
            if (System.IO.File.Exists(filePath) == false) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.FileNotFound);
                return resultItems;
            }
            // ファイルからすべての行を読み込んでリストに格納します
            List<string> urls = new(System.IO.File.ReadAllLines(filePath));
            resultItems = await ImportFromURLListAsync(fromFolder, urls);
            return resultItems;
        }

        public static async Task ImportItemsFromJson(ContentFolderWrapper toFolder, string json) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.FailedToParseJSONString);
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.FailedToParseJSONString);
                return;
            }

            var tasks = new List<Task>();
            foreach (JsonObject? jsonValue in jsonArray.Cast<JsonObject?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ContentItem? item = ContentItem.FromJson(jsonString);

                if (item == null) {
                    continue;
                }
                item.Entity.FolderId = toFolder.Id;
                //保存
                tasks.Add(item.SaveAsync());
            }
            await Task.WhenAll(tasks);
        }

    }
}

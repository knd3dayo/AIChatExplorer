using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Utils.ExportImport {
    public  class ImportExportUtil {

        // --- Export/Import
        public static void ExportToExcel(ContentFolder fromFolder, string fileName, List<ExportImportItem> items) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ClipboardItemのリスト要素毎に処理を行う
            foreach (var clipboardItem in fromFolder.GetItems<ContentItem>()) {
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
        public static void ImportFromExcel(ContentFolder fromFolder, string fileName, List<ExportImportItem> items, Action<ContentItem> afterImport) {

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

                ContentItem item = new(fromFolder.Id);
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


        public static void ImportFromURLList(ContentFolder fromFolder, string filePath, Action<ContentItem> afterImport) {

            // filePathのファイルが存在しない場合は何もしない
            if (System.IO.File.Exists(filePath) == false) {
                LogWrapper.Error(PythonAILibStringResources.Instance.FileNotFound);
                return;
            }
            // ファイルからすべての行を読み込んでリストに格納します
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(filePath));
            Task[] tasks = [];

            foreach (var line in lines) {
                // URLを取得
                string url = line;
                // URLからデータを取得して一時ファイルに保存
                // Task.Runを使用して非同期操作を開始します
                Task task = Task.Run(async () => {
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
                        ContentItem item = new(fromFolder.Id) {
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
                tasks.Append(task);
            }
            // すべてのタスクが完了するまで待機します
            Task.WaitAll(tasks);

        }

        public static void ImportItemsFromJson(ContentFolder toFolder, string json) {
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
                ContentItem? item = ContentItem.FromJson<ContentItem>(jsonString);

                if (item == null) {
                    continue;
                }
                item.CollectionId = toFolder.Id;
                //保存
                item.Save();
            }
        }

    }
}

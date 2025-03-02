using System.Collections.ObjectModel;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ContentFolderWrapper targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [];
            using var db = new PythonAILibDBContext();
            var items = db.AutoProcessRules.Where(x => x.TargetFolder == targetFolder.Entity);

            foreach (var item in items) {
                if (item != null) {
                    rules.Add(new AutoProcessRule(item));
                }
            }
            return rules;

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            using var db = new PythonAILibDBContext();
            var items = db.AutoProcessRules.Where(x => x.RuleAction != null
                    && (x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.CopyToFolder.ToString()
                        || x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.MoveToFolder.ToString()));
            foreach (var item in items) {
                if (item != null) {
                    yield return new AutoProcessRule(item);
                }
            }

        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        public static async Task<ContentItemWrapper> ApplyGlobalAutoAction(ContentItemWrapper item) {

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;


            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text && lineCount <= configParams.IgnoreLineCount()) {
                return item;
            }
            // If AutoFileExtract is set, extract files
            if (configParams.AutoFileExtract() && item.SourceType == ContentSourceType.File) {
                string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.SourcePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (configParams.AutoExtractImageWithPyOCR()) {
                    string extractImageText = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(item.Image, configParams.TesseractExePath());
                    item.Content += "\n" + extractImageText;
                    LogWrapper.Info(PythonAILibStringResources.Instance.OCR);

                } else if (configParams.AutoExtractImageWithOpenAI()) {

                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoExtractImageText);
                    ContentItemCommands.ExtractImageWithOpenAI(item);
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (configParams.AutoTag()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTag);
                    // ClipboardItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run(() => {
                // If AUTO_DESCRIPTION is set, automatically set the DisplayText
                if (configParams.AutoTitle()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitle(item);

                } else if (configParams.AutoTitleWithOpenAI()) {

                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitleWithOpenAI(item);
                }
            });
            var task3 = Task.Run(() => {
                // 背景情報
                if (configParams.AutoBackgroundInfo()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetBackgroundInfo);
                    ContentItemCommands.CreateAutoBackgroundInfo(item);
                }
            });
            var task4 = Task.Run(() => {
                // サマリー
                if (configParams.AutoSummary()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateSummary);
                    ContentItemCommands.CreateChatResult(item, SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(() => {
                // Tasks
                if (configParams.AutoGenerateTasks()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateTaskList);
                    ContentItemCommands.CreateChatResult(item, SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(() => {
                // Tasks
                if (configParams.AutoDocumentReliabilityCheck()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCheckDocumentReliability);
                    ContentItemCommands.CheckDocumentReliability(item);
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }

        // 自動処理を適用する処理
        public static ContentItemWrapper? ApplyFolderAutoAction(ContentItemWrapper item) {

            ContentItemWrapper? result = item;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(item.GetFolder());
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info($"{PythonAILibStringResources.Instance.ApplyAutoProcessing} {rule.GetDescriptionString()}");
                rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.ItemsDeletedByAutoProcessing);
                    return null;
                }
            }
            return result;
        }


    }
}

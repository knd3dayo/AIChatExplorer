using System.Collections.ObjectModel;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ContentFolderWrapper targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [];
            using var db = new PythonAILibDBContext();
            var items = AutoProcessRule.GetItemByTargetFolder(targetFolder);
            return [.. items];

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static ObservableCollection<AutoProcessRule> GetCopyToMoveToRules() {
            return [ .. AutoProcessRule.GetCopyToMoveToRules()];
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        public static async Task<ContentItemWrapper> ApplyGlobalAutoActionAsync(ContentItemWrapper item) {

            IPythonAILibConfigParams configParams = PythonAILibManager.Instance.ConfigParams;


            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text && lineCount <= configParams.IgnoreLineCount()) {
                return item;
            }
            // If AutoFileExtract is set, extract files
            if (configParams.AutoFileExtract() && item.SourceType == ContentSourceType.File) {
                string text = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(item.SourcePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (configParams.AutoExtractImageWithOpenAI()) {

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
            var task2 = Task.Run( async () => {
                // If AUTO_DESCRIPTION is set, automatically set the DisplayText
                if (configParams.AutoTitle()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitle(item);

                } else if (configParams.AutoTitleWithOpenAI()) {

                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetTitle);
                    await PromptItem.CreateAutoTitleWithOpenAIAsync(item);
                }
            });
            var task3 = Task.Run(async () => {
                // 背景情報
                if (configParams.AutoBackgroundInfo()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoSetBackgroundInfo);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
                }
            });
            var task4 = Task.Run(async () => {
                // サマリー
                if (configParams.AutoSummary()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateSummary);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(async () => {
                // Tasks
                if (configParams.AutoGenerateTasks()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCreateTaskList);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(async () => {
                // Tasks
                if (configParams.AutoDocumentReliabilityCheck()) {
                    LogWrapper.Info(PythonAILibStringResources.Instance.AutoCheckDocumentReliability);
                    await PromptItem.CheckDocumentReliability(item);
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

using System.Collections.ObjectModel;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.PythonIF;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.AutoProcess {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static async Task<ObservableCollection<AutoProcessRule>> GetAutoProcessRules(ContentFolderWrapper? targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [];
            var items = await AutoProcessRule.GetItemByTargetFolder(targetFolder);
            return [.. items];

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static async Task<ObservableCollection<AutoProcessRule>> GetCopyToMoveToRules() {
            var rules = await AutoProcessRule.GetCopyToMoveToRules();
            return [ .. rules];
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
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text && lineCount <= configParams.GetIgnoreLineCount()) {
                return item;
            }

            // If IsAutoFileExtractEnabled is set, extract files
            if (configParams.IsAutoFileExtractEnabled() && item.SourceType == ContentSourceType.File) {
                string text = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(item.SourcePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (configParams.IsAutoExtractImageWithOpenAIEnabled()) {

                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoExtractImageText);
                    await ContentItemCommands.ExtractImageWithOpenAIAsync(item);
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (configParams.IsAutoTagEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTag);
                    // ApplicationItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run( async () => {
                // If AUTO_DESCRIPTION is set, automatically set the DisplayText
                if (configParams.IsAutoTitleEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                    ContentItemCommands.CreateAutoTitle(item);

                } else if (configParams.IsAutoTitleWithOpenAIEnabled()) {

                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                    await PromptItem.CreateAutoTitleWithOpenAIAsync(item);
                }
            });
            var task3 = Task.Run(async () => {
                // 背景情報
                if (configParams.IsAutoBackgroundInfoEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetBackgroundInfo);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
                }
            });
            var task4 = Task.Run(async () => {
                // サマリー
                if (configParams.IsAutoSummaryEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCreateSummary);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(async () => {
                // Tasks
                if (configParams.IsAutoGenerateTasksEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCreateTaskList);
                    await PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(async () => {
                // Tasks
                if (configParams.IsAutoDocumentReliabilityCheckEnabled()) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCheckDocumentReliability);
                    await PromptItem.CheckDocumentReliability(item);
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }

        // 自動処理を適用する処理
        public static async Task<ContentItemWrapper?> ApplyFolderAutoAction(ContentItemWrapper item) {

            ContentItemWrapper? result = item;
            // AutoProcessRulesを取得
            var folder = await item.GetFolderAsync();
            var AutoProcessRules = await GetAutoProcessRules(folder);
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info($"{PythonAILibStringResourcesJa.Instance.ApplyAutoProcessing} {rule.GetDescriptionString()}");
                await rule.RunActionAsync(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    LogWrapper.Info(PythonAILibStringResourcesJa.Instance.ItemsDeletedByAutoProcessing);
                    return null;
                }
            }
            return result;
        }


    }
}

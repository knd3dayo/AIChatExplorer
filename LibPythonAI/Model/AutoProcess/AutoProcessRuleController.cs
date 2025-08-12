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


            var tasks = new List<Task>();

            // If AUTO_TAG is set, automatically set the tags (CPUバウンド想定ならTask.Run、そうでなければ直接呼び出し)
            if (configParams.IsAutoTagEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTag);
                // ApplicationItem.CreateAutoTags(item); // CPUバウンドならTask.Runでラップ
                // tasks.Add(Task.Run(() => ApplicationItem.CreateAutoTags(item)));
            }

            // If AUTO_DESCRIPTION is set, automatically set the DisplayText
            if (configParams.IsAutoTitleEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                ContentItemCommands.CreateAutoTitle(item);
            } else if (configParams.IsAutoTitleWithOpenAIEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetTitle);
                tasks.Add(PromptItem.CreateAutoTitleWithOpenAIAsync(item));
            }

            // 背景情報
            if (configParams.IsAutoBackgroundInfoEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoSetBackgroundInfo);
                tasks.Add(PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.BackgroundInformationGeneration.ToString()));
            }

            // サマリー
            if (configParams.IsAutoSummaryEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCreateSummary);
                tasks.Add(PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.SummaryGeneration.ToString()));
            }

            // Tasks
            if (configParams.IsAutoGenerateTasksEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCreateTaskList);
                tasks.Add(PromptItem.CreateChatResultAsync(item, SystemDefinedPromptNames.TasksGeneration.ToString()));
            }

            // Document Reliability Check
            if (configParams.IsAutoDocumentReliabilityCheckEnabled()) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AutoCheckDocumentReliability);
                tasks.Add(PromptItem.CheckDocumentReliability(item));
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

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

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.AutoProcess {

    public class AutoProcessRule {


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        public string? AutoProcessItemId { get; set; }

        public string? TargetFolderId { get; set; }

        public string? DestinationFolderId { get; set; }


        public string ConditionsJson { get; set; } = "[]";

        private List<AutoProcessRuleCondition>? _conditions;
        public List<AutoProcessRuleCondition> Conditions {
            get {
                if (_conditions == null) {
                    List<Dictionary<string, dynamic?>> dict = JsonUtil.ParseJsonArray(ConditionsJson);
                    _conditions = AutoProcessRuleCondition.FromDictList(dict);
                }
                return _conditions;
            }
            set {
                _conditions = value;
            }

        }



        public async Task<AutoProcessItem?> GetRuleAction() {
            return await AutoProcessItem.GetItemById(AutoProcessItemId);
        }

        public void SetRuleAction(AutoProcessItem? autoProcessItem) {
            AutoProcessItemId = autoProcessItem?.Id;
        }

        public async Task<ContentFolderWrapper?> GetDestinationFolder() {
            return await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(DestinationFolderId);
        }

        public async Task<ContentFolderWrapper?> GetTargetFolder() {
            return await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(TargetFolderId);
        }

        // Load
        private static bool _isInitialized = false;

        private static List<AutoProcessRule> _items = [];
        public static async Task LoadItemsAsync() {
            // PythonExecutor.PythonAIFunctions.GetAutoProcessRulesAsync() が非同期メソッドであれば直接awaitする
            _items = await PythonExecutor.PythonAIFunctions.GetAutoProcessRulesAsync();
            if (_items != null) {
                _isInitialized = true;
            }
        }
        public static async Task<List<AutoProcessRule>> GetItems() {
            if (!_isInitialized) {
                await LoadItemsAsync();
            }
            return _items;
        }
        //SaveAsync
        public async Task SaveAsync() {
            // ConditionsJsonを更新
            ConditionsJson = JsonSerializer.Serialize(Conditions, JsonUtil.JsonSerializerOptions);
            // 優先順位が-1の場合は、最大の優先順位を取得して設定
            if (Priority == -1) {
                var items = await GetItems();
                Priority = items.Count + 1;
            }
            await PythonExecutor.PythonAIFunctions.UpdateAutoProcessRuleAsync(new AutoProcessRuleRequest(this));
        }
        // DeleteAsync
        public async Task DeleteAsync() {
            await PythonExecutor.PythonAIFunctions.DeleteAutoProcessRuleAsync(new AutoProcessRuleRequest(this));
        }

        // RuleConditionTypesの条件に全てマッチした場合にTrueを返す。マッチしない場合とルールがない場合はFalseを返す。
        public bool IsMatch(ContentItemWrapper applicationItem) {
            if (Conditions.Count == 0) {
                return false;
            }
            // IsAllItemsRuleが含まれるかどうか
            if (Conditions.Any(c => c.Type == AutoProcessRuleCondition.ConditionTypeEnum.AllItems)) {
                return true;
            }
            // 全ての条件を満たすかどうか
            foreach (var condition in Conditions) {
                if (!condition.CheckCondition(applicationItem)) {
                    return false;
                }
            }
            return true;
        }

        // 条件にマッチした場合にRunActionを実行する
        public async Task RunActionAsync(ContentItemWrapper applicationItem) {
            // ルールが有効でない場合はそのまま返す
            if (!IsEnabled) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.RuleNameIsInvalid(RuleName));
                return;
            }

            if (!IsMatch(applicationItem)) {
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.NoMatch);
                return;
            }
            // DestinationIdに一致するフォルダを取得
            var ruleAction = await GetRuleAction();
            if (ruleAction == null) {
                LogWrapper.Warn(PythonAILibStringResourcesJa.Instance.NoActionSet);
                return;
            }
            var DestinationFolder = await GetDestinationFolder();
            await ruleAction.ExecuteAsync(applicationItem, DestinationFolder);
        }

        public async Task<string> GetDescriptionString() {
            string result = $"{PythonAILibStringResourcesJa.Instance.Condition}\n";
            var ruleAction = await GetRuleAction();
            foreach (var condition in Conditions) {
                // ConditionTypeごとに処理
                switch (condition.Type) {
                    case AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains:
                        result += PythonAILibStringResourcesJa.Instance.DescriptionContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.ContentContains:
                        result += PythonAILibStringResourcesJa.Instance.ContentContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains:
                        result += PythonAILibStringResourcesJa.Instance.SourceApplicationNameContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains:
                        result += PythonAILibStringResourcesJa.Instance.SourceApplicationTitleContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationPathContains:
                        result += PythonAILibStringResourcesJa.Instance.SourceApplicationPathContains(condition.Keyword) + "\n";
                        break;
                }
                // AutoProcessItemが設定されている場合
                if (ruleAction != null) {
                    result += $"{PythonAILibStringResourcesJa.Instance.Action}:{ruleAction.Description}\n";
                } else {
                    result += $"{PythonAILibStringResourcesJa.Instance.ActionNone}\n";
                }
                // TypeValue が CopyToFolderまたはMoveToFolderの場合
                if (ruleAction != null && ruleAction.IsCopyOrMoveAction()) {

                    var destinationFolder = await GetDestinationFolder();
                    if (destinationFolder != null) {
                        var contentFolderPath = await destinationFolder.GetContentFolderPath();
                        result += $"{PythonAILibStringResourcesJa.Instance.Folder}:{contentFolderPath}\n";
                    } else {
                        result += $"{PythonAILibStringResourcesJa.Instance.FolderNone}\n";
                    }
                }
            }
            return result;

        }
        // 無限ループなコピーまたは移動の可能性をチェックする
        public static async Task<bool> CheckInfiniteLoop(AutoProcessRule rule) {
            // ruleがNullの場合はFalseを返す
            if (rule == null) {
                return false;
            }
            var ruleAction = await rule.GetRuleAction();
            // rule.RuleActionがNullの場合はFalseを返す
            if (ruleAction == null) {
                return false;
            }
            // ruleがCopyToFolderまたはMoveToFolder以外の場合はFalseを返す
            if (ruleAction.IsCopyOrMoveAction() == false) {
                return false;
            }
            IEnumerable<AutoProcessRule> copyToMoveToRules = await AutoProcessRuleController.GetCopyToMoveToRules();

            // ルールがない場合はFalseを返す
            if (!copyToMoveToRules.Any()) {
                return false;
            }
            // copyToMoveToRulesにRuleを追加
            copyToMoveToRules = copyToMoveToRules.Append(rule);

            // fromとtoを格納するDictionary
            Dictionary<string, List<string>> fromToDictionary = [];
            foreach (var r in copyToMoveToRules) {
                // TargetFolderとDestinationFolderが設定されている場合
                var targetFolder = await r.GetTargetFolder();
                var destinationFolder = await r.GetDestinationFolder();
                if (targetFolder != null && destinationFolder != null) {
                    // keyが存在しない場合は新しいLinkedListを作成
                    var contentFolderPath = await targetFolder.GetContentFolderPath();
                    if (!fromToDictionary.TryGetValue(contentFolderPath, out List<string>? value)) {
                        value = [];
                        fromToDictionary[contentFolderPath] = value;
                    }

                    value.Add(contentFolderPath);
                }
            }

            // fromToDictionaryの中でルールが存在するかどうかを再帰的にチェックする
            foreach (var from in fromToDictionary.Keys) {
                // PathListを作成
                List<string> pathList = [];
                // ルールが存在する場合はTrueを返す
                if (CheckInfiniteLoopRecursive(fromToDictionary, from, pathList)) {
                    return true;
                }
            }
            return false;

        }
        // Dictionaryの中でルールが存在するかどうかを再帰的にチェックする
        public static bool CheckInfiniteLoopRecursive(Dictionary<string, List<string>> fromToDictionary, string from, List<string> pathList) {
            // PathListのコピーを作成
            pathList = new(pathList) {
                // PathListにFromを追加する。
                from
            };
            // PathList内に重複があるかどうかをチェック。重複がある場合はTrueを返す
            if (pathList.Distinct().Count() != pathList.Count) {
                LogWrapper.Warn($"{PythonAILibStringResourcesJa.Instance.DetectedAnInfiniteLoop}\n{Tools.ListToString(pathList)}");
                return true;
            }
            // fromToDictionaryのうちKeyがFromのものを取得
            if (fromToDictionary.TryGetValue(from, out List<string>? value)) {
                // FromのValueを取得
                var toList = value;
                foreach (var to in toList) {
                    // ToをFromにして再帰的にチェック
                    bool result = CheckInfiniteLoopRecursive(fromToDictionary, to, pathList);
                    if (result) {
                        return true;
                    }
                }
            }
            // Fromが存在しない場合はPathList内に重複があるかどうかをチェック。重複がある場合はTrueを返す
            return pathList.Distinct().Count() != pathList.Count;

        }
        // 指定したAutoProcessRuleの優先順位を上げる
        public static async Task UpPriority(AutoProcessRule autoProcessRule) {
            var items = await GetItems();
            List<AutoProcessRule> autoProcessRules = items.ToList();
            // 引数のAutoProcessRuleのIndexを取得
            int index = autoProcessRules.FindIndex(r => r.Id == autoProcessRule.Id);
            // indexが0以下の場合は何もしない
            if (index <= 0) {
                return;
            }
            // 優先順位を入れ替える
            AutoProcessRule temp = autoProcessRules[index];
            autoProcessRules[index] = autoProcessRules[index - 1];
            autoProcessRules[index - 1] = temp;
            // 優先順位を再設定
            for (int i = 0; i < autoProcessRules.Count; i++) {
                autoProcessRules[i].Priority = i + 1;
                // 保存
                await autoProcessRules[i].SaveAsync();
            }

        }
        // 指定したAutoProcessRuleの優先順位を下げる
        public static async Task DownPriority(AutoProcessRule autoProcessRule) {
            var items = await GetItems();
            List<AutoProcessRule> autoProcessRules = items.ToList();
            // 引数のAutoProcessRuleのIndexを取得
            int index = autoProcessRules.FindIndex(r => r.Id == autoProcessRule.Id);
            // indexがリストの最大Index以上の場合は何もしない
            if (index >= autoProcessRules.Count - 1) {
                return;
            }
            // 優先順位を入れ替える
            AutoProcessRule temp = autoProcessRules[index];
            autoProcessRules[index] = autoProcessRules[index + 1];
            autoProcessRules[index + 1] = temp;
            // 優先順位を再設定
            for (int i = 0; i < autoProcessRules.Count; i++) {
                autoProcessRules[i].Priority = i + 1;
                // 保存
                await autoProcessRules[i].SaveAsync();
            }
        }
        // GetItemsByRuleName
        public static async Task<List<AutoProcessRule>> GetItemsByRuleName(string? ruleName) {
            var items = await GetItems();
            return items.Where(x => x.RuleName == ruleName).ToList();
        }
        // GetItemsByTargetFolder
        public static async Task<List<AutoProcessRule>> GetItemByTargetFolder(ContentFolderWrapper? targetFolder) {
            var items = await GetItems();
            return items.Where(x => x.TargetFolderId == targetFolder?.Id).ToList();
        }

        // ToDict
        public Dictionary<string, object> ToDict() {
            var dict = new Dictionary<string, object> {
                { "id", Id },
                { "rule_name", RuleName },
                { "is_enabled", IsEnabled },
                { "priority", Priority },
                { "conditions_json", ConditionsJson }
            };
            if (AutoProcessItemId != null) {
                dict.Add("auto_process_item_id", AutoProcessItemId);
            }
            if (TargetFolderId != null) {
                dict.Add("target_folder_id", TargetFolderId);
            }
            if (DestinationFolderId != null) {
                dict.Add("destination_folder_id", DestinationFolderId);
            }

            return dict;

        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<AutoProcessRule> rules) {
            List<Dictionary<string, object>> dictList = [];
            foreach (var rule in rules) {
                dictList.Add(rule.ToDict());
            }
            return dictList;
        }

        // FromDict
        public static AutoProcessRule FromDict(Dictionary<string, object> dict) {
            AutoProcessRule rule = new() {
                Id = dict["id"] as string ?? Guid.NewGuid().ToString(),
                RuleName = dict["rule_name"] as string ?? "",
                IsEnabled = (bool)(dict["is_enabled"] ?? true),
                Priority = (int)(dict["priority"] ?? -1),
                AutoProcessItemId = dict["auto_process_item_id"] as string,
                TargetFolderId = dict["target_folder_id"] as string,
                DestinationFolderId = dict["destination_folder_id"] as string,
                ConditionsJson = dict["conditions_json"] as string ?? "[]"
            };
            return rule;
        }


        // GetCopyToMoveToRules
        public static async Task<List<AutoProcessRule>> GetCopyToMoveToRules() {
            var copyRules = await GetItemsByRuleName(AutoProcessActionTypeEnum.CopyToFolder.ToString());
            var moveRules = await GetItemsByRuleName(AutoProcessActionTypeEnum.MoveToFolder.ToString());

            List<AutoProcessRule> rules = [];
            rules.AddRange(copyRules);
            rules.AddRange(moveRules);
            return rules;
        }
    }
}

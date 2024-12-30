using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.AutoProcess {

    public class AutoProcessRule {
        public ObjectId Id { get; set; } = ObjectId.Empty;

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        public List<AutoProcessRuleCondition> Conditions { get; set; } = [];

        public SystemAutoProcessItem? RuleAction { get; set; }

        public ObjectId TargetFolderId { get; set; } = ObjectId.Empty;

        // 移動またはコピー先のフォルダ
        public ObjectId DestinationFolderId { get; set; } = ObjectId.Empty;

        [BsonIgnore]
        public ContentFolder? DestinationFolder {
            get {
                return ContentFolder.GetFolderById<ContentFolder>(DestinationFolderId);
            }
        }
        [BsonIgnore]
        public ContentFolder? TargetFolder {
            get {
                return ContentFolder.GetFolderById<ContentFolder>(TargetFolderId);
            }
        }

        public AutoProcessRule() {
        }

        /// <summary>
        /// 指定した名前のルールを作成する
        /// </summary>
        /// <param name="ruleName"></param>
        public AutoProcessRule(string ruleName) {
            RuleName = ruleName;
        }

        // 保存
        public void Save() {
            // 優先順位が-1の場合は、最大の優先順位を取得して設定
            if (Priority == -1) {
                Priority = GetAllAutoProcessRules().Count() + 1;
            }
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoProcessRuleCollection();
            collection.Upsert(this);
        }
        // 削除
        public void Delete() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoProcessRuleCollection();
            collection.Delete(Id);
            // 削除後はIdをNullにする
            Id = ObjectId.Empty;
        }
        // 取得
        public static IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoProcessRuleCollection();
            return collection.FindAll().OrderBy(x => x.Priority);
        }


        // RuleConditionTypesの条件に全てマッチした場合にTrueを返す。マッチしない場合とルールがない場合はFalseを返す。
        public bool IsMatch(ContentItem clipboardItem) {
            if (Conditions.Count == 0) {
                return false;
            }
            // IsAllItemsRuleが含まれるかどうか
            if (Conditions.Any(c => c.Type == AutoProcessRuleCondition.ConditionTypeEnum.AllItems)) {
                return true;
            }
            // 全ての条件を満たすかどうか
            foreach (var condition in Conditions) {
                if (!condition.CheckCondition(clipboardItem)) {
                    return false;
                }
            }
            return true;
        }

        // 条件にマッチした場合にRunActionを実行する
        public void RunAction(ContentItem clipboardItem) {
            // ルールが有効でない場合はそのまま返す
            if (!IsEnabled) {
                LogWrapper.Info(PythonAILibStringResources.Instance.RuleNameIsInvalid(RuleName));
                return;
            }

            if (!IsMatch(clipboardItem)) {
                LogWrapper.Info(PythonAILibStringResources.Instance.NoMatch);
                return;
            }
            if (RuleAction == null) {
                LogWrapper.Warn(PythonAILibStringResources.Instance.NoActionSet);
                return;
            }
            // DestinationIdに一致するフォルダを取得
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetFolderCollection<ContentFolder>();
            ContentFolder? destinationFolder = collection.FindById(DestinationFolderId);

            RuleAction.Execute(clipboardItem, destinationFolder);
        }

        public string GetDescriptionString() {
            string result = $"{PythonAILibStringResources.Instance.Condition}\n";
            foreach (var condition in Conditions) {
                // ConditionTypeごとに処理
                switch (condition.Type) {
                    case AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains:
                        result += PythonAILibStringResources.Instance.DescriptionContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.ContentContains:
                        result += PythonAILibStringResources.Instance.ContentContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains:
                        result += PythonAILibStringResources.Instance.SourceApplicationNameContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains:
                        result += PythonAILibStringResources.Instance.SourceApplicationTitleContains(condition.Keyword) + "\n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationPathContains:
                        result += PythonAILibStringResources.Instance.SourceApplicationPathContains(condition.Keyword) + "\n";
                        break;
                }
                // AutoProcessItemが設定されている場合
                if (RuleAction != null) {
                    result += $"{PythonAILibStringResources.Instance.Action}:{RuleAction.Description}\n";
                } else {
                    result += $"{PythonAILibStringResources.Instance.ActionNone}\n";
                }
                // TypeValue が CopyToFolderまたはMoveToFolderの場合
                if (RuleAction != null && RuleAction.IsCopyOrMoveOrMergeAction()) {
                    // DestinationFolderが設定されている場合
                    // DestinationIdに一致するフォルダを取得
                    PythonAILibManager libManager = PythonAILibManager.Instance;
                    var collection = libManager.DataFactory.GetFolderCollection<ContentFolder>();
                    ContentFolder? destinationFolder = collection.FindById(DestinationFolderId);

                    if (destinationFolder != null) {
                        result += $"{PythonAILibStringResources.Instance.Folder}:{destinationFolder.FolderPath}\n";
                    } else {
                        result += $"{PythonAILibStringResources.Instance.FolderNone}\n";
                    }
                }
            }
            return result;

        }
        // 無限ループなコピーまたは移動の可能性をチェックする
        public static bool CheckInfiniteLoop(AutoProcessRule rule) {
            // ruleがNullの場合はFalseを返す
            if (rule == null) {
                return false;
            }
            // rule.RuleActionがNullの場合はFalseを返す
            if (rule.RuleAction == null) {
                return false;
            }
            // ruleがCopyToFolderまたはMoveToFolder以外の場合はFalseを返す
            if (rule.RuleAction.IsCopyOrMoveOrMergeAction() == false) {
                return false;
            }
            IEnumerable<AutoProcessRule> copyToMoveToRules = AutoProcessRuleController.GetCopyToMoveToRules();

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
                if (r.TargetFolderId != null && r.DestinationFolderId != null) {
                    // keyが存在しない場合は新しいLinkedListを作成
                    if (!fromToDictionary.TryGetValue(r.TargetFolderId.ToString(), out List<string>? value)) {
                        value = [];
                        fromToDictionary[r.TargetFolderId.ToString()] = value;
                    }

                    value.Add(r.DestinationFolderId.ToString());
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
                LogWrapper.Warn($"{PythonAILibStringResources.Instance.DetectedAnInfiniteLoop}\n{Tools.ListToString(pathList)}");
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
        public static void UpPriority(AutoProcessRule autoProcessRule) {
            List<AutoProcessRule> autoProcessRules = GetAllAutoProcessRules().ToList();
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
                autoProcessRules[i].Save();
            }

        }
        // 指定したAutoProcessRuleの優先順位を下げる
        public static void DownPriority(AutoProcessRule autoProcessRule) {
            List<AutoProcessRule> autoProcessRules = GetAllAutoProcessRules().ToList();
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
                autoProcessRules[i].Save();
            }
        }
    }
}

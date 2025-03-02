using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Model.AutoProcess {

    public class AutoProcessRule {

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        public List<AutoProcessRuleCondition> Conditions { get; set; } = [];

        public AutoProcessItem? RuleAction {
            get {
                var item = Entity.RuleAction;
                if (item == null) {
                    return null;
                }
                return new AutoProcessItem(item);
            }
            set {
                Entity.RuleAction = value?.AutoProcessItemInstance;
            }
        }


        public ContentFolderWrapper? DestinationFolder {
            get {
                if (Entity.DestinationFolder == null) {
                    return null;
                }
                return new ContentFolderWrapper(Entity.DestinationFolder);
            }
            set {
                Entity.DestinationFolder = value?.Entity;
            }
        }

        public ContentFolderWrapper? TargetFolder {
            get {
                if (Entity.TargetFolder == null) {
                    return null;
                }
                return new ContentFolderWrapper(Entity.TargetFolder);
            }
        }

        public AutoProcessRule(AutoProcessRuleEntity autoProcessRuleEntity) {
            Entity = autoProcessRuleEntity;
        }

        public AutoProcessRuleEntity Entity { get; set; }


        /// <summary>
        /// 指定した名前のルールを作成する
        /// </summary>
        /// <param name="ruleName"></param>
        public AutoProcessRule(string ruleName) {
            Entity = new AutoProcessRuleEntity() {
                RuleName = ruleName
            };
        }

        // 保存
        public void Save() {
            // 優先順位が-1の場合は、最大の優先順位を取得して設定
            if (Priority == -1) {
                Priority = GetAllAutoProcessRules().Count() + 1;
            }
            using PythonAILibDBContext db = new();
            var item = db.AutoProcessRules.Find(Entity.Id);
            if (item == null) {
                db.AutoProcessRules.Add(Entity);
            } else {
                db.AutoProcessRules.Update(Entity);
            }
            db.SaveChanges();
        }
        // 削除
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.AutoProcessRules.Find(Entity.Id);
            if (item == null) {
                return;
            }
            db.AutoProcessRules.Remove(item);
        }

        // 取得
        public static IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            using PythonAILibDBContext db = new();
            // 全てのルールを取得
            var items = db.AutoProcessRules;
            foreach (var item in items) {
                yield return new AutoProcessRule(item);
            }

        }


        // RuleConditionTypesの条件に全てマッチした場合にTrueを返す。マッチしない場合とルールがない場合はFalseを返す。
        public bool IsMatch(ContentItemWrapper clipboardItem) {
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
        public void RunAction(ContentItemWrapper clipboardItem) {
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

            RuleAction.Execute(clipboardItem, DestinationFolder);
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
                if (RuleAction != null && RuleAction.IsCopyOrMoveAction()) {


                    if (DestinationFolder != null) {
                        result += $"{PythonAILibStringResources.Instance.Folder}:{DestinationFolder.ContentFolderPath}\n";
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
            if (rule.RuleAction.IsCopyOrMoveAction() == false) {
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
                if (r.TargetFolder != null && r.DestinationFolder != null) {
                    // keyが存在しない場合は新しいLinkedListを作成
                    if (!fromToDictionary.TryGetValue(r.TargetFolder.ContentFolderPath, out List<string>? value)) {
                        value = [];
                        fromToDictionary[r.TargetFolder.ContentFolderPath] = value;
                    }

                    value.Add(r.TargetFolder.ContentFolderPath);
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
            int index = autoProcessRules.FindIndex(r => r.Entity.Id == autoProcessRule.Entity.Id);
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
            int index = autoProcessRules.FindIndex(r => r.Entity.Id == autoProcessRule.Entity.Id);
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

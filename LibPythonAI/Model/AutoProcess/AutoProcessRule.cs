using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Resources;

namespace LibPythonAI.Model.AutoProcess {

    public class AutoProcessRule {

        public AutoProcessRule(AutoProcessRuleEntity autoProcessRuleEntity) {
            Entity = autoProcessRuleEntity;
        }

        /// <summary>
        /// 指定した名前のルールを作成する
        /// </summary>
        /// <param name="ruleName"></param>
        public AutoProcessRule(string ruleName) {
            Entity = new AutoProcessRuleEntity() {
                RuleName = ruleName
            };
        }

        public AutoProcessRuleEntity Entity { get; set; }

        public string Id { get => Entity.Id; }


        public string RuleName {
            get => Entity.RuleName;
            set => Entity.RuleName = value;
        }

        // このルールを有効にするかどうか
        public bool IsEnabled {
            get => Entity.IsEnabled;
            set => Entity.IsEnabled = value;
        }

        // 優先順位
        public int Priority {
            get => Entity.Priority;
            set => Entity.Priority = value;
        }
        private List<AutoProcessRuleCondition>? _condition;
        public List<AutoProcessRuleCondition> Conditions {
            get {
                if (_condition == null) {
                    _condition = Entity.Conditions.Select(c => new AutoProcessRuleCondition(c)).ToList();
                }
                return _condition;
            }
        }

        public AutoProcessItem? RuleAction {
            get {
                return  AutoProcessItem.GetItemById(Entity?.AutoProcessItemId);
            }
            set {
                Entity.AutoProcessItemId = value?.Id;
            }
        }


        public ContentFolderWrapper? DestinationFolder {
            get {
                return ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(Entity.DestinationFolderId);
            }
            set {
                Entity.DestinationFolderId = value?.Id;
            }
        }

        public ContentFolderWrapper? TargetFolder {
            get {
                return ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(Entity.TargetFolderId);
            }
            set {
                Entity.TargetFolderId = value?.Id;
            }
        }


        // 保存
        public void Save() {
            // 優先順位が-1の場合は、最大の優先順位を取得して設定
            if (Priority == -1) {
                Priority = GetAllAutoProcessRules().Count() + 1;
            }
            Entity.Conditions = Conditions.Select(c => c.Entity).ToList();
            AutoProcessRuleEntity.SaveItems([ Entity ]);

        }
        // 削除
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.AutoProcessRules.Find(Id);
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
                LogWrapper.Info(PythonAILibStringResources.Instance.RuleNameIsInvalid(RuleName));
                return;
            }

            if (!IsMatch(applicationItem)) {
                LogWrapper.Info(PythonAILibStringResources.Instance.NoMatch);
                return;
            }
            if (RuleAction == null) {
                LogWrapper.Warn(PythonAILibStringResources.Instance.NoActionSet);
                return;
            }
            // DestinationIdに一致するフォルダを取得

             await RuleAction.Execute(applicationItem, DestinationFolder);
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
        // GetItemsByRuleName
        public static List<AutoProcessRule> GetItemsByRuleName(string? ruleName) {
            List<AutoProcessRule> rules = [];
            if (ruleName == null) {
                return rules;
            }
            using PythonAILibDBContext db = new();
            var items = db.AutoProcessRules.Where(x => x.RuleName == ruleName);
            foreach (var item in items) {
                rules.Add(new AutoProcessRule(item));
            }
            return rules;
        }
        // GetItemsByTargetFolder
        public static List<AutoProcessRule> GetItemByTargetFolder(ContentFolderWrapper? targetFolder) {
            List<AutoProcessRule> rules = [];
            if (targetFolder == null) {
                return rules;
            }
            using PythonAILibDBContext db = new();
            var items = db.AutoProcessRules.Where(x => x.TargetFolderId == targetFolder.Id);
            foreach (var item in items) {
                rules.Add(new AutoProcessRule(item));
            }
            return rules;
        }


        // GetCopyToMoveToRules
        public static List<AutoProcessRule> GetCopyToMoveToRules() {
            var copyRules = GetItemsByRuleName(AutoProcessItem.TypeEnum.CopyToFolder.ToString());
            var moveRules = GetItemsByRuleName(AutoProcessItem.TypeEnum.MoveToFolder.ToString());

            List<AutoProcessRule> rules = [];
            rules.AddRange(copyRules);
            rules.AddRange(moveRules);
            return rules;
        }
    }
}

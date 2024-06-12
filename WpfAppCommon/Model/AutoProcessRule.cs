using WpfAppCommon.Utils;
using LiteDB;

namespace WpfAppCommon.Model {

    // 自動処理ルールの条件

    public class AutoProcessRuleCondition {
        // 条件の種類
        public ConditionTypeEnum Type { get; set; }

        // アイテムのタイプ種類のリスト
        public List<ClipboardContentTypes> ContentTypes { get; set; } = [];

        // 条件のキーワード
        public string Keyword { get; set; } = "";

        public AutoProcessRuleCondition(ConditionTypeEnum type, string keyword) {
            Type = type;
            Keyword = keyword;
        }
        public AutoProcessRuleCondition(List<ClipboardContentTypes> contentTypes, int minLineCount) {
            ContentTypes = contentTypes;
            MinLineCount = minLineCount;
            Type = ConditionTypeEnum.ContentTypeIs;
        }


        public enum ConditionTypeEnum {
            AllItems,
            DescriptionContains,
            ContentContains,
            SourceApplicationNameContains,
            SourceApplicationTitleContains,
            SourceApplicationPathContains,
            ContentTypeIs,

        }

        public ConditionTypeEnum ConditionType { get; set; } = ConditionTypeEnum.AllItems;
        public int MinLineCount { get; set; } = 0;

        //ClipboardItemのDescriptionが指定したキーワードを含むかどうか
        public  bool IsDescriptionContains(ClipboardItem clipboardItem, string keyword) {
            // DescriptionがNullの場合はFalseを返す
            if (clipboardItem.Description == null) {
                return false;
            }
            Tools.Logger.Debug("Description:" + clipboardItem.Description);
            Tools.Logger.Debug("Keyword:" + keyword);
            Tools.Logger.Debug("Contains:" + clipboardItem.Description.Contains(keyword));

            return clipboardItem.Description.Contains(keyword);

        }
        //ClipboardItemのContentが指定したキーワードを含むかどうか
        public  bool IsContentContains(ClipboardItem clipboardItem, string keyword) {
            // ContentがNullの場合はFalseを返す
            if (clipboardItem.Content == null) {
                return false;
            }
            return clipboardItem.Content.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationNameが指定したキーワードを含むかどうか
        public  bool IsSourceApplicationNameContains(ClipboardItem clipboardItem, string keyword) {
            // SourceApplicationNameがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationName == null) {
                return false;
            }
            return clipboardItem.SourceApplicationName.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationTitleが指定したキーワードを含むかどうか
        public  bool IsSourceApplicationTitleContains(ClipboardItem clipboardItem, string keyword) {
            // SourceApplicationTitleがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationTitle == null) {
                return false;
            }
            return clipboardItem.SourceApplicationTitle.Contains(keyword);
        }
        // ClipboardItemのSourceApplicationPathが指定したキーワードを含むかどうか
        public  bool IsSourceApplicationPathContains(ClipboardItem clipboardItem, string keyword) {
            // SourceApplicationPathがnullの場合は、falseを返す
            if (clipboardItem.SourceApplicationPath == null) {
                return false;
            }
            return clipboardItem.SourceApplicationPath != null && clipboardItem.SourceApplicationPath.Contains(keyword);
        }

        // ClipboardItemのContentの行数が指定した行数以上かどうか
        public  bool IsContentLineCountMoreThan(ClipboardItem clipboardItem) {
            // ContentがNullの場合はFalseを返す
            if (clipboardItem.Content == null) {
                return false;
            }
            return clipboardItem.Content.Split('\n').Length > MinLineCount;
        }

        // ConditionTypeに対応する関数を実行してBoolを返す
        // ★TODO SearchConditionと共通化する
        public bool CheckCondition(ClipboardItem clipboardItem) {
            return Type switch {
                ConditionTypeEnum.DescriptionContains => IsDescriptionContains(clipboardItem, Keyword),
                ConditionTypeEnum.ContentContains => IsContentContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationNameContains => IsSourceApplicationNameContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationTitleContains => IsSourceApplicationTitleContains(clipboardItem, Keyword),
                ConditionTypeEnum.SourceApplicationPathContains => IsSourceApplicationPathContains(clipboardItem, Keyword),
                ConditionTypeEnum.ContentTypeIs => CheckContentTypeIs(clipboardItem),
                _ => false,
            };
        }

        // ContentTypeIsの条件にマッチするかどうか
        public bool CheckContentTypeIs(ClipboardItem clipboardItem) {
            if (ContentTypes.Contains(clipboardItem.ContentType) == false) {
                return false;
            }
            if (clipboardItem.ContentType == ClipboardContentTypes.Text) {
                return IsContentLineCountMoreThan(clipboardItem);
            }
            return true;
        }

    }
    public class AutoProcessRule {
        public ObjectId Id { get; set; } = ObjectId.Empty;

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        public List<AutoProcessRuleCondition> Conditions { get; set; } = [];

        public SystemAutoProcessItem? RuleAction { get; set; }

        public ClipboardFolder? TargetFolder { get; set; }

        // 移動またはコピー先のフォルダ
        public ClipboardFolder? DestinationFolder { get; set; }

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
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertAutoProcessRule(this);
        }
        // 削除
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteAutoProcessRule(this);
            // 削除後はIdをNullにする
            Id = LiteDB.ObjectId.Empty;
        }
        // 取得
        public static IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetAllAutoProcessRules();
        }


        // RuleConditionTypesの条件に全てマッチした場合にTrueを返す。マッチしない場合とルールがない場合はFalseを返す。
        public bool IsMatch(ClipboardItem clipboardItem) {
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
        public ClipboardItem? RunAction(ClipboardItem clipboardItem) {
            // ルールが有効でない場合はそのまま返す
            if (!IsEnabled) {
                Tools.Info(RuleName + "は無効です");
                return clipboardItem;
            }

            if (!IsMatch(clipboardItem)) {
                Tools.Info("条件にマッチしませんでした");
                return clipboardItem;
            }
            if (RuleAction == null) {
                Tools.Warn("アクションが設定されていません");
                return clipboardItem;
            }
            return RuleAction.Execute(clipboardItem, DestinationFolder);
        }
        public string GetDescriptionString() {
            string result = "条件\n";
            foreach (var condition in Conditions) {
                // ConditionTypeごとに処理
                switch (condition.Type) {
                    case AutoProcessRuleCondition.ConditionTypeEnum.DescriptionContains:
                        result += "Descriptionが" + condition.Keyword + "を含む \n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.ContentContains:
                        result += "Contentが" + condition.Keyword + "を含む \n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationNameContains:
                        result += "SourceApplicationNameが" + condition.Keyword + "を含む \n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationTitleContains:
                        result += "SourceApplicationTitleが" + condition.Keyword + "を含む \n";
                        break;
                    case AutoProcessRuleCondition.ConditionTypeEnum.SourceApplicationPathContains:
                        result += "SourceApplicationPathが" + condition.Keyword + "を含む \n";
                        break;
                }
                // AutoProcessItemが設定されている場合
                if (RuleAction != null) {
                    result += "アクション:" + RuleAction.Description + "\n";
                } else {
                    result += "アクション:なし\n";
                }
                // Type が CopyToFolderまたはMoveToFolderの場合
                if (RuleAction != null && RuleAction.IsCopyOrMoveOrMergeAction()) {
                    // DestinationFolderが設定されている場合
                    if (DestinationFolder != null) {
                        result += "フォルダ:" + DestinationFolder.FolderPath + "\n";
                    } else {
                        result += "フォルダ:なし\n";
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
                if (r.TargetFolder != null && r.DestinationFolder != null) {
                    // keyが存在しない場合は新しいLinkedListを作成
                    if (!fromToDictionary.TryGetValue(r.TargetFolder.Id.ToString(), out List<string>? value)) {
                        value = [];
                        fromToDictionary[r.TargetFolder.Id.ToString()] = value;
                    }

                    value.Add(r.DestinationFolder.Id.ToString());
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
                Tools.Warn($"無限ループを検出しました\n{Tools.ListToString(pathList)}");
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

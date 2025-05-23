using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleCondition {


        public AutoProcessRuleConditionEntity Entity { get; set; }
        public AutoProcessRuleCondition(AutoProcessRuleConditionEntity entity) {
            Entity = entity;
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
        // 条件のキーワード
        public string Keyword {
            get {
                return Entity.Keyword;
            }
            set {
                Entity.Keyword = value;
            }
        }

        // 条件の種類
        public ConditionTypeEnum Type {
            get {
                return Entity.ConditionType;
            }
            set {
                Entity.ConditionType = value;
            }
        }

        // アイテムのタイプ種類のリスト
        public List<ContentItemTypes.ContentItemTypeEnum> ContentTypes {
            get {
                return Entity.ContentTypes;
            }
            set {
                Entity.ContentTypes = value;
            }
        }


        public ConditionTypeEnum ConditionType {
            get {
                return Entity.ConditionType;
            }
            set {
                Entity.ConditionType = value;
            }
        }

        public int MinLineCount {
            get {
                return Entity.MinLineCount;
            }
            set {
                Entity.MinLineCount = value;
            }
        }

        public int MaxLineCount {
            get {
                return Entity.MaxLineCount;
            }
            set {
                Entity.MaxLineCount = value;
            }
        }




        //ApplicationItemのDescriptionが指定したキーワードを含むかどうか
        public bool IsDescriptionContains(ContentItemWrapper applicationItem, string keyword) {
            // DescriptionがNullの場合はFalseを返す
            if (applicationItem.Description == null) {
                return false;
            }
            LogWrapper.Info("Description:" + applicationItem.Description);
            LogWrapper.Info("Keyword:" + keyword);
            LogWrapper.Info("Contains:" + applicationItem.Description.Contains(keyword));

            return applicationItem.Description.Contains(keyword);

        }
        //ApplicationItemのContentが指定したキーワードを含むかどうか
        public bool IsContentContains(ContentItemWrapper applicationItem, string keyword) {
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationNameが指定したキーワードを含むかどうか
        public bool IsSourceApplicationNameContains(ContentItemWrapper applicationItem, string keyword) {
            // SourceApplicationNameがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationName == null) {
                return false;
            }
            return applicationItem.SourceApplicationName.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationTitleが指定したキーワードを含むかどうか
        public bool IsSourceApplicationTitleContains(ContentItemWrapper applicationItem, string keyword) {
            // SourceApplicationTitleがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationTitle == null) {
                return false;
            }
            return applicationItem.SourceApplicationTitle.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationPathが指定したキーワードを含むかどうか
        public bool IsSourceApplicationPathContains(ContentItemWrapper applicationItem, string keyword) {
            // SourceApplicationPathがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationPath == null) {
                return false;
            }
            return applicationItem.SourceApplicationPath != null && applicationItem.SourceApplicationPath.Contains(keyword);
        }

        // ApplicationItemのContentの行数が指定した行数以上かどうか
        public bool IsContentLineCountOver(ContentItemWrapper applicationItem) {
            // MinLineCountが-1の場合はTrueを返す
            if (MinLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Split('\n').Length >= MinLineCount;
        }
        // ApplicationItemのContentの行数が指定した行数以下かどうか
        public bool IsContentLineCountUnder(ContentItemWrapper applicationItem) {
            // MaxLineCountが-1の場合はTrueを返す
            if (MaxLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Split('\n').Length <= MaxLineCount;
        }

        // ConditionTypeに対応する関数を実行してBoolを返す
        // ★TODO SearchConditionと共通化する
        public bool CheckCondition(ContentItemWrapper applicationItem) {
            return Type switch {
                ConditionTypeEnum.DescriptionContains => IsDescriptionContains(applicationItem, Keyword),
                ConditionTypeEnum.ContentContains => IsContentContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationNameContains => IsSourceApplicationNameContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationTitleContains => IsSourceApplicationTitleContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationPathContains => IsSourceApplicationPathContains(applicationItem, Keyword),
                ConditionTypeEnum.ContentTypeIs => CheckContentTypeIs(applicationItem),
                _ => false,
            };
        }

        // ContentTypeIsの条件にマッチするかどうか
        public bool CheckContentTypeIs(ContentItemWrapper applicationItem) {
            if (ContentTypes.Contains(applicationItem.ContentType) == false) {
                return false;
            }
            if (applicationItem.ContentType == LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Text) {
                return IsContentLineCountOver(applicationItem) && IsContentLineCountUnder(applicationItem);
            }
            return true;
        }

    }

}

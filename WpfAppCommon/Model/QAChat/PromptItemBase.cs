using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model.QAChat {

    public abstract class PromptItemBase {

        public enum PromptTemplateTypeEnum {
            // ユーザー定義
            UserDefined,
            // システム定義
            SystemDefined,
            // 変更を加えたシステム定義
            ModifiedSystemDefined
        }

        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;

        // Save
        public abstract void Save();

    }
}

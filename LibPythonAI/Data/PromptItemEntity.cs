using System.ComponentModel.DataAnnotations;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;

namespace LibPythonAI.Data {
    public class PromptItemEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // プロンプトテンプレートの種類
        public PromptTemplateTypeEnum PromptTemplateType { get; set; } = PromptTemplateTypeEnum.UserDefined;

        // プロンプト結果の種類
        public PromptResultTypeEnum PromptResultType { get; set; } = PromptResultTypeEnum.TextContent;

        // チャットタイプ
        public OpenAIExecutionModeEnum ChatMode { get; set; } = OpenAIExecutionModeEnum.Normal;

        // 分割モード
        public SplitOnTokenLimitExceedModeEnum SplitMode { get; set; } = SplitOnTokenLimitExceedModeEnum.None;

        // ベクトルDBを使用する
        public bool UseVectorDB { get; set; } = false;

        // プロンプトの出力タイプ
        public PromptOutputTypeEnum PromptOutputType { get; set; } = PromptOutputTypeEnum.NewContent;

        // PromptInputName
        public string PromptInputName { get; set; } = string.Empty;

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            PromptItemEntity other = (PromptItemEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}

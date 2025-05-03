namespace PythonAILib.Model.Prompt {
    public enum PromptOutputTypeEnum {
        // 新規作成
        NewContent = 0,
        // 本文を上書き
        OverwriteContent = 1,
        // タイトルを上書き
        OverwriteTitle = 2,
        // タグを追加
        AppendTags = 3,
    }
}

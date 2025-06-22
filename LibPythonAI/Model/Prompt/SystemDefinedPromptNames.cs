namespace LibPythonAI.Model.Prompt {
    public enum SystemDefinedPromptNames {
        // タイトル生成
        TitleGeneration = 0,
        // 背景情報生成
        BackgroundInformationGeneration = 1,
        // サマリー生成
        SummaryGeneration = 2,
        // 課題リスト生成
        TasksGeneration = 3,
        // 文章の信頼度判定
        DocumentReliabilityCheck = 4,
        // タグ生成
        TagGeneration = 5,
        // 既存のタグを選択
        SelectExistingTags = 6,
        // クリップボードの内容からユーザーの意図を推測
        PredictUserIntentFromClipboard = 7,
        // 画像からユーザーの意図を推測
        PredictUserIntentFromImage = 8,

    }
}

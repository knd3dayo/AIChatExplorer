namespace LibUIMergeChat.Resources {
    public class MergeChatStringResourcesJa : MergeChatStringResources {

        // MergeTarget
        public override string MergeTarget { get; set; } = "マージ対象";

        // MergeTargetItemSelection
        public override string MergeTargetItemSelection { get; set; } = "マージ対象アイテム選択";

        // MergeTargetDataSelection
        public override string MergeTargetDataSelection { get; set; } = "マージ対象データ";

        // 事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public override string PreProcessingPromptHint { get; set; } = """
            事前処理用プロンプト マージ前に各アイテムに対して事前処理を行うためのプロンプト。
            ダブルクリックするとプロンプトテンプレート選択画面が開きます。
            """;

        // 事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public override string PostProcessingPromptHint { get; set; } = """
            事後処理用プロンプト マージ後のアイテムに対して事後処理を行うためのプロンプト
            ダブルクリックするとプロンプトテンプレート選択画面が開きます。
            """;

        // OutputFolderSelection
        public override string OutputFolderSelection { get; set; } = "出力フォルダ選択";

        // リストから除外
        public override string ExcludeFromList { get; } = "リストから除外";

    }
}

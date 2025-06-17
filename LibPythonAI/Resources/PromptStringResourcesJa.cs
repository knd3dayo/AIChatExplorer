namespace LibPythonAI.Resources {
    public class PromptStringResourceJa : PromptStringResource {

        // SummarizePromptText 
        public override string SummarizePromptText { get; } = "単純に結合しただけなので、文章のつながりがよくない箇所があるかもしれません。 文章のつながりがよくなるように整形してください。 出力言語は日本語にしてください。\n";


        #region システム定義プロンプト
        // "この画像のテキストを抽出してください。\n"
        public override string ExtractTextRequest { get; } = "この画像のテキストを抽出してください。\n";


        // Json形式で文字列のリストを生成するプロンプト
        public override string JsonStringListGenerationPrompt { get; } = "出力は文字列のリストとして、JSON形式で{result:[リストの項目]}でお願いします。\n";

        // 文書の信頼度判定結果の文章から信頼度を取得するプロンプト
        public override string DocumentReliabilityDictionaryPrompt { get; } = "以下の文章は、ある文章の信頼度を判定して結果です。最終的な信頼度の数値(0-100)を出力してください。" +
            "出力は次のJSON形式でお願いします。 {\"reliability\": 信頼度の数値, \"reason\": \"信頼度の判定理由\"}";

        // 文章の信頼度の定義
        public override string DocumentReliabilityDefinition { get; } = "文章の信頼度とは、その文章を別の文章の根拠としてよいかのレベルを示す指標です。\n" +
            "### 文章作成元・公開範囲による判定\n" +
            "* 権威ある組織や機関、人が書いたもので、一般に公開された情報の場合は信頼レベル高(信頼度：90～100%)\n" +
            "* Wikipediaなど、信頼ある情報の掲載が求められるサイトの情報は信頼レベル中～高(信頼度：70～90%)\n" +
            "* 社内の組織や人が書いたもので、公開範囲が組織に限定された情報の場合は信頼レベル低～高(信頼度：40～90%) " +
            "* 公開範囲の想定が不明または個人間のやり取り、と思われる文章は信頼レベル低(信頼度：10%～30%)\n" +
            "## 内容による判定\n" +
            "* 各信頼レベルの文章はその内容により、信頼度が上下する。\n" +
            "  *  既存の論理、数学的な法則、自然科学的法則により正しいと判定可能な情報は信頼度をレベル内での上限値にする。\n" +
            "  *  一般的な社会学的法則、慣習などによりある程度正しいと判定可能は情報は信頼度をレベル内での中間値にする。\n" +
            "  * 正しさが判断できない、検証が必要な情報は信頼度をレベル内での下限値にする。\n";


        // 既存のタグ一覧は以下の通りです
        public override string TagListPrompt(string tags) {
            return $"既存のタグ一覧は以下のとおりです。\n{tags}\n";
        }

        #endregion
    }
}

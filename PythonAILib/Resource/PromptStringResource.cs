namespace PythonAILib.Resource {
    public class PromptStringResource {

        // Instance
        public static PromptStringResource Instance { get; set; } = new();


        // 定義が不明な文章については、以下の説明を参考にしてください
        public virtual string UnknownContentDescription { get; } = "定義が不明な文章については、以下の説明を参考にしてください";


        // サマリー生成
        public virtual string SummaryGeneration { get; } = "サマリー";

        // "以下の文章から100～200文字程度のサマリーを生成してください。\n"
        public virtual string SummaryGenerationPrompt { get; } = "以下の文章から100～200文字程度のサマリーを生成してください。\n";


        // BackgroundInformationGeneration
        public virtual string BackgroundInformationGeneration { get; } = "背景情報";

        // "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n"
        public virtual string BackgroundInformationGenerationPrompt { get; } = "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n";

        // 質問生成
        public virtual string GenerateQuestionRequest { get; } = "文章を分析して質問を挙げてください。\r\n例：\r\n# 定義(類と種差)に関する質問\r\n 文章. ぽんちょろりん汁はおいしいです。\r\n 質問. ぽんちょろりん汁は汁物料理のカテゴリに属するものですか？またはカテゴリ内の他のものとの異なる点はなんですか？\r\n# 目的・理由に関する質問\r\n 文章. 〇〇という仕事は今日中に終えなければならない\r\n 質問.  〇〇という仕事を今日中に終えなければいけない理由は何ですか？ また、〇〇という仕事を行う目的はなんですか？\r\n# 原因・経緯・歴史に関する質問\r\n 文章. 徳川家康は将軍である\r\n  質問. 徳川家康が将軍になった原因はなんですか？\r\n# 構成要素、機能に関する質問\r\n 文章. ぽんちょろりん汁は健康に良い\r\n 質問. ぽんちょろりん汁はどのような材料で作成されていますか？また、どの様な効果があるのですか？";

        // 回答依頼
        public virtual string AnswerRequest { get; } = "以下の質問に回答してください。\n";

        // タイトル生成
        public virtual string TitleGeneration { get; } = "タイトル生成";
        // "以下の文章からタイトルを生成してください。\n"
        public virtual string TitleGenerationPrompt { get; } = "以下の文章からタイトルを生成してください。\n";

        // "この画像のテキストを抽出してください。\n"
        public virtual string ExtractTextRequest { get; } = "この画像のテキストを抽出してください。\n";

        // 上記の文章の不明点については、以下の関連情報を参考にしてください
        public virtual string RelatedInformation { get; } = "------\n 以下は参考情報です。不正確な情報が含まれる可能性がありますが、本文の背景や文脈を理解するための参考にしてください\n";

        // 以下の文章を解析して、定義が不明な言葉を含む文を洗い出してください。" +
        // "定義が不明な言葉とはその言葉の類と種差、原因、目的、機能、構成要素が不明確な言葉です。" +
        // "出力は以下のJSON形式のリストで返してください。解析対象の文章がない場合や解析不能な場合は空のリストを返してください\n" +
        // "{'result':[{'sentence':'定義が不明な言葉を含む文','reason':'定義が不明な言葉を含むと判断した理由'}]}"

        // TODOリスト生成
        public virtual string TasksGeneration { get; } = "TODOリスト";

        // "以下の文章から課題リストを生成してください。\n"
        public virtual string TasksGenerationPrompt { get; } = "以下の文章からTODOとアクションプランのリストを生成してください。" +
            "なお参考情報がある場合には参考情報から得た背景や文脈を踏まえてTODOとアクションプランを具体的なものにしてください\n" +
            "TODOには対応すべき優先順位をつけてください。本文とは関連度が低いTODOは除外してください。\n" +
            "出力はJSON形式で{result:['todo': 'TODOの内容','plan': 'プランの内容']}でお願いします。\n";

        // Json形式で文字列のリストを生成するプロンプト
        public virtual string JsonStringListGenerationPrompt { get; } = "出力は文字列のリストとして、JSON形式で{result:[リストの項目]}でお願いします。\n";

        // 文章の信頼度
        public virtual string DocumentReliability { get; } = "文章の信頼度";

        // 文章の信頼度を判定するプロンプト
        public virtual string DocumentReliabilityCheckPrompt { get; } = "# 文章の信頼度判定\r\nその文章を別の文章の根拠としてよいかのレベル。" +
            "## 概要\r\nその文章を別の文章の根拠としてよいかのレベル。" +
            "このレベルにより、" +
            "##  どうやって判断するか？" +
            "まずは大まかな機銃として次の指標を置く。" +
            "### 文章作成元・公開範囲による判定" +
            "* 権威ある組織や機関、人が書いたもので、一般に公開された情報の場合は信頼レベル高(信頼度：90～100%)  " +
            "  ただし、今後、信頼度高にあたるサイトの分類が必要。" +
            "* Wikipediaなど、信頼ある情報の掲載が求められるサイトの情報は信頼レベル中～高(信頼度：70～90%)  " +
            "  ただし、今後、信頼度中～高にあたるサイトの分類が必要。  " +
            "* StackOverflowなど、誤りが含まれる可能性が高いものの多くの人によりチェックが可能なサイトの情報は信頼レベル低～中(信頼度：40～60%)  " +
            "* 社内の組織や人が書いたもので、公開範囲が組織に限定された情報の場合は信頼レベル低～高(信頼度：40～90%)  " +
            "  * メールやTeamsのチャットなど他者への依頼や確認を行う文章、通達や研究発表資料など組織内の多くの人が見ると思われる文章。  " +
            "  * その情報は作成途中の段階のものやレビュー未済の情報も含まれている可能性があり" +
            "* 公開範囲の想定が不明または個人間のやり取り、と思われる文章は信頼レベル低(信頼度：10%～30%)" +
            "  * 個人的なアイデアやメモ、文脈が不明な文章など。" +
            "## 内容による判定" +
            "* 各信頼レベルの文章はその内容により、信頼度が上下する。 " +
            "  *  既存の論理、数学的な法則、自然科学的法則により正しいと判定可能な情報は信頼度をレベル内での上限値にする。" +
            "  *  一般的な社会学的法則、慣習などによりある程度正しいと判定可能は情報は信頼度をレベル内での中間値にする。" +
            "  * 正しさが判断できない、検証が必要な情報は信頼度をレベル内での下限値にする。" +
            "" +
            "以上を踏まえて、次の文章の信頼レベルを判定して、信頼度の数値(0-100)と信頼度の判定理由を出力してください。";

        // 文書の信頼度判定結果の文章から信頼度を取得するプロンプト
        public virtual string DocumentReliabilityDictonaryPrompt { get; } = "以下の文章は、ある文章の信頼度を判定して結果です。最終的な信頼度の数値(0-100)を出力してください。" +
            "出力は次のJSON形式でお願いします。 {\"reliability\": 信頼度の数値, \"reason\": \"信頼度の判定理由\"}";

    }
}

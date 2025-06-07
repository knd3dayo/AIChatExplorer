namespace PythonAILib.Resources {
    public class PromptStringResource {

        // Instance
        public static PromptStringResource Instance { get; set; } = new();

        #region RequestContext


        // 上記の文章の不明点については、以下の関連情報を参考にしてください
        public virtual string RelatedInformationByVectorSearch { get; } = "------ 以下は本文に関連する情報をベクトルDBから検索した結果です。---\n";
        // SummarizePromptText 
        public virtual string SummarizePromptText { get; } = "単純に結合しただけなので、文章のつながりがよくない箇所があるかもしれません。 文章のつながりがよくなるように整形してください。 出力言語は日本語にしてください。\n";

        #endregion

        #region システム定義プロンプト

        // ベクトルDBシステムメッセージ。 
        public virtual string VectorDBSystemMessage(string description) {
            string message = $"以下の情報が格納されたベクトルDBを検索します。ユーザーから指定された文字列にマッチするデータを返します\n\n {description}";
            return message;
        }

        // 定義が不明な文章については、以下の説明を参考にしてください
        public virtual string UnknownContentDescription { get; } = "定義が不明な文章については、以下の説明を参考にしてください";

        // 上記の文章の不明点については、以下の関連情報を参考にしてください
        public virtual string RelatedInformation {
            get {
                return "------ 以下は参考情報です。---\n情報には信頼度が設定されている場合があります。参考情報に矛盾した内容がある場合は、信頼度が高い情報を優先してください。信頼度の定義は次の通りです。\n" +
                    DocumentReliabilityDefinition + "\n " +
                    "ユーザーに情報の信頼度を伝えるために、回答とともに使用した参考情報を信頼度毎に教えてください\n ------";
            }
        }

        
        // 質問生成
        public virtual string GenerateQuestionRequest { get; } = "文章を分析して質問を挙げてください。\r\n例：\r\n# 定義(類と種差)に関する質問\r\n 文章. ぽんちょろりん汁はおいしいです。\r\n 質問. ぽんちょろりん汁は汁物料理のカテゴリに属するものですか？またはカテゴリ内の他のものとの異なる点はなんですか？\r\n# 目的・理由に関する質問\r\n 文章. 〇〇という仕事は今日中に終えなければならない\r\n 質問.  〇〇という仕事を今日中に終えなければいけない理由は何ですか？ また、〇〇という仕事を行う目的はなんですか？\r\n# 原因・経緯・歴史に関する質問\r\n 文章. 徳川家康は将軍である\r\n  質問. 徳川家康が将軍になった原因はなんですか？\r\n# 構成要素、機能に関する質問\r\n 文章. ぽんちょろりん汁は健康に良い\r\n 質問. ぽんちょろりん汁はどのような材料で作成されていますか？また、どの様な効果があるのですか？";

        // 回答依頼
        public virtual string AnswerRequest { get; } = "以下の質問に回答してください。\n";

        // "この画像のテキストを抽出してください。\n"
        public virtual string ExtractTextRequest { get; } = "この画像のテキストを抽出してください。\n";


        // Json形式で文字列のリストを生成するプロンプト
        public virtual string JsonStringListGenerationPrompt { get; } = "出力は文字列のリストとして、JSON形式で{result:[リストの項目]}でお願いします。\n";

        // 文書の信頼度判定結果の文章から信頼度を取得するプロンプト
        public virtual string DocumentReliabilityDictionaryPrompt { get; } = "以下の文章は、ある文章の信頼度を判定して結果です。最終的な信頼度の数値(0-100)を出力してください。" +
            "出力は次のJSON形式でお願いします。 {\"reliability\": 信頼度の数値, \"reason\": \"信頼度の判定理由\"}";

        // 文章の信頼度の定義
        public virtual string DocumentReliabilityDefinition { get; } = "文章の信頼度とは、その文章を別の文章の根拠としてよいかのレベルを示す指標です。\n" +
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
        public virtual string TagListPrompt(string tags) {
            return $"既存のタグ一覧は以下のとおりです。\n{tags}\n";
        }

        #endregion
    }
}

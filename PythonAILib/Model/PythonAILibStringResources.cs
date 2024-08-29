namespace PythonAILib.Model {
    public class PythonAILibStringResources {

        private static PythonAILibStringResources? _Instance;
        public static PythonAILibStringResources Instance {
            get {
                if (_Instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _Instance = new PythonAILibStringResources();
                            break;
                        default:
                            _Instance = new PythonAILibStringResourcesEn();
                            break;
                    }
                }
                return _Instance;
            }
        }

        private static bool _LangChanged = false;
        private static string _Lang = "ja-JP";
        public static string Lang {
            get { return _Lang; }
            set {
                if (_Lang != value) {
                    _LangChanged = true;
                }
                _Lang = value; 
            }
        }

        // --- namespace WpfAppCommon.PythonIF ---


        // クリップボードの内容が変更されました
        public virtual string ClipboardChangedMessage { get; } = "クリップボードの内容が変更されました";
        // クリップボードアイテムを処理
        public virtual string ProcessClipboardItem { get; } = "クリップボードアイテムを処理";
        // 自動処理を実行中
        public virtual string AutoProcessing { get; } = "自動処理を実行中";
        // クリップボードアイテムの追加処理が失敗しました。
        public virtual string AddItemFailed { get; } = "クリップボードアイテムの追加処理が失敗しました。";

        // 自動タイトル設定処理を実行します
        public virtual string AutoSetTitle { get; } = "自動タイトル設定処理を実行します";
        // タイトル設定処理が失敗しました
        public virtual string SetTitleFailed { get; } = "タイトル設定処理が失敗しました";
        // 自動タグ設定処理を実行します
        public virtual string AutoSetTag { get; } = "自動タグ設定処理を実行します";
        // タグ設定処理が失敗しました
        public virtual string SetTagFailed { get; } = "タグ設定処理が失敗しました";
        // 自動マージ処理を実行します
        public virtual string AutoMerge { get; } = "自動マージ処理を実行します";
        // マージ処理が失敗しました
        public virtual string MergeFailed { get; } = "マージ処理が失敗しました";
        // OCR処理を実行します
        public virtual string OCR { get; } = "OCR処理を実行します";
        // OCR処理が失敗しました
        public virtual string OCRFailed { get; } = "OCR処理が失敗しました";

        // 自動ファイル抽出処理を実行します
        public virtual string ExecuteAutoFileExtract { get; } = "自動ファイル抽出処理を実行します";
        // 自動ファイル抽出処理が失敗しました
        public virtual string AutoFileExtractFailed { get; } = "自動ファイル抽出処理が失敗しました";

        // --- EmptyPythonFunctions.cs ---
        // Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。
        public virtual string PythonNotEnabledMessage { get; } = "Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。";

        // --- PythonExecutor.cs ---
        // カスタムPythonスクリプトの、templateファイル
        public virtual string TemplateScript { get; } = "python/script_template.py";

        // OpenAI用のPythonスクリプト
        public virtual string WpfAppCommonOpenAIScript { get; } = "python/ai_app.py";

        // その他用のPythonスクリプト
        public virtual string WpfAppCommonMiscScript { get; } = "python/dev/misc_app.py";

        // テンプレートファイルが見つかりません
        public virtual string TemplateScriptNotFound { get; } = "テンプレートファイルが見つかりません";

        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public virtual string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";

        // Python venv環境が見つかりません。Python venvのパスを確認してください:
        public virtual string PythonVenvNotFound { get; } = "Python venv環境が見つかりません。Python venvのパスを確認してください:";
        //  "Pythonの初期化に失敗しました。"
        public virtual string PythonInitFailed { get; } = "Pythonの初期化に失敗しました。";

        // "Pythonスクリプトファイルに、{function_name}関数が見つかりません"
        public virtual string FunctionNotFound(string function_name) {
            return $"Pythonスクリプトファイルに、{function_name}関数が見つかりません";
        }
        // "Pythonスクリプトの実行中にエラーが発生しました
        public virtual string PythonExecuteError { get; } = "Pythonスクリプトの実行中にエラーが発生しました";

        // "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。
        public virtual string ModuleNotFound { get; } = "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。";

        // $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        public virtual string PythonExecuteErrorDetail(Exception e) {
            return $"メッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
        }
        // "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください"
        public virtual string SpacyModelNameNotSet { get; } = "Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください";

        // "マスキング結果がありません"
        public virtual string MaskingResultNotFound { get; } = "マスキング結果がありません";

        // "マスキングした文字列取得に失敗しました"
        public virtual string MaskingResultFailed { get; } = "マスキングした文字列取得に失敗しました";

        // "マスキング解除結果がありません"
        public virtual string UnmaskingResultNotFound { get; } = "マスキング解除結果がありません";
        // "マスキング解除した文字列取得に失敗しました"
        public virtual string UnmaskingResultFailed { get; } = "マスキング解除した文字列取得に失敗しました";

        // "画像のバイト列に変換できません"
        public virtual string ImageByteFailed { get; } = "画像のバイト列に変換できません";

        // "VectorDBItemsが空です"
        public virtual string VectorDBItemsEmpty { get; } = "VectorDBItemsが空です";

        // "OpenAIの応答がありません"
        public virtual string OpenAIResponseEmpty { get; } = "OpenAIの応答がありません";

        // ファイルが存在しません
        public virtual string FileNotFound { get; } = "ファイルが存在しません";

        // --- ChatItem.cs ---

        // <参照元ドキュメントルート>
        public virtual string SourceDocumentRoot { get; } = "<参照元ドキュメントルート>";

        // --- ChatRequest.cs ---
        // \n---------以下は本文です------\n
        public virtual string ContentHeader { get; } = "\n---------以下は本文です------\n";

        // \n---------以下は関連情報です------\n
        public virtual string SourcesHeader { get; } = "\n---------以下は関連情報です------\n";

        // 画像のフォーマットが不明です。
        public virtual string UnknownImageFormat { get; } = "画像のフォーマットが不明です。";

        // 上記の文章の不明点については、以下の関連情報を参考にしてください
        public virtual string UnknownContent { get; } = "上記の文章の不明点については、以下の関連情報を参考にしてください";

        // 以下の文章を解析して、定義が不明な言葉を含む文を洗い出してください。" +
        // "定義が不明な言葉とはその言葉の類と種差、原因、目的、機能、構成要素が不明確な言葉です。" +
        // "出力は以下のJSON形式のリストで返してください。解析対象の文章がない場合や解析不能な場合は空のリストを返してください\n" +
        // "{'result':[{'sentence':'定義が不明な言葉を含む文','reason':'定義が不明な言葉を含むと判断した理由'}]}"

        public virtual string AnalyzeAndDictionarizeRequest { get; } = "以下の文章を解析して、定義が不明な言葉を含む文を洗い出してください。" +
            "定義が不明な言葉とはその言葉の類と種差、原因、目的、機能、構成要素が不明確な言葉です。" +
            "出力は以下のJSON形式のリストで返してください。解析対象の文章がない場合や解析不能な場合は空のリストを返してください\n" +
            "{'result':[{'sentence':'定義が不明な言葉を含む文','reason':'定義が不明な言葉を含むと判断した理由'}]}";

        // "ChatResultがnullです。"
        public virtual string ChatResultNull { get; } = "ChatResultがnullです。";

        // ChatResultのResponseが不正です。
        public virtual string ChatResultResponseInvalid { get; } = "ChatResultのResponseが不正です。";

        // ChatResultのResponseにResultが含まれていません。
        public virtual string ChatResultResponseResultNotFound { get; } = "ChatResultのResponseにResultが含まれていません。";

        // 定義が不明な文章については、以下の説明を参考にしてください
        public virtual string UnknownContentDescription { get; } = "定義が不明な文章については、以下の説明を参考にしてください";

        // "以下の文章から100～200文字程度のサマリーを生成してください。\n"
        public virtual string SummarizeRequest { get; } = "以下の文章から100～200文字程度のサマリーを生成してください。\n";

        // "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n"
        public virtual string BackgroundInfoRequest { get; } = "以下の文章の背景情報(経緯、目的、原因、構成要素、誰が？いつ？どこで？など)を生成してください。\n";

        // 日本語文章解析
        public virtual string AnalyzeJapaneseSentenceRequest { get; } = "* 命題とは(すべて|ある)主語(または主題)は〇〇である（またはではない)を表す文のことです。\r\n" +
            "* 一般的に日本語は次の構造をとります。\r\n " +
            "日本語構造=[主題]は[〇〇+格助詞] + 述語 + [時制、モダリティ、テンス]\r\n" +
            "* 日本語は、話し手と受け手の間ですでに共通認識がある部分については省略することがあり、述語さえあれば会話することが出来ます。\r\n" +
            "* 「モダリティ」とは、その文の内容に対する話し手の判断・聞き手に対する伝え方といった文の述べ方を担うもののことです。\r\n" +
            "* モダリティは、大きく\r\n" +
            "   -  文の伝達的な表し分けを表すモダリティ：表現類型のモダリティ\r\n" +
            "   -  事態に対する捉え方を表すモダリティ：評価のモダリティ、認識のモダリティ\r\n" +
            "   -  文と先行文脈との関連付けを表すモダリティ：説明のモダリティ\r\n" +
            "   -  聞き手に対する伝え方を表すモダリティ：丁寧さのモダリティ、伝達態度のモダリティ\r\n" +
            "の4つのタイプに分類することができます。\r\n" +
            "* 「表現類型のモダリティ」は、【叙述】【意志】【命令】【疑問】といった文の基本的な性質を表しています。\r\n" +
            "  - 【叙述】教科書を読む。\r\n  - 【意志】教科書を読もう。\r\n  - 【命令】教科書を読め。\r\n  - 【疑問】教科書を読みますか？\r\n" +
            "* 「評価のモダリティ」は、必要・不必要など、その事態に対する話し手の評価的な捉え方を表しています。\r\n" +
            "  - 図書館では、静かにしなくてはならない。\r\n" +
            "* 「認識のモダリティ」は、命題の内容である事態を話し手がどのような認識的な態度で捉えたかを表しています。\r\n" +
            "  - 明日は晴れるだろう。\r\n" +
            "* 「説明のモダリティ」は、そのモダリティが含まれる文と先行している文とが関連付いていることを表しています。\r\n" +
            "  - 雪が降っているのか。道理で寒いわけだ。\r\n" +
            "* 「丁寧さのモダリティ」とは、聞き手に対してその文を【普通体】【丁寧体】のどちらで伝えるかといったスタイルの選択に関わるもののことです。\r\n" +
            "  - 【普通体】今日は、この本を読んだ。\r\n  - 【丁寧体】今日は、この本を読みました。\r\n" +
            "* 「伝達態度のモダリティ」とは、聞き手に伝えるにあたっての微調整を行ったり、話し手の認識状態を表したりするもののことです。\r\n" +
            "  - ここを見てね\r\n  - きれいな景色だなあ\r\n\r\n" +
            "次の文章について以下の処理を行ってください。\r\n" +
            "- 省略部分を補完して上記の「日本語構造」の形式の命題のリストにしてください。  また、各命題がどのようなモダリティであるかも説明してください。\r\n   " +
            "想定される結果が複数の場合はそれらのうち可能性が高いものを10個まで挙げてください。";

        // "以下の文章からタイトルを生成してください。\n"
        public virtual string TitleRequest { get; } = "以下の文章からタイトルを生成してください。\n";

        // "この画像のテキストを抽出してください。\n"
        public virtual string ExtractTextRequest { get; } = "この画像のテキストを抽出してください。\n";

        // --- VectorDBItem.cs ---
        // "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        public virtual string VectorDBDescription { get; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";


        // --- PythonNetFunctions.cs ---

        // Embedding実行
        public virtual string EmbeddingExecute { get; } = "Embedding実行";

        // プロパティ情報
        public virtual string PropertyInfo { get; } = "プロパティ情報";

        // テキスト
        public virtual string Text { get; } = "テキスト";

        // レスポンス
        public virtual string Response { get; } = "レスポンス";

        // OpenAI実行
        public virtual string OpenAIExecute { get; } = "OpenAI実行";

        // チャット履歴
        public virtual string ChatHistory { get; } = "チャット履歴";

        // UpdateVectorDBIndex実行
        public virtual string UpdateVectorDBIndexExecute { get; } = "UpdateVectorDBIndex実行";

        // モードが不正です
        public virtual string InvalidMode { get; } = "モードが不正です";

        // UpdateVectorDBIndex実行
        public virtual string UpdateVectorDBIndex { get; } = "UpdateVectorDBIndex実行";

        // LangChain実行
        public virtual string LangChainExecute { get; } = "LangChain実行";
        // プロンプト
        public virtual string Prompt { get; } = "プロンプト";

        // VectorSearch実行
        public virtual string VectorSearchExecute { get; } = "VectorSearch実行";

        // ベクトル検索リクエスト
        public virtual string VectorSearchRequest { get; } = "ベクトル検索リクエスト";

        // Excelへのエクスポートを実行します
        public virtual string ExportToExcelExecute { get; } = "Excelへのエクスポートを実行します";
        // Excelへのエクスポートが失敗しました
        public virtual string ExportToExcelFailed { get; } = "Excelへのエクスポートが失敗しました";
        // Excelへのエクスポートが成功しました
        public virtual string ExportToExcelSuccess { get; } = "Excelへのエクスポートが成功しました";

        // ファイルパス
        public virtual string FilePath { get; } = "ファイルパス";
        // データ
        public virtual string Data { get; } = "データ";

    }
}

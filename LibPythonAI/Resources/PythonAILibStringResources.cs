namespace PythonAILib.Resources {
    public class PythonAILibStringResources {

        private static PythonAILibStringResources? _Instance;
        public static PythonAILibStringResources Instance {
            get {
                if (_Instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _Instance = new PythonAILibStringResources();
                            // PromptStringResourceもここで設定
                            PromptStringResource.Instance = new PromptStringResource();
                            break;
                        default:
                            _Instance = new PythonAILibStringResourcesEn();
                            // PromptStringResourceもここで設定
                            PromptStringResource.Instance = new PromptStringResourceEn();
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


        // PythonAILibManagerIsNotInitialized
        public virtual string PythonAILibManagerIsNotInitialized { get; } = "PythonAILibManagerが初期化されていません";

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


        // "ChatResultがnullです。"
        public virtual string ChatResultNull { get; } = "ChatResultがnullです。";

        // ChatResultのResponseが不正です。
        public virtual string ChatResultResponseInvalid { get; } = "ChatResultのResponseが不正です。";

        // ChatResultのResponseにResultが含まれていません。
        public virtual string ChatResultResponseResultNotFound { get; } = "ChatResultのResponseにResultが含まれていません。";


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

        // GetTokenCountExecute
        public virtual string GetTokenCountExecute { get; } = "GetTokenCount実行";

        // UpdateVectorDBIndex実行
        public virtual string UpdateVectorDBIndexExecute { get; } = "UpdateVectorDBIndex実行";

        // ベクトルDBのコレクション削除を実行
        public virtual string DeleteVectorDBCollectionExecute { get; } = "ベクトルDBのコレクション削除を実行";
        // ベクトルDBのコレクション更新を実行
        public virtual string UpdateVectorDBCollectionExecute { get; } = "ベクトルDBのコレクション更新を実行";
        // GetVectorDBDescription
        public virtual string GetVectorDBDescription { get; } = "ベクトルDBの説明を取得";
        // UpdateVectorDBDescription
        public virtual string UpdateVectorDBDescription { get; } = "ベクトルDBの説明を更新";

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

        // ベクトルDB
        public virtual string VectorDBItems { get; } = "ベクトルDBアイテム";

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

        // テキストを抽出しました
        public virtual string TextExtracted { get; } = "テキストを抽出しました";

        // 更新日
        public virtual string UpdateDate { get; } = "更新日";

        // ベクトル化日時
        public virtual string VectorizedDate { get; } = "ベクトル化日時";
        // タイトル
        public virtual string Title { get; } = "タイトル";

        // ソースタイトル
        public virtual string SourceTitle { get; } = "ソースタイトル";

        // SourcePath
        public virtual string SourcePath { get; } = "ソースパス";

        // ピン留め
        public virtual string Pin { get; } = "ピン留め";

        // 文章の信頼度
        public virtual string DocumentReliability { get; } = "文章の信頼度";
        
        // 文章カテゴリ概要
        public virtual string DocumentCategorySummary { get; } = "文章カテゴリ";

        // 種別
        public virtual string Type { get; } = "種別";

        // 作成日時
        public virtual string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public virtual string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public virtual string Pinned { get; } = "ピン留めしてます";
        // タグ
        public virtual string Tag { get; } = "タグ";

        // 背景情報
        public virtual string BackgroundInformation { get; } = "背景情報";

        // --- ScreenShotCheckCondition.cs ---

        public virtual string CheckTypeEqual { get; } = "等しい";
        public virtual string CheckTypeNotEqual { get; } = "等しくない";
        public virtual string CheckTypeInclude { get; } = "含む";
        public virtual string CheckTypeNotInclude { get; } = "含まない";
        public virtual string CheckTypeStartWith { get; } = "開始している";
        public virtual string CheckTypeNotStartWith { get; } = "開始していない";
        public virtual string CheckTypeEndWith { get; } = "終わっている";
        public virtual string CheckTypeNotEndWith { get; } = "終わっていない";
        public virtual string CheckTypeEmpty { get; } = "空である";
        public virtual string CheckTypeCheckBox { get; } = "チェックボックス";


        //  $"{SettingItem}の値は{SettingValue}である";
        public virtual string SettingValueIs(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}である";
        }

        // $"{SettingItem}の値は{SettingValue}でない";
        public virtual string SettingValueIsNot(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値は{SettingValue}でない";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれている";
        public virtual string SettingValueContains(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれている";
        }
        // $"{SettingItem}の値に{SettingValue}が含まれていない";
        public virtual string SettingValueNotContain(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値に{SettingValue}が含まれていない";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっている";
        public virtual string SettingValueStartsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっている";
        }

        // $"{SettingItem}の値が{SettingValue}で始まっていない";
        public virtual string SettingValueNotStartWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で始まっていない";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっている";
        public virtual string SettingValueEndsWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっている";
        }
        // $"{SettingItem}の値が{SettingValue}で終わっていない";
        public virtual string SettingValueNotEndWith(string SettingItem, string SettingValue) {
            return $"{SettingItem}の値が{SettingValue}で終わっていない";
        }

        //  $"{SettingItem}の値が空である";
        public virtual string SettingValueIsEmpty(string SettingItem) {
            return $"{SettingItem}の値が空である";
        }
        // $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        public virtual string SettingValueIsChecked(string SettingItem, string SettingValue) {
            return $"{SettingItem}のチェックボックスが{SettingValue}になっている";
        }

        // リモートリポジトリが設定されていません
        public virtual string NoRemoteRepositorySet { get; } = "リモートリポジトリが設定されていません";

        // 作業ディレクトリが指定されていません
        public virtual string NoWorkingDirectorySpecified { get; } = "作業ディレクトリが指定されていません";

        // "指定されたディレクトリが存在しません"
        public virtual string SpecifiedDirectoryDoesNotExist { get; } = "指定されたディレクトリが存在しません";

        // "指定されたディレクトリはGitリポジトリではありません"
        public virtual string SpecifiedDirectoryIsNotAGitRepository { get; } = "指定されたディレクトリはGitリポジトリではありません";

        // "ベクトルDBが設定されていません"
        public virtual string NoVectorDBSet { get; } = "ベクトルDBが設定されていません";
        // サポートされていないファイル形式です
        public virtual string UnsupportedFileType { get; } = "サポートされていないファイル形式です";

        // "Embeddingを保存します
        public virtual string SaveEmbedding { get; } = "Embeddingを保存します";
        // Embeddingを保存しました
        public virtual string SavedEmbedding { get; } = "Embeddingを保存しました";
        // Embeddingを削除します
        public virtual string DeleteEmbedding { get; } = "Embeddingを削除します";

        // Embeddingを削除しました
        public virtual string DeletedEmbedding { get; } = "Embeddingを削除しました";


        // 画像から抽出したテキストのEmbeddingを保存します
        public virtual string SaveTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存します";
        // 画像から抽出したテキストのEmbeddingを保存しました
        public virtual string SavedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを保存しました";

        // 画像から抽出したテキストのEmbeddingを削除します
        public virtual string DeleteTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除します";
        // 画像から抽出したテキストのEmbeddingを削除しました
        public virtual string DeletedTextEmbeddingFromImage { get; } = "画像から抽出したテキストのEmbeddingを削除しました";

        //  "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        public virtual string GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions { get; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";



        // InputContentNotFound
        public virtual string InputContentNotFound { get; } = "入力内容が見つかりません";


        // "アイテムを追加しました"
        public virtual string AddedItems { get; } = "アイテムを追加しました";

        // --- SystemAutoProcessItem.cs ---
        // 無視
        public virtual string Ignore { get; } = "無視";
        // "何もしません"
        public virtual string DoNothing { get; } = "何もしません";

        // フォルダにコピー
        public virtual string CopyToFolder { get; } = "フォルダにコピー";
        // クリップボードの内容を指定されたフォルダにコピーします
        public virtual string CopyClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダにコピーします";
        // テキストを抽出
        public virtual string ExtractText { get; } = "テキストを抽出";

        // フォルダに移動"
        public virtual string MoveToFolder { get; } = "フォルダに移動";
        // "クリップボードの内容を指定されたフォルダに移動します"
        public virtual string MoveClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダに移動します";

        // "クリップボードのテキストを抽出します"
        public virtual string ExtractClipboardText { get; } = "クリップボードのテキストを抽出します";

        // "データマスキング",
        public virtual string DataMasking { get; } = "データマスキング";
        // "クリップボードのテキストをマスキングします"
        public virtual string MaskClipboardText { get; } = "クリップボードのテキストをマスキングします";

        //  "フォルダ内のアイテムをマージ", 
        public virtual string MergeItemsInFolder { get; } = "フォルダ内のアイテムをマージ";

        // "フォルダ内のアイテムをマージします"
        public virtual string MergeItemsInFolderDescription { get; } = "フォルダ内のアイテムをマージします";

        // "同じSourceApplicationTitleを持つアイテムをマージ",
        public virtual string MergeItemsWithTheSameSourceApplicationTitle { get; } = "同じSourceApplicationTitleを持つアイテムをマージ";
        // "同じSourceApplicationTitleを持つアイテムをマージします"
        public virtual string MergeItemsWithTheSameSourceApplicationTitleDescription { get; } = "同じSourceApplicationTitleを持つアイテムをマージします";

        // フォルダが選択されていません
        public virtual string NoFolderSelected { get; } = "フォルダが選択されていません";

        // フォルダにコピーします
        public virtual string CopyToFolderDescription { get; } = "フォルダにコピーします";

        // ディレクトリは新規ファイルとして開けません
        public virtual string CannotOpenDirectoryAsNewFile { get; } = "ディレクトリは新規ファイルとして開けません";

        // --- AutoProcessRule.cs ---
        // RuleName + "は無効です"
        public virtual string RuleNameIsInvalid(string RuleName) {
            return RuleName + "は無効です";
        }
        // 条件にマッチしませんでした
        public virtual string NoMatch { get; } = "条件にマッチしませんでした";

        // アクションが設定されていません
        public virtual string NoActionSet { get; } = "アクションが設定されていません";

        // 条件
        public virtual string Condition { get; } = "条件";

        // アクション
        public virtual string Action { get; } = "アクション";

        // アクション:なし
        public virtual string ActionNone { get; } = "アクション:なし";

        // フォルダ:なし
        public virtual string FolderNone { get; } = "フォルダ:なし";

        // 無限ループを検出しました
        public virtual string DetectedAnInfiniteLoop { get; } = "無限ループを検出しました";

        // "Descriptionが" + condition.Keyword + "を含む
        public virtual string DescriptionContains(string Keyword) {
            return "Descriptionが" + Keyword + "を含む";
        }
        // "Contentが" + condition.Keyword + "を含む 
        public virtual string ContentContains(string Keyword) {
            return "Contentが" + Keyword + "を含む";
        }
        // "SourceApplicationNameが" + condition.Keyword + "を含む \n";
        public virtual string SourceApplicationNameContains(string Keyword) {
            return "SourceApplicationNameが" + Keyword + "を含む \n";
        }
        // "SourceApplicationTitleが" + condition.Keyword + "を含む
        public virtual string SourceApplicationTitleContains(string Keyword) {
            return "SourceApplicationTitleが" + Keyword + "を含む";
        }
        // "SourceApplicationPathが" + condition.Keyword + "を含む
        public virtual string SourceApplicationPathContains(string Keyword) {
            return "SourceApplicationPathが" + Keyword + "を含む";
        }
        // 自動イメージテキスト抽出処理を実行します
        public virtual string AutoExtractImageText { get; } = "自動イメージテキスト抽出処理を実行します";

        // File
        public virtual string File { get; } = "ファイル";
        // Folder
        public virtual string Folder { get; } = "フォルダ";


        // 自動背景情報追加処理を実行します
        public virtual string AutoSetBackgroundInfo { get; } = "自動背景情報追加処理を実行します";
        // 背景情報追加処理が失敗しました
        public virtual string AddBackgroundInfoFailed { get; } = "背景情報追加処理が失敗しました";

        // 自動サマリー作成処理を実行します
        public virtual string AutoCreateSummary { get; } = "自動サマリー作成処理を実行します";
        // サマリー作成処理が失敗しました

        // 自動文書信頼度チェック処理を実行します
        public virtual string AutoCheckDocumentReliability { get; } = "自動文書信頼度チェック処理を実行します";
        // 文書信頼度チェック処理が失敗しました
        public virtual string CheckDocumentReliabilityFailed { get; } = "文書信頼度チェック処理が失敗しました";

        public virtual string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";

        // 自動課題リスト作成処理を実行します
        public virtual string AutoCreateTaskList { get; } = "自動課題リスト作成処理を実行します";
        // 課題リスト作成処理が失敗しました
        public virtual string CreateTaskListFailed { get; } = "課題リスト作成処理が失敗しました";


        // 自動処理を適用します
        public virtual string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理でアイテムが削除されました
        public virtual string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";


        #region 統計、ログ関連

        // 日次トークン数
        public virtual string DailyTokenCount { get; } = "日次トークン数";
        // 総トークン数
        public virtual string TotalTokenFormat(long tokens) {
            return $"総トークン数: {tokens} トークン";
        }
        // トークン数
        public virtual string TokenCount { get; } = "トークン数";

        public virtual string DailyTokenFormat(string date, long totalTokens) {
            return $"{date}のトークン数: {totalTokens} トークン";
        }
        #endregion
    }
}

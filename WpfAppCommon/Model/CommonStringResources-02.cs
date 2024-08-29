namespace WpfAppCommon.Model {
    public partial class CommonStringResources {

        // --- namespace WpfAppCommon.PythonIF ---

        // --- DefaultClipboardController.cs ---
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

        // 自動背景情報追加処理を実行します
        public virtual string AutoSetBackgroundInfo { get; } = "自動背景情報追加処理を実行します";
        // 背景情報追加処理が失敗しました
        public virtual string AddBackgroundInfoFailed { get; } = "背景情報追加処理が失敗しました";

        // 自動サマリー作成処理を実行します
        public virtual string AutoCreateSummary { get; } = "自動サマリー作成処理を実行します";
        // サマリー作成処理が失敗しました
        public virtual string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";


        // 自動イメージテキスト抽出処理を実行します
        public virtual string AutoExtractImageText { get; } = "自動イメージテキスト抽出処理を実行します";
        // イメージテキスト抽出処理が失敗しました
        public virtual string ExtractImageTextFailed { get; } = "イメージテキスト抽出処理が失敗しました";

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

        // クリップボードアプリ用のPythonスクリプト
        public virtual string WpfAppCommonUtilsScript { get; } = "python/ai_app.py";

        // テンプレートファイルが見つかりません
        public virtual string TemplateScriptNotFound { get; } = "テンプレートファイルが見つかりません";

        // --- PythonNetFunctions.cs ---
        // "PythonDLLが見つかりません。PythonDLLのパスを確認してください:"
        public virtual string PythonDLLNotFound { get; } = "PythonDLLが見つかりません。PythonDLLのパスを確認してください:";
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

        // -- ClipboardApp.MainWindowDataGrid1 --
        // 更新日
        public virtual string UpdateDate { get; } = "更新日";
        // タイトル
        public virtual string Title { get; } = "タイトル";

        // ソースタイトル
        public virtual string SourceTitle { get; } = "ソースタイトル";
        // ピン留め
        public virtual string Pin { get; } = "ピン留め";

        // 種別
        public virtual string Type { get; } = "種別";

        // -- AutoProcessRule --
        // 自動処理ルール

        // ルール名
        public virtual string RuleName { get; } = "ルール名";

        // 有効
        public virtual string Enable { get; } = "有効";

        // 適用対象フォルダ
        public virtual string TargetFolder { get; } = "適用対象フォルダ";

        // すべてのアイテムに適用
        public virtual string ApplyAllItems { get; } = "すべてのアイテムに適用";

        // 次の条件に合致するアイテムに適用
        public virtual string ApplyMatchedItems { get; } = "次の条件に合致するアイテムに適用";

        // アイテムの種類
        public virtual string ItemType { get; } = "アイテムの種類";
        // アイテムのタイプがテキストの場合
        public virtual string ItemTypeText { get; } = "アイテムのタイプがテキストの場合";
        // 行以上
        public virtual string LineOrMore { get; } = "行以上";

        // 行以下のテキスト
        public virtual string LineOrLess { get; } = "行以下のテキスト";

        // アイテムのタイプがファイルの場合
        public virtual string ItemTypeFile { get; } = "アイテムのタイプがファイルの場合";

        // アイテムのタイプがイメージの場合
        public virtual string ItemTypeImage { get; } = "アイテムのタイプがイメージの場合";

        // タイトルに次の文字が含まれる場合
        public virtual string TitleContains { get; } = "タイトルに次の文字が含まれる場合";

        // 本文に次の文字列が含まれる場合
        public virtual string BodyContains { get; } = "本文に次の文字列が含まれる場合";

        // ソースアプリの名前に次の文字列が含まれる場合
        public virtual string SourceAppContains { get; } = "ソースアプリの名前に次の文字列が含まれる場合";

        // 実行する処理
        public virtual string ExecuteProcess { get; } = "実行する処理";

        // 次の処理を実行する
        public virtual string ExecuteNextProcess { get; } = "次の処理を実行する";

        // コピー/移動/マージ先
        public virtual string CopyMoveMergeTarget { get; } = "コピー/移動/マージ先";

        // Pythonスクリプトを実行する
        public virtual string ExecutePythonScript { get; } = "Pythonスクリプトを実行する";

        // OpenAIのプロンプトを実行する
        public virtual string ExecuteOpenAI { get; } = "OpenAIのプロンプトを実行する";

        // OpenAIの実行モード
        public virtual string OpenAIMode { get; } = "OpenAIの実行モード";

        // ベクトルDBに格納する
        public virtual string StoreVectorDB { get; } = "ベクトルDBに格納する";

        // 適用対象フォルダ(パス)
        public virtual string TargetFolderFullPath { get; } = "適用対象フォルダ(パス)";

        // フォルダ単位
        public virtual string FolderUnit { get; } = "フォルダ単位";

        // 上へ
        public virtual string Up { get; } = "上へ";
        // 下へ
        public virtual string Down { get; } = "下へ";

        // クリップボード監視対象のソースアプリ名
        public virtual string SourceApp { get; } = "クリップボード監視対象のソースアプリ名";

        // 監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe
        public virtual string SourceAppExample { get; } = "監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe";

        // 指定した行数以下のテキストアイテムを無視
        public virtual string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "指定した行数以下のテキストアイテムを無視";

        // 自動タイトル生成
        public virtual string AutoTitleGeneration { get; } = "自動タイトル生成";

        // しない
        public virtual string DoNot { get; } = "しない";

        // OpenAIを使用して自動的にタイトルを生成する
        public virtual string AutomaticallyGenerateTitleUsingOpenAI { get; } = "OpenAIを使用して自動的にタイトルを生成する";

        // 自動でタグ生成する
        public virtual string AutomaticallyGenerateTags { get; } = "自動でタグ生成する";

        // クリップボードの内容から自動的にタグを生成します
        public virtual string AutomaticallyGenerateTagsFromClipboardContent { get; } = "クリップボードの内容から自動的にタグを生成します";

        // 自動でマージ
        public virtual string AutomaticallyMerge { get; } = "自動でマージ";

        // コピー元のアプリ名、タイトルが同じ場合にアイテムを自動的にマージします
        public virtual string AutomaticallyMergeItemsIfSourceAppAndTitleAreTheSame { get; } = "コピー元のアプリ名、タイトルが同じ場合にアイテムを自動的にマージします";

        // 自動でEmbedding
        public virtual string AutomaticallyEmbedding { get; } = "自動でEmbedding";

        // クリップボードアイテム保存時に自動でEmbeddingを行います
        public virtual string AutomaticallyEmbeddingWhenSavingClipboardItems { get; } = "クリップボードアイテム保存時に自動でEmbeddingを行います";

        // ファイルから自動でテキスト抽出
        public virtual string AutomaticallyExtractTextFromFile { get; } = "ファイルから自動でテキスト抽出";

        // クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います
        public virtual string AutomaticallyExtractTextFromFileIfClipboardItemIsFile { get; } = "クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います";

        // 画像から自動でテキスト抽出
        public virtual string AutomaticallyExtractTextFromImage { get; } = "画像から自動でテキスト抽出";

        // PyOCRを使用してテキスト抽出します
        public virtual string ExtractTextUsingPyOCR { get; } = "PyOCRを使用してテキスト抽出します";

        // OpenAIを使用してテキスト抽出します
        public virtual string ExtractTextUsingOpenAI { get; } = "OpenAIを使用してテキスト抽出します";

        // 画像からテキスト抽出時にEmbedding
        public virtual string EmbeddingWhenExtractingTextFromImage { get; } = "画像からテキスト抽出時にEmbedding";
        // 画像からテキスト抽出時にEmbeddingを行います
        public virtual string EmbeddingWhenExtractingTextFromImageDescription { get; } = "画像からテキスト抽出時にEmbeddingを行います";


        // 自動背景情報追加
        public virtual string AutomaticallyAddBackgroundInformation { get; } = "自動背景情報追加";

        // 自動背景情報に日本語文章解析結果を追加します
        public virtual string AutomaticallyAddJapaneseSentenceAnalysisResultsToBackgroundInformation { get; } = "(実験的機能)自動背景情報に日本語文章解析結果を追加します";

        // 同じフォルダにあるアイテムから背景情報を生成します。
        public virtual string GenerateBackgroundInformationFromItemsInTheSameFolder { get; } = "同じフォルダにあるアイテムから背景情報を生成します。";

        // Embeddingに背景情報を含める
        public virtual string IncludeBackgroundInformationInEmbedding { get; } = "Embeddingに背景情報を含める";

        // Embedding対象テキストに背景情報を含めます。
        public virtual string IncludeBackgroundInformationInEmbeddingTargetText { get; } = "Embedding対象テキストに背景情報を含めます。";

        // 自動サマリー生成
        public virtual string AutomaticallyGenerateSummary { get; } = "自動サマリー生成";

        // コンテンツからサマリーテキストを生成します。
        public virtual string GenerateSummaryTextFromContent { get; } = "コンテンツからサマリーテキストを生成します。";

        // クリップボードアイテムをOS上のフォルダと同期させる
        public virtual string SynchronizeClipboardItemsWithFoldersOnTheOS { get; } = "クリップボードアイテムをOS上のフォルダと同期させる";

        // クリップボードアイテムをOS上のフォルダと同期させます。
        public virtual string SynchronizeClipboardItemsWithFoldersOnTheOSDescription { get; } = "クリップボードアイテムをOS上のフォルダと同期させます。";

        // 同期先のフォルダ名
        public virtual string SyncTargetFolderName { get; } = "同期先のフォルダ名";

        // クリップボードアイテムを同期するOS上のフォルダ名を指定。
        public virtual string SpecifyTheFolderNameOnTheOSToSynchronizeTheClipboardItems { get; } = "クリップボードアイテムを同期するOS上のフォルダ名を指定。";

        // 同期先のフォルダがGitリポジトリの場合、ファイル更新時に自動的にコミットします。
        public virtual string IfTheSyncTargetFolderIsAGitRepositoryItWillAutomaticallyCommitWhenTheFileIsUpdated { get; } = "同期先のフォルダがGitリポジトリの場合、ファイル更新時に自動的にコミットします。";

        // エンティティ抽出/データマスキング
        public virtual string EntityExtractionDataMasking { get; } = "エンティティ抽出/データマスキング";

        // クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います
        public virtual string ExtractEntitiesAndMaskDataUsingSpacyFromClipboardContent { get; } = "クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います";

        // OpenAIに送信するデータ内の個人情報などをマスキングします。
        public virtual string MaskPersonalInformationInDataSentToOpenAI { get; } = "OpenAIに送信するデータ内の個人情報などをマスキングします。";

        // 新規自動処理ルール
        public virtual string NewAutoProcessRule { get; } = "新規自動処理ルール";

        // システム共通設定を保存
        public virtual string SaveSystemCommonSettings { get; } = "システム共通設定を保存";

        // -- FolderEditWindow --
        // クリップボードフォルダ編集
        public virtual string EditClipboardFolder { get; } = "クリップボードフォルダ編集";

        // 名前
        public virtual string Name { get; } = "名前";

        // 説明
        public virtual string Description { get; } = "説明";

        // フォルダ選択
        public virtual string SelectFolder { get; } = "フォルダ選択";

        // ファイル選択
        public virtual string SelectFile { get; } = "ファイル選択";

    }
}

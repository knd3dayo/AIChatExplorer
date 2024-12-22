namespace QAChat.Resource {
    public partial class CommonStringResources {

        #region RAG Clipboard共通
        public virtual string AppName { get; } = "RAG Clipboard";

        #endregion

        #region 設定画面
        // 設定
        public virtual string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // 設定チェック結果
        public virtual string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - 設定チェック結果";
            }
        }
        // -- 設定を反映させるためにアプリケーションの再起動を行ってください。
        public virtual string RestartAppToApplyChanges { get; } = "設定を反映させるためにアプリケーションの再起動を行ってください。";

        // 基本設定
        public virtual string BasicSettings { get; } = "基本設定";

        // 詳細設定
        public virtual string DetailSettings { get; } = "詳細設定";

        // Pythonインストール先のpython3**.dllを指定
        public virtual string SpecifyPython3Dll { get; } = "Pythonインストール先のpython3**.dllを指定";

        // PythonDLLのパス
        public virtual string PythonDLLPath { get; } = "PythonDLLのパス";

        // Python仮想環境の場所
        public virtual string PythonVenvPath { get; } = "Python仮想環境の場所";

        // Python venvを使用する場合はvenvの場所を設定
        public virtual string SpecifyVenvPath { get; } = "Python venvを使用する場合はvenvの場所を設定";

        // クリップボードDBのバックアップ世代数
        public virtual string ClipboardDBBackupGenerations { get; } = "クリップボードDBのバックアップ世代数";

        // clipbord.db,clipboard-log.dbのバックアップ世代数
        public virtual string ClipboardDBBackupGenerationsDescription { get; } = "clipbord.db,clipboard-log.dbのバックアップ世代数";

        // OpenAI設定
        public virtual string OpenAISettings { get; } = "OpenAI設定";

        // OpenAIのAPI Key
        public virtual string OpenAIKey { get; } = "OpenAIのAPI Key";

        // OpenAIまたはAzure OpenAIのAPIキーを設定
        public virtual string SetOpenAIKey { get; } = "OpenAIまたはAzure OpenAIのAPIキーを設定";

        // Azure OpenAIを使用する
        public virtual string UseAzureOpenAI { get; } = "Azure OpenAIを使用する";

        // OpenAIの代わりにAzure OpenAIを使用します
        public virtual string UseAzureOpenAIInsteadOfOpenAI { get; } = "OpenAIの代わりにAzure OpenAIを使用します";

        // Azure OpenAIのエンドポイント
        public virtual string AzureOpenAIEndpoint { get; } = "Azure OpenAIのエンドポイント";

        // Azure OpenAIを使用する場合はAzure OpenAIのエンドポイントを設定する
        public virtual string SetAzureOpenAIEndpoint { get; } = "Azure OpenAIを使用する場合はAzure OpenAIのエンドポイントを設定する";

        // OpenAIのチャットで使用するモデル
        public virtual string OpenAIModel { get; } = "OpenAIのチャットで使用するモデル";

        // OpenAIまたはAzure OpenAIのチャット用モデルを設定。例：　gpt-4-turbo,gpt-4-1106-previewなど
        public virtual string SetOpenAIModel { get; } = "OpenAIまたはAzure OpenAIのチャット用モデルを設定。例：　gpt-4-turbo,gpt-4-1106-previewなど";

        // OpenAIのEmbeddingで使用するモデル
        public virtual string OpenAIEmbeddingModel { get; } = "OpenAIのEmbeddingで使用するモデル";

        // OpenAIまたはAzure OpenAIのEmbedding用モデルを設定。例：　text-embedding-ada-002,text-embedding-3-smallなど
        public virtual string SetOpenAIEmbeddingModel { get; } = "OpenAIまたはAzure OpenAIのEmbedding用モデルを設定。例：　text-embedding-ada-002,text-embedding-3-smallなど";

        // OpenAIのチャットモデルのBaseUR
        public virtual string OpenAIChatBaseURL { get; } = "OpenAIのチャットモデルのBaseUR";

        // OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定
        public virtual string SetOpenAIChatBaseURL { get; } = "OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定";

        // OpenAIのEmbeddingモデルのBaseURL
        public virtual string OpenAIEmbeddingBaseURL { get; } = "OpenAIのEmbeddingモデルのBaseURL";

        // OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定
        public virtual string SetOpenAIEmbeddingBaseURL { get; } = "OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定";

        // Python Spacy設定
        public virtual string PythonSpacySettings { get; } = "Python Spacy設定";

        // Spacyのモデル名
        public virtual string SpacyModelName { get; } = "Spacyのモデル名";

        // インストール済みのSpacyのモデル名を指定。例:ja_core_news_sm,ja_core_news_lgなど
        public virtual string SetSpacyModelName { get; } = "インストール済みのSpacyのモデル名を指定。例:ja_core_news_sm,ja_core_news_lgなど";

        // Python OCR設定
        public virtual string PythonOCRSettings { get; } = "Python OCR設定";

        // Tesseractのパス
        public virtual string TesseractPath { get; } = "Tesseractのパス";

        // ProxyURL
        public virtual string ProxyURL { get; } = "ProxyサーバーのURL";
        // NoProxyList
        public virtual string NoProxyList { get; } = "Proxy除外リスト";

        // その他
        public virtual string Other { get; } = "その他";

        // 開発中機能を有効にする
        public virtual string EnableDevelopmentFeatures { get; } = "開発中機能を有効にする";

        // 設定のチェック
        public virtual string CheckSettings { get; } = "設定のチェック";


        #endregion

        #region 設定画面 詳細設定
        // クリップボード監視対象のソースアプリ名
        public virtual string SourceApp { get; } = "クリップボード監視対象のソースアプリ名";

        // 監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe
        public virtual string SourceAppExample { get; } = "監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe";

        // 指定した行数以下のテキストアイテムを無視
        public virtual string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "指定した行数以下のテキストアイテムを無視";

        // 自動タイトル生成
        public virtual string AutoTitleGeneration { get; } = "自動タイトル生成";


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

        // 自動背景情報に自動QA結果を追加します
        public virtual string AutomaticallyAddAutoQAResultsToBackgroundInformation { get; } = "(実験的機能)自動背景情報に自動QA結果を追加します";

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

        // 自動課題リスト生成
        public virtual string AutomaticallyGenerateTaskList { get; } = "自動課題リスト生成";

        // コンテンツから課題リストを生成します。
        public virtual string GenerateTaskListFromContent { get; } = "コンテンツから課題リストを生成します。";

        // コンテンツの文書信頼度をチェックします。
        public virtual string CheckDocumentReliabilityOfContent { get; } = "コンテンツの文書信頼度をチェックします。";

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

        #endregion

        #region 設定画面　イベント
        // --- SettingUserControlViewModel
        // Pythonの設定チェック
        public virtual string PythonSettingCheck { get; } = "Pythonの設定チェック";

        // PythonDLLのパスが設定されていません
        public virtual string PythonDLLPathNotSet { get; } = "PythonDLLのパスが設定されていません";

        // PythonDLLのパスが設定されています
        public virtual string PythonDLLPathSet { get; } = "PythonDLLのパスが設定されています";

        // PythonDLLのファイルが存在しません
        public virtual string PythonDLLFileDoesNotExist { get; } = "PythonDLLのファイルが存在しません";

        // PythonDLLのファイルが存在します
        public virtual string PythonDLLFileExists { get; } = "PythonDLLのファイルが存在します";

        // Pythonスクリプトをテスト実行
        public virtual string TestRunPythonScript { get; } = "Pythonスクリプトをテスト実行";

        // OpenAIの設定チェック
        public virtual string OpenAISettingCheck { get; } = "OpenAIの設定チェック";

        // OpenAIのAPIキーが設定されていません
        public virtual string OpenAIKeyNotSet { get; } = "OpenAIのAPIキーが設定されていません";
        // OpenAIのAPIキーが設定されています
        public virtual string OpenAIKeySet { get; } = "OpenAIのAPIキーが設定されています";

        // OpenAIのCompletionModelが設定されていません
        public virtual string OpenAICompletionModelNotSet { get; } = "OpenAIのCompletionModelが設定されていません";

        // OpenAIのCompletionModelが設定されています
        public virtual string OpenAICompletionModelSet { get; } = "OpenAIのCompletionModelが設定されています";

        // OpenAIのEmbeddingModelが設定されていません
        public virtual string OpenAIEmbeddingModelNotSet { get; } = "OpenAIのEmbeddingModelが設定されていません";

        // OpenAIのEmbeddingModelが設定されています
        public virtual string OpenAIEmbeddingModelSet { get; } = "OpenAIのEmbeddingModelが設定されています";

        // Azure OpenAIの設定チェック
        public virtual string AzureOpenAISettingCheck { get; } = "Azure OpenAIの設定チェック";

        // Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック
        public virtual string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック";

        // Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください
        public virtual string SetAzureOpenAIEndpointOrBaseURL { get; } = "Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください";

        // Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません
        public virtual string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません";

        // OpenAIのテスト実行
        public virtual string TestRunOpenAI { get; } = "OpenAIのテスト実行";

        // Pythonの実行に失敗しました
        public virtual string FailedToRunPython { get; } = "Pythonの実行に失敗しました";

        // Pythonの実行が可能です
        public virtual string PythonRunIsPossible { get; } = "Pythonの実行が可能です";

        // OpenAIの実行に失敗しました
        public virtual string FailedToRunOpenAI { get; } = "OpenAIの実行に失敗しました";

        // OpenAIの実行が可能です。
        public virtual string OpenAIRunIsPossible { get; } = "OpenAIの実行が可能です。";

        // LangChainの実行に失敗しました
        public virtual string FailedToRunLangChain { get; } = "LangChainの実行に失敗しました";

        // LangChainの実行が可能です。
        public virtual string LangChainRunIsPossible { get; } = "LangChainの実行が可能です。";

        #endregion

        #region メイン画面

        #endregion

        #region ToolTip
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public virtual string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.
        public virtual string ToggleNotificationWatchToolTop { get; } = "開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.";
 
        #endregion

        #region イベント


        // クリップボード監視を開始しました
        public virtual string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public virtual string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";
        // Windows通知監視を開始しました
        public virtual string StartNotificationWatchMessage { get; } = "Windows通知監視を開始しました";
        // Windows通知監視を停止しました
        public virtual string StopNotificationWatchMessage { get; } = "Windows通知監視を停止しました";

        // AutoGenStudioを開始しました
        public virtual string StartAutoGenStudioMessage { get; } = "AutoGenStudioを開始しました";
        // AutoGenStudioを停止しました
        public virtual string StopAutoGenStudioMessage { get; } = "AutoGenStudioを停止しました";

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

        // 自動文書信頼度チェック処理を実行します
        public virtual string AutoCheckDocumentReliability { get; } = "自動文書信頼度チェック処理を実行します";
        // 文書信頼度チェック処理が失敗しました
        public virtual string CheckDocumentReliabilityFailed { get; } = "文書信頼度チェック処理が失敗しました";

        public virtual string CreateSummaryFailed { get; } = "サマリー作成処理が失敗しました";

        // 自動課題リスト作成処理を実行します
        public virtual string AutoCreateTaskList { get; } = "自動課題リスト作成処理を実行します";
        // 課題リスト作成処理が失敗しました
        public virtual string CreateTaskListFailed { get; } = "課題リスト作成処理が失敗しました";

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


        #endregion

        #region EditItemWindow関連
        // 新規アイテム
        public virtual string NewItem { get; } = "新規アイテム";
        #endregion

        #region FolderView関連
        // 参照先ベクトルDBに保存用ベクトルDBも追加する
        public virtual string AddVectorDBForSaveToReferenceVectorDB { get; } = "参照先ベクトルDBに保存用ベクトルDBも追加する";

        // こにフォルダの説明を入力
        public virtual string InputDescriptionOfThisFolder { get; } = "こにフォルダの説明を入力";

        #endregion

        #region ClipboardApp MainWindow

        // クリップボード監視開始
        public virtual string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public virtual string StopClipboardWatch { get; } = "クリップボード監視停止";
        // Windows通知監視開始
        public virtual string StartNotificationWatch { get; } = "Windows通知監視開始";
        // Windows通知監視停止
        public virtual string StopNotificationWatch { get; } = "Windows通知監視停止";

        // タグ編集
        public virtual string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public virtual string EditAutoProcessRule { get; } = "自動処理ルール編集";
        // Pythonスクリプト編集
        public virtual string EditPythonScript { get; } = "Pythonスクリプト編集";
        // プロンプトテンプレート編集
        public virtual string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public virtual string EditGitRagSource { get; } = "RAGソース(git)編集";
        // AutoGen定義編集
        public virtual string EditAutoGenDefinition { get; } = "AutoGen定義編集";


        // -- 表示メニュー
        // テキストを右端で折り返す
        public virtual string TextWrapping { get; } = "テキストを右端で折り返す";
        // サイズの大きなテキストの場合は自動的に折り返しを解除する
        public virtual string AutoTextWrapping { get; } = "サイズの大きなテキストの場合は自動的に折り返しを解除する";

        // プレビューモード
        public virtual string PreviewMode { get; } = "プレビューを有効にする";
        // コンパクト表示モード
        public virtual string CompactMode { get; } = "コンパクト表示にする";

        // ツール
        public virtual string Tool { get; } = "ツール";
        // OpenAIチャット
        public virtual string OpenAIChat { get; } = "OpenAIチャット";
        // 画像エビデンスチェッカー
        public virtual string ImageChat { get; } = "イメージチャット";
        // ローカルファイルシステム
        public virtual string FileSystem { get; } = "ファイルシステム";

        // Shortcut
        public virtual string Shortcut { get; } = "ショートカット";

        #endregion

        #region SettingWindow
        #endregion

        #region 未整理

        #endregion
    }
}

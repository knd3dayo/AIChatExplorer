namespace LibUIPythonAI.Resource {
    public partial class CommonStringResourcesJa {

        #region RAG Clipboard共通
        public override string AppName { get; } = "AIChatExplorer";

        #endregion

        #region 設定画面
        // 設定
        public override string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // 設定チェック結果
        public override string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - 設定チェック結果";
            }
        }
        // -- 設定を反映させるためにアプリケーションの再起動を行ってください。
        public override string RestartAppToApplyChanges { get; } = "設定を反映させるためにアプリケーションの再起動を行ってください。";

        // 基本設定
        public override string BasicSettings { get; } = "基本設定";

        // 詳細設定
        public override string DetailSettings { get; } = "詳細設定";

        // Pythonインストール先のpython3**.dllを指定
        public override string SpecifyPython3Dll { get; } = "Pythonインストール先のpython3**.dllを指定";

        // PythonDLLのパス
        public override string PythonDLLPath { get; } = "PythonDLLのパス";

        // AppDataPath
        public override string AppDataPath { get; } = "アプリケーションデータ格納先";
        // AppDataPathの場所を設定
        public override string AppDataPathHint { get; } = "アプリケーションデータ格納先の場所を設定";

        // Python仮想環境の場所
        public override string PythonVenvPath { get; } = "Python仮想環境の場所";

        // Python venvを使用する場合はvenvの場所を設定
        public override string PythonVenvPathHint { get; } = "Python venvを使用する場合はvenvの場所を設定";

        // クリップボードDBのバックアップ世代数
        public override string ApplicationDBBackupGenerations { get; } = "メインDBのバックアップ世代数";

        // clipbord.db,clipboard-log.dbのバックアップ世代数
        public override string ApplicationDBBackupGenerationsDescription { get; } = "clipbord.db,clipboard-log.dbのバックアップ世代数";

        // OpenAI設定
        public override string OpenAISettings { get; } = "OpenAI設定";

        // OpenAIのAPI Key
        public override string OpenAIKey { get; } = "OpenAIのAPI Key";

        // OpenAIまたはAzure OpenAIのAPIキーを設定
        public override string SetOpenAIKey { get; } = "OpenAIまたはAzure OpenAIのAPIキーを設定";

        // Azure OpenAIを使用する
        public override string UseAzureOpenAI { get; } = "Azure OpenAIを使用する";

        // AzureOpenAIAPIVersion
        public override string AzureOpenAIAPIVersion { get; } = "AzureOpenAIのAPIバージョン";

        // OpenAIの代わりにAzure OpenAIを使用します
        public override string UseAzureOpenAIInsteadOfOpenAI { get; } = "OpenAIの代わりにAzure OpenAIを使用します";

        // Azure OpenAIのエンドポイント
        public override string AzureOpenAIEndpoint { get; } = "Azure OpenAIのエンドポイント";

        // Azure OpenAIを使用する場合はAzure OpenAIのエンドポイントを設定する
        public override string SetAzureOpenAIEndpoint { get; } = "Azure OpenAIを使用する場合はAzure OpenAIのエンドポイントを設定する";

        // OpenAIのチャットで使用するモデル
        public override string OpenAIModel { get; } = "OpenAIのチャットで使用するモデル";

        // OpenAIまたはAzure OpenAIのチャット用モデルを設定。例：　gpt-4-turbo,gpt-4-1106-previewなど
        public override string SetOpenAIModel { get; } = "OpenAIまたはAzure OpenAIのチャット用モデルを設定。例：　gpt-4-turbo,gpt-4-1106-previewなど";

        // OpenAIのEmbeddingで使用するモデル
        public override string OpenAIEmbeddingModel { get; } = "OpenAIのEmbeddingで使用するモデル";

        // OpenAIまたはAzure OpenAIのEmbedding用モデルを設定。例：　text-embedding-ada-002,text-embedding-3-smallなど
        public override string SetOpenAIEmbeddingModel { get; } = "OpenAIまたはAzure OpenAIのEmbedding用モデルを設定。例：　text-embedding-ada-002,text-embedding-3-smallなど";

        // OpenAIのチャットモデルのBaseUR
        public override string OpenAIBaseURL { get; } = "OpenAIのBaseURL";

        // OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定
        public override string SetOpenAIBaseURL { get; } = "OpenAIのデフォルトのエンドポイントやAzure OpenAIのエンドポイントと異なるエンドポイントを使用する場合に設定";

        // Python Spacy設定
        public override string PythonSpacySettings { get; } = "Python Spacy設定";

        // Spacyのモデル名
        public override string SpacyModelName { get; } = "Spacyのモデル名";

        // インストール済みのSpacyのモデル名を指定。例:ja_core_news_sm,ja_core_news_lgなど
        public override string SetSpacyModelName { get; } = "インストール済みのSpacyのモデル名を指定。例:ja_core_news_sm,ja_core_news_lgなど";

        // Python OCR設定
        public override string PythonOCRSettings { get; } = "Python OCR設定";

        // Tesseractのパス
        public override string TesseractPath { get; } = "Tesseractのパス";

        // ProxyURL
        public override string ProxyURL { get; } = "ProxyサーバーのURL";
        // NoProxyList
        public override string NoProxyList { get; } = "Proxy除外リスト";

        // その他
        public override string Other { get; } = "その他";

        // 開発中機能を有効にする
        public override string EnableDevelopmentFeatures { get; } = "開発中機能を有効にする";

        // 設定のチェック
        public override string CheckSettings { get; } = "設定のチェック";

        // PythonSettings
        public override string PythonSettings { get; } = "Python設定";

        // UseExternalAPIServer
        public override string UseExternalAPIServer { get; } = "外部APIサーバーを使用する";
        // UseInternalAPIServer
        public override string UseInternalAPIServer { get; } = "内部APIサーバーを使用する";

        // APIServerURL
        public override string APIServerURL { get; } = "APIサーバーURL";

        // SpecifiedAPIServerURL
        public override string SpecifyAPIServerURL { get; } = "APIサーバーURLを指定";

        // InternalAPIServerSettings
        public override string InternalAPIServerSettings { get; } = "内部APIサーバー設定";

        // ScreenMonitoringInterval
        public override string ScreenMonitoringInterval { get; } = "画面監視間隔";

        // IsAutoPredictUserIntentEnabled
        public override string AutoPredictUserIntent { get; } = "ユーザーの意図を自動予測する";

        // UsePythonNet
        public override string UsePythonNet { get; } = "PythonNetを使用する";

        #endregion

        #region 設定画面 詳細設定
        // クリップボード監視対象のソースアプリ名
        public override string SourceApp { get; } = "クリップボード監視対象のソースアプリ名";

        // 監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe
        public override string SourceAppExample { get; } = "監視対象のアプリ名をカンマ区切りで入力。例：notepad.exe,Teams.exe";

        // 指定した行数以下のテキストアイテムを無視
        public override string IgnoreTextLessOrEqualToSpecifiedLines { get; } = "指定した行数以下のテキストアイテムを無視";

        // 自動タイトル生成
        public override string AutoTitleGeneration { get; } = "自動タイトル生成";


        // OpenAIを使用して自動的にタイトルを生成する
        public override string AutomaticallyGenerateTitleUsingOpenAI { get; } = "OpenAIを使用して自動的にタイトルを生成する";

        // 自動でタグ生成する
        public override string AutomaticallyGenerateTags { get; } = "自動でタグ生成する";

        // クリップボードの内容から自動的にタグを生成します
        public override string AutomaticallyGenerateTagsFromContent { get; } = "クリップボードの内容から自動的にタグを生成します";

        // ファイルから自動でテキスト抽出
        public override string AutomaticallyExtractTextFromFile { get; } = "ファイルから自動でテキスト抽出";

        // アイテムがファイルの場合、自動でテキスト抽出を行います
        public override string AutomaticallyExtractTextFromFileIfApplicationItemIsFile { get; } = "アイテムがファイルの場合、自動でテキスト抽出を行います";

        // 画像から自動でテキスト抽出
        public override string AutomaticallyExtractTextFromImage { get; } = "画像から自動でテキスト抽出";

        // PyOCRを使用してテキスト抽出します
        public override string ExtractTextUsingPyOCR { get; } = "PyOCRを使用してテキスト抽出します";

        // OpenAIを使用してテキスト抽出します
        public override string ExtractTextUsingOpenAI { get; } = "OpenAIを使用してテキスト抽出します";

        // 画像からテキスト抽出時にEmbedding
        public override string EmbeddingWhenExtractingTextFromImage { get; } = "画像からテキスト抽出時にEmbedding";
        // 画像からテキスト抽出時にEmbeddingを行います
        public override string EmbeddingWhenExtractingTextFromImageDescription { get; } = "画像からテキスト抽出時にEmbeddingを行います";


        // 自動背景情報追加
        public override string AutomaticallyAddBackgroundInformation { get; } = "自動背景情報追加";


        // 自動サマリー生成
        public override string AutomaticallyGenerateSummary { get; } = "自動サマリー生成";

        // コンテンツからサマリーテキストを生成します。
        public override string GenerateSummaryTextFromContent { get; } = "コンテンツからサマリーテキストを生成します。";

        // 自動課題リスト生成
        public override string AutomaticallyGenerateTaskList { get; } = "自動課題リスト生成";

        // コンテンツから課題リストを生成します。
        public override string GenerateTaskListFromContent { get; } = "コンテンツから課題リストを生成します。";

        // コンテンツの文書信頼度をチェックします。
        public override string CheckDocumentReliabilityOfContent { get; } = "コンテンツの文書信頼度をチェックします。";

        // エンティティ抽出/データマスキング
        public override string EntityExtractionDataMasking { get; } = "エンティティ抽出/データマスキング";

        // クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います
        public override string ExtractEntitiesAndMaskDataUsingSpacyFromClipboardContent { get; } = "クリップボードの内容からSpacyを使用してエンティティ抽出、データマスキングを行います";

        // OpenAIに送信するデータ内の個人情報などをマスキングします。
        public override string MaskPersonalInformationInDataSentToOpenAI { get; } = "OpenAIに送信するデータ内の個人情報などをマスキングします。";

        // 新規自動処理ルール
        public override string NewAutoProcessRule { get; } = "新規自動処理ルール";

        // システム共通設定を保存
        public override string SaveSystemCommonSettings { get; } = "システム共通設定を保存";

        #endregion

        #region 設定画面　イベント
        // --- SettingUserControlViewModel
        // Pythonの設定チェック
        public override string PythonSettingCheck { get; } = "Pythonの設定チェック";

        // PythonVenvPathNotSet
        public override string PythonVenvPathNotSet { get; } = "Python仮想環境のパスが設定されていません";

        // PythonVenvPathSet
        public override string PythonVenvPathSet { get; } = "Python仮想環境のパスが設定されています";

        // PythonVenvNotCreated
        public override string PythonVenvNotCreated { get; } = "Python仮想環境が作成されていません";
        // PythonVenvCreated
        public override string PythonVenvCreated { get; } = "Python仮想環境が作成されています";

        // APIServerURLCheck
        public override string APIServerURLCheck { get; } = "APIサーバーURLのチェック";
        // APIServerURLNotSet
        public override string APIServerURLNotSet { get; } = "APIサーバーURLが設定されていません";

        // APIServerURLSet
        public override string APIServerURLSet { get; } = "APIサーバーURLが設定されています";

        // PythonDLLのパスが設定されていません
        public override string PythonDLLPathNotSet { get; } = "PythonDLLのパスが設定されていません";

        // PythonDLLのパスが設定されています
        public override string PythonDLLPathSet { get; } = "PythonDLLのパスが設定されています";

        // PythonDLLのファイルが存在します
        public override string PythonDLLFileExists { get; } = "PythonDLLのファイルが存在します";

        // Pythonスクリプトをテスト実行
        public override string TestRunPythonScript { get; } = "Pythonスクリプトをテスト実行";

        // OpenAIの設定チェック
        public override string OpenAISettingCheck { get; } = "OpenAIの設定チェック";

        // OpenAIのAPIキーが設定されていません
        public override string OpenAIKeyNotSet { get; } = "OpenAIのAPIキーが設定されていません";
        // OpenAIのAPIキーが設定されています
        public override string OpenAIKeySet { get; } = "OpenAIのAPIキーが設定されています";

        // OpenAIのCompletionModelが設定されていません
        public override string OpenAICompletionModelNotSet { get; } = "OpenAIのCompletionModelが設定されていません";

        // OpenAIのCompletionModelが設定されています
        public override string OpenAICompletionModelSet { get; } = "OpenAIのCompletionModelが設定されています";

        // OpenAIのEmbeddingModelが設定されていません
        public override string OpenAIEmbeddingModelNotSet { get; } = "OpenAIのEmbeddingModelが設定されていません";

        // OpenAIのEmbeddingModelが設定されています
        public override string OpenAIEmbeddingModelSet { get; } = "OpenAIのEmbeddingModelが設定されています";

        // Azure OpenAIの設定チェック
        public override string AzureOpenAISettingCheck { get; } = "Azure OpenAIの設定チェック";

        // Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック
        public override string AzureOpenAIEndpointNotSet { get; } = "Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック";

        // Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません
        public override string CannotSetBothAzureOpenAIEndpointAndBaseURL { get; } = "Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません";

        // OpenAIのテスト実行
        public override string TestRunOpenAI { get; } = "OpenAIのテスト実行";

        // Pythonの実行に失敗しました
        public override string FailedToRunPython { get; } = "Pythonの実行に失敗しました";

        // Pythonの実行が可能です
        public override string PythonRunIsPossible { get; } = "Pythonの実行が可能です";

        // OpenAIの実行に失敗しました
        public override string FailedToRunOpenAI { get; } = "OpenAIの実行に失敗しました";

        // OpenAIの実行が可能です。
        public override string OpenAIRunIsPossible { get; } = "OpenAIの実行が可能です。";


        #endregion


        #region ToolTip
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public override string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 開始：画面監視を開始します。停止：画面監視を停止します。
        public override string ToggleScreenWatchToolTop { get; } = "開始：画面監視を開始します。停止：画面監視を停止します。";

        #endregion

        #region イベント

        // クリップボード監視開始
        public override string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public override string StopClipboardWatch { get; } = "クリップボード監視停止";

        // クリップボード監視を開始しました
        public override string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public override string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";

        // 画面監視開始
        public override string StartScreenWatch { get; } = "画面監視開始";

        // 画面監視停止
        public override string StopScreenWatch { get; } = "画面監視停止";

        // 画面監視を開始しました
        public override string StartScreenWatchMessage { get; } = "画面監視を開始しました";

        // 画面監視を停止しました
        public override string StopScreenWatchMessage { get; } = "画面監視を停止しました";

        // 統合監視開始
        public override string StartIntegratedMonitorWatch { get; } = "統合監視開始";
        // 統合監視停止
        public override string StopIntegratedMonitorWatch { get; } = "統合監視停止";

        // 統合監視を開始しました
        public override string StartIntegratedMonitorMessage { get; } = "統合監視を開始しました";

        // 統合監視を停止しました
        public override string StopIntegratedMonitorMessage { get; } = "統合監視を停止しました";


        // 自動処理を実行中
        public override string AutoProcessing { get; } = "自動処理を実行中";
        // アイテムの追加処理が失敗しました。
        public override string AddItemFailed { get; } = "アイテムの追加処理が失敗しました。";

        
        // 自動文書信頼度チェック処理を実行します
        public override string AutoCheckDocumentReliability { get; } = "自動文書信頼度チェック処理を実行します";


        #endregion


        #region FolderView関連
        // 参照先ベクトルDBに保存用ベクトルDBも追加する
        public override string AddVectorDBForSaveToReferenceVectorDB { get; } = "参照先ベクトルDBに保存用ベクトルDBも追加する";

        // ここにフォルダの説明を入力
        public override string InputDescriptionOfThisFolder { get; } = "ここにフォルダの説明を入力";

        #endregion

        #region  MainWindow

        
        // タグ編集
        public override string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public override string EditAutoProcessRule { get; } = "自動処理ルール編集";

        // プロンプトテンプレート編集
        public override string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public override string EditGitRagSource { get; } = "RAGソース(git)編集";
        // AutoGen定義編集
        public override string EditAutoGenDefinition { get; } = "AutoGen定義編集";


        // -- 表示メニュー
        // テキストを右端で折り返す
        public override string TextWrapping { get; } = "テキストを右端で折り返す";
        // サイズの大きなテキストの場合は自動的に折り返しを解除する
        public override string AutoTextWrapping { get; } = "サイズの大きなテキストの場合は自動的に折り返しを解除する";

        // ツール
        public override string Tool { get; } = "ツール";
        // OpenAIチャット
        public override string OpenAIChat { get; } = "OpenAIチャット";
        // イメージチャット
        public override string ImageChat { get; } = "イメージチャット";

        // イメージチャット 英語名
        public override string ImageChatEnglish { get; } = "ImageChat";



        // マージチャット
        public override string MergeChat { get; } = "マージチャット";

        // AutoGenChat
        public override string AutoGenChat { get; } = "AutoGenチャット";

        // Monitor
        public override string Monitor { get; } = "監視";


        // ローカルファイルシステム
        public override string FileSystem { get; } = "ファイルシステム";

        // ローカルファイルシステム 英語名
        public override string FileSystemEnglish { get; } = "FileSystem";

        // Shortcut
        public override string Shortcut { get; } = "ショートカット";

        // Shortcut 英語名
        public override string ShortcutEnglish { get; } = "Shortcut";

        // Outlook
        public override string Outlook { get; } = "Outlook";

        // Outlook 英語名
        public override string OutlookEnglish { get; } = "Outlook";

        // Edgeのブラウズ履歴
        public override string EdgeBrowseHistory { get; } = "Edgeの閲覧履歴";

        // Edgeのブラウズ履歴 英語名
        public override string EdgeBrowseHistoryEnglish { get; } = "EdgeBrowseHistory";

        // RecentFiles
        public override string RecentFiles { get; } = "最近のファイル";

        // ClipboardHistory
        public override string ClipboardHistory { get; } = "クリップボード履歴";

        // ScreenShotHistory
        public override string ScreenShotHistory { get; } = "スクリーンショット履歴";

        // IntegratedMonitorHistory
        public override string IntegratedMonitorHistory { get; } = "統合モニター履歴";

        // RecentFiles 英語名
        public override string RecentFilesEnglish { get; } = "RecentFiles";

        #endregion
    }
}

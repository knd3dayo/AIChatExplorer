namespace WpfAppCommon.Model {
    public partial class CommonStringResources {

        private static CommonStringResources? _instance;
        public static CommonStringResources Instance {
            get {
                if (_instance == null || _LangChanged) {
                    _LangChanged = false;
                    switch (Lang) {
                        case "ja-JP":
                            _instance = new CommonStringResources();
                            break;
                        default:
                            _instance = new CommonStringResourcesEn();
                            break;
                    }
                }
                return _instance;
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

        // -- SettingsUserControl.xaml --
        // -- 設定を反映させるためにアプリケーションの再起動を行ってください。
        public virtual string RestartAppToApplyChanges { get; } = "設定を反映させるためにアプリケーションの再起動を行ってください。";

        // 基本設定
        public virtual string BasicSettings { get; } = "基本設定";
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

        // その他
        public virtual string Other { get; } = "その他";

        // 開発中機能を有効にする
        public virtual string EnableDevelopmentFeatures { get; } = "開発中機能を有効にする";

        // 設定のチェック
        public virtual string CheckSettings { get; } = "設定のチェック";

        public virtual string AppName { get; } = "RAG Clipboard";
        // ファイル
        public virtual string File { get; } = "ファイル";

        // ファイル/画像
        public virtual string FileOrImage { get; } = "ファイル/画像";
        // 作成
        public virtual string Create { get; } = "作成";
        // アイテム作成
        public virtual string CreateItem { get; } = "アイテム作成";
        // 終了
        public virtual string Exit { get; } = "終了";
        // 編集
        public virtual string Edit { get; } = "編集";

        #region プロンプトメニュー

        public virtual string PromptMenu { get; } = "プロンプトメニュー";
        // タイトルを生成
        public virtual string GenerateTitle { get; } = "タイトルを生成";

        // 背景情報を生成
        public virtual string GenerateBackgroundInfo { get; } = "背景情報を生成";

        // サマリーを生成
        public virtual string GenerateSummary { get; } = "サマリーを生成";

        // ベクトル生成
        public virtual string GenerateVector { get; } = "ベクトル生成";

        #endregion

        //ベクトル検索
        public virtual string VectorSearch { get; } = "ベクトル検索";

        // 開始
        public virtual string Start { get; } = "開始";
        // 停止
        public virtual string Stop { get; } = "停止";
        // 選択
        public virtual string Select { get; } = "選択";
        // ヘルプ
        public virtual string Help { get; } = "ヘルプ";
        // バージョン情報
        public virtual string VersionInfo { get; } = "バージョン情報";

        // 表示
        public virtual string View { get; } = "表示";

        // クリップボード監視開始
        public virtual string StartClipboardWatch { get; } = "クリップボード監視開始";
        // クリップボード監視停止
        public virtual string StopClipboardWatch { get; } = "クリップボード監視停止";
        // Windows通知監視開始
        public virtual string StartNotificationWatch { get; } = "Windows通知監視開始";
        // Windows通知監視停止
        public virtual string StopNotificationWatch { get; } = "Windows通知監視停止";

        // クリップボード監視を開始しました
        public virtual string StartClipboardWatchMessage { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public virtual string StopClipboardWatchMessage { get; } = "クリップボード監視を停止しました";
        // Windows通知監視を開始しました
        public virtual string StartNotificationWatchMessage { get; } = "Windows通知監視を開始しました";
        // Windows通知監視を停止しました
        public virtual string StopNotificationWatchMessage { get; } = "Windows通知監視を停止しました";

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

        // -- 表示メニュー
        // テキストを右端で折り返す
        public virtual string TextWrapping { get; } = "テキストを右端で折り返す";
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

        // 検索
        public virtual string Search { get; } = "検索";
        // 設定
        public virtual string Setting { get; } = "設定";
        // 削除
        public virtual string Delete { get; } = "削除";
        // 追加
        public virtual string Add { get; } = "追加";
        // OK
        public virtual string OK { get; } = "OK";
        // キャンセル
        public virtual string Cancel { get; } = "キャンセル";
        // 閉じる
        public virtual string Close { get; } = "閉じる";

        // ExportImport
        public virtual string ExportImport { get; } = "エクスポート/インポート";

        // Export
        public virtual string Export { get; } = "エクスポート";
        // Import
        public virtual string Import { get; } = "インポート";


        // バックアップ/リストア
        public virtual string BackupRestore { get; } = "バックアップ/リストア";

        // アイテムのバックアップ
        public virtual string BackupItem { get; } = "アイテムのバックアップ";
        // アイテムのリストア
        public virtual string RestoreItem { get; } = "アイテムのリストア";

        // 自動処理ルール一覧
        public virtual string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public virtual string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public virtual string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public virtual string NewTag { get; } = "新規タグ";
        // タグ
        public virtual string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public virtual string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public virtual string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public virtual string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.
        public virtual string ToggleNotificationWatchToolTop { get; } = "開始：Windows通知監視を開始します。停止：Windows通知監視を停止します.";

        // 選択中のフォルダにアイテムを作成します。
        public virtual string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public virtual string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public virtual string EditTagToolTip { get; } = "タグを編集します。";

        // 選択したタグを削除します。
        public virtual string DeleteSelectedTag { get; } = "選択したタグを削除";
        // すべて選択します。
        public virtual string SelectAll { get; } = "すべて選択";
        // すべて選択解除します。
        public virtual string UnselectAll { get; } = "すべて選択解除";

        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public virtual string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public virtual string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // Pythonスクリプト一覧
        public virtual string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }
        // Pythonスクリプト編集
        public virtual string EditPythonScriptWindowTitle {
            get {
                return $"{AppName} - {EditPythonScript}";
            }
        }
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

        // RAGソース(git)編集
        public virtual string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // RAGソース一覧
        public virtual string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - RAGソース(git)一覧";
            }
        }
        // ベクトルDB一覧
        public virtual string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // ベクトルDB編集
        public virtual string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // コミット選択
        public virtual string SelectCommitWindowTitle {
            get {
                return $"{AppName} - コミット選択";
            }
        }
        // QAチャット
        public virtual string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public virtual string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

        // ログ表示
        public virtual string LogWindowTitle {
            get {
                return $"{AppName} - ログ表示";
            }
        }
        // スクリーンショットチェック用プロンプト生成
        public virtual string ScreenShotCheckPromptWindowTitle {
            get {
                return $"{AppName} - スクリーンショットチェック用プロンプト生成";
            }
        }

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

        // 自動処理時の設定
        public virtual string AutoProcessSetting { get; } = "自動処理時の設定";

        // Chatタイプ
        public virtual string ChatType { get; } = "Chatタイプ";
        // 出力形式
        public virtual string OutputType { get; } = "出力形式";
        // 文字列
        public virtual string StringType { get; } = "文字列";
        // リスト
        public virtual string ListType { get; } = "リスト";
        // テーブル
        public virtual string TableType { get; } = "テーブル";
        // 出力先
        public virtual string OutputDestination { get; } = "出力先";
        // 新規タブ
        public virtual string NewTab { get; } = "新規タブ";
        // 本文を上書き
        public virtual string OverwriteContent { get; } = "本文を上書き";
        // タイトルを上書き
        public virtual string OverwriteTitle { get; } = "タイトルを上書き";





        // フォルダ選択
        public virtual string SelectFolder { get; } = "フォルダ選択";

        // ファイル選択
        public virtual string SelectFile { get; } = "ファイル選択";

        // -- EditItemWindow --
        // テキストをファイルとして開く
        public virtual string OpenTextAsFile { get; } = "テキストをファイルとして開く";

        // ファイルを開く
        public virtual string OpenFile { get; } = "ファイルを開く";

        // 新規ファイルとして開く
        public virtual string OpenAsNewFile { get; } = "新規ファイルとして開く";

        // フォルダを開く
        public virtual string OpenFolder { get; } = "フォルダを開く";

        // テキストを抽出
        public virtual string ExtractText { get; } = "テキストを抽出";

        // 画像を開く
        public virtual string OpenImage { get; } = "画像を開く";

        // 画像からテキストを抽出
        public virtual string ExtractTextFromImage { get; } = "画像からテキストを抽出";

        // データをマスキング
        public virtual string MaskData { get; } = "データをマスキング";

        // ここをクリックするとタグ編集画面が開きます
        public virtual string ClickHereToOpenTheTagEditScreen { get; } = "ここをクリックするとタグ編集画面が開きます";

        // テキスト
        public virtual string Text { get; } = "テキスト";


        // ファイルパス
        public virtual string FilePath { get; } = "ファイルパス";

        // フォルダ
        public virtual string Folder { get; } = "フォルダ";

        // ファイル名
        public virtual string FileName { get; } = "ファイル名";

        // フォルダ名とファイル名
        public virtual string FolderNameAndFileName { get; } = "フォルダ名とファイル名";

        // イメージ
        public virtual string Image { get; } = "イメージ";

        // -- EditPythonScriptWindow --
        // 内容
        public virtual string Content { get; } = "内容";

        // -- ListPythonScriptWindow --
        // 新規Pythonスクリプト
        public virtual string NewPythonScript { get; } = "新規Pythonスクリプト";

        // -- SearchWindow --
        // 検索対象フォルダ
        public virtual string SearchTargetFolder { get; } = "検索対象フォルダ";

        // 除外
        public virtual string Exclude { get; } = "除外";

        // コピー元アプリ名
        public virtual string CopySourceAppName { get; } = "コピー元アプリ名";

        // 開始日
        public virtual string StartDate { get; } = "開始日";

        // 終了日
        public virtual string EndDate { get; } = "終了日";

        // 適用対象配下のフォルダも対象にする
        public virtual string IncludeSubfolders { get; } = "適用対象配下のフォルダも対象にする";

        // クリア
        public virtual string Clear { get; } = "クリア";

        // -- TagSearchWindow
        // タグ検索
        public virtual string TagSearch { get; } = "タグ検索";

        // -- VectorSearchResultWindow
        // ベクトル検索結果
        public virtual string VectorSearchResult { get; } = "ベクトル検索結果";

        // -- ImageChatWindow
        // 設定項目
        public virtual string SettingItem { get; } = "設定項目";

        // 設定値
        public virtual string SettingValue { get; } = "設定値";

        // チェックタイプ
        public virtual string CheckType { get; } = "チェックタイプ";

        // 貼り付け
        public virtual string Paste { get; } = "貼り付け";

        // -- ImageCheck.MainWindow --
        // 画像ファイル選択
        public virtual string SelectImageFile { get; } = "画像ファイル選択";

        // 画像エビデンスチェック項目編集
        public virtual string EditImageEvidenceCheckItem { get; } = "画像エビデンスチェック項目編集";

        // 開く
        public virtual string Open { get; } = "開く";

        // ここに回答が表示されます
        public virtual string TheAnswerWillBeDisplayedHere { get; } = "ここに回答が表示されます";

        // ここに質問を入力
        public virtual string EnterYourQuestionHere { get; } = "ここに質問を入力";

        // 保存
        public virtual string Save { get; } = "保存";

        // 送信
        public virtual string Send { get; } = "送信";

        // -- ListVectorDBWindow --
        // システム用のベクトルを表示
        public virtual string DisplayVectorsForTheSystem { get; } = "システム用のベクトルを表示";
        // システム用のプロンプトを表示
        public virtual string DisplayPromptsForTheSystem { get; } = "システム用のプロンプトを表示";

        // ベクトルDBの場所
        public virtual string VectorDBLocation { get; } = "ベクトルDBの場所";

        // ベクトルDBのタイプ
        public virtual string VectorDBType { get; } = "ベクトルDBのタイプ";

        // 新規ベクトルDB設定
        public virtual string NewVectorDBSetting { get; } = "新規ベクトルDB設定";

        // ベクトルDB設定編集
        public virtual string EditVectorDBSetting { get; } = "ベクトルDB設定編集";

        // -- QAChatControl --
        // 実験的機能1(文章解析+辞書生成+RAG)"
        public virtual string ExperimentalFunction1 { get; } = "実験的機能1(文章解析+辞書生成+RAG)";

        // ベクトルDB(フォルダ)
        public virtual string VectorDBFolder { get; } = "ベクトルDB(フォルダ)";

        // ここをクリックしてベクトルDB(フォルダ)を追加
        public virtual string ClickHereToAddVectorDBFolder { get; } = "ここをクリックしてベクトルDB(フォルダ)を追加";

        // ベクトルDB選択
        public virtual string SelectVectorDB { get; } = "ベクトルDB選択";

        // リストから除外
        public virtual string ExcludeFromList { get; } = "リストから除外";

        // ベクトルDB(外部)
        public virtual string VectorDB { get; } = "ベクトルDB";

        // ベクトルDB(外部)
        public virtual string VectorDBExternal { get; } = "ベクトルDB(外部)";

        // ここをクリックしてベクトルDBを追加
        public virtual string ClickHereToAddVectorDB { get; } = "ここをクリックしてベクトルDBを追加";

        // 画像アイテム
        public virtual string AdditionalItem { get; } = "追加アイテム";

        // ここをクリックして選択中のアイテムを貼り付け
        public virtual string ClickHereToPasteTheSelectedItem { get; } = "ここをクリックして選択中のアイテムを貼り付け";

        // 画像ファイル
        public virtual string ImageFile { get; } = "画像ファイル";

        // ここをクリックして画像ファイルを追加
        public virtual string ClickHereToAddImageFile { get; } = "ここをクリックして画像ファイルを追加";

        // チャット
        public virtual string Chat { get; } = "チャット";

        // プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。
        public virtual string PromptTemplate { get; } = "プロンプトテンプレート。ダブルクリックするとプロンプトテンプレート選択画面が開きます。";

        // プレビュー
        public virtual string Preview { get; } = "プレビュー";

        // プレビュー(JSON)
        public virtual string PreviewJSON { get; } = "プレビュー(JSON)";

        // Copy
        public virtual string Copy { get; } = "Copy";

        // --- ClipboardFolderViewModel ---
        // 自動処理が設定されています
        public virtual string AutoProcessingIsSet { get; } = "自動処理が設定されています";

        // 検索条件
        public virtual string SearchCondition { get; } = "検索条件";

        // フォルダを編集しました
        public virtual string FolderEdited { get; } = "フォルダを編集しました";

        // リロードしました
        public virtual string Reloaded { get; } = "リロードしました";

        // ファイルを選択してください
        public virtual string SelectFilePlease { get; } = "ファイルを選択してください";

        // フォルダを選択してください
        public virtual string SelectFolderPlease { get; } = "フォルダを選択してください";

        // フォルダをエクスポートしました
        public virtual string FolderExported { get; } = "フォルダをエクスポートしました";

        // フォルダをインポートしました
        public virtual string FolderImported { get; } = "フォルダをインポートしました";

        // ルートフォルダは削除できません
        public virtual string RootFolderCannotBeDeleted { get; } = "ルートフォルダは削除できません";

        // 確認
        public virtual string Confirm { get; } = "確認";

        // "フォルダを削除しますか？"
        public virtual string ConfirmDeleteFolder { get; } = "フォルダを削除しますか？";

        // "フォルダを削除しました"
        public virtual string FolderDeleted { get; } = "フォルダを削除しました";

        // "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?"
        public virtual string ConfirmDeleteItems { get; } = "ピン留めされたアイテム以外の表示中のアイテムを削除しますか?";

        // アイテムを削除しました
        public virtual string DeletedItems { get; } = "アイテムを削除しました";

        // "追加しました"
        public virtual string Added { get; } = "追加しました";

        // "編集しました"
        public virtual string Edited { get; } = "編集しました";

        // 貼り付けました
        public virtual string Pasted { get; } = "貼り付けました";

        // "マージするアイテムを2つ選択してください"
        public virtual string SelectTwoItemsToMerge { get; } = "マージするアイテムを2つ選択してください";

        // マージ先のアイテムが選択されていません
        public virtual string MergeTargetNotSelected { get; } = "マージ先のアイテムが選択されていません";

        // マージ元のアイテムが選択されていません
        public virtual string MergeSourceNotSelected { get; } = "マージ元のアイテムが選択されていません";

        // "マージしました"
        public virtual string Merged { get; } = "マージしました";

        // エラーが発生しました。\nメッセージ
        public virtual string ErrorOccurredAndMessage { get; } = "エラーが発生しました。\nメッセージ";

        // スタックトレース
        public virtual string StackTrace { get; } = "スタックトレース";

        // --- ClipboardItemViewModel ---
        // ファイル以外のコンテンツはフォルダを開けません
        public virtual string CannotOpenFolderForNonFileContent { get; } = "ファイル以外のコンテンツはフォルダを開けません";

        // ファイル以外のコンテンツはテキストを抽出できません
        public virtual string CannotExtractTextForNonFileContent { get; } = "ファイル以外のコンテンツはテキストを抽出できません";

        // "MainWindowViewModelがNullです"
        public virtual string MainWindowViewModelIsNull { get; } = "MainWindowViewModelがNullです";

        // タイトルを生成します
        public virtual string GenerateTitleInformation { get; } = "タイトルを生成します";
        // "タイトルを生成しました"
        public virtual string GeneratedTitleInformation { get; } = "タイトルを生成しました";



        // 背景情報
        public virtual string BackgroundInformation { get; } = "背景情報";

        // 背景情報を生成します
        public virtual string GenerateBackgroundInformation { get; } = "背景情報を生成します";

        // "背景情報を生成しました"
        public virtual string GeneratedBackgroundInformation { get; } = "背景情報を生成しました";

        // "サマリーを生成します"
        public virtual string GenerateSummary2 { get; } = "サマリーを生成します";

        // "サマリーを生成しました"
        public virtual string GeneratedSummary { get; } = "サマリーを生成しました";

        // 課題リスト
        public virtual string TasksList { get; } = "課題リスト";

        // "課題リストを生成します"
        public virtual string GenerateTasks { get; } = "課題リストを生成します";

        // "課題リストを生成しました"
        public virtual string GeneratedTasks { get; } = "課題リストを生成しました";

        // その他のプロンプト
        public virtual string OtherPrompts { get; } = "その他のプロンプト";

        // ベクトルを生成します
        public virtual string GenerateVector2 { get; } = "ベクトルを生成します";

        // "ベクトルを生成しました"
        public virtual string GeneratedVector { get; } = "ベクトルを生成しました";

        // 画像以外のコンテンツはテキストを抽出できません
        public virtual string CannotExtractTextForNonImageContent { get; } = "画像以外のコンテンツはテキストを抽出できません";

        // "数値を入力してください。"
        public virtual string EnterANumber { get; } = "数値を入力してください。";

        // フォルダが選択されていません。
        public virtual string FolderNotSelected { get; } = "フォルダが選択されていません。";

        // ルール名を入力してください。
        public virtual string EnterRuleName { get; } = "ルール名を入力してください。";

        // "アクションを選択してください。"
        public virtual string SelectAction { get; } = "アクションを選択してください。";

        // "編集対象のルールが見つかりません。"
        public virtual string RuleNotFound { get; } = "編集対象のルールが見つかりません。";

        // コピーまたは移動先のフォルダを選択してください。
        public virtual string SelectCopyOrMoveTargetFolder { get; } = "コピーまたは移動先のフォルダを選択してください。";

        // "同じフォルダにはコピーまたは移動できません。"
        public virtual string CannotCopyOrMoveToTheSameFolder { get; } = "同じフォルダにはコピーまたは移動できません。";

        // "コピー/移動処理の無限ループを検出しました。"
        public virtual string DetectedAnInfiniteLoopInCopyMoveProcessing { get; } = "コピー/移動処理の無限ループを検出しました。";

        // "PromptTemplateを選択してください。"
        public virtual string SelectPromptTemplate { get; } = "PromptTemplateを選択してください。";

        // PythonScriptを選択してください。
        public virtual string SelectPythonScript { get; } = "PythonScriptを選択してください。";

        
        // 標準フォルダ以外にはコピーまたは移動できません。
        public virtual string CannotCopyOrMoveToNonStandardFolders { get; } = "標準フォルダ以外にはコピーまたは移動できません。";

        // RootFolderViewModelがNullです。
        public virtual string RootFolderViewModelIsNull { get; } = "RootFolderViewModelがNullです。";


        // --- EditPythonScriptWindowViewModel ---
        // 説明を入力してください
        public virtual string EnterDescription { get; } = "説明を入力してください";

        // --- FolderEditWindowViewModel ---
        // フォルダが指定されていません
        public virtual string FolderNotSpecified { get; } = "フォルダが指定されていません";

        // フォルダ名を入力してください
        public virtual string EnterFolderName { get; } = "フォルダ名を入力してください";

        // --- FolderSelectWindowViewModel ---
        // "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない
        public virtual string FolderSelectWindowViewModelInstanceNotFound { get; } = "エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない";

        // エラーが発生しました。選択中のフォルダがない
        public virtual string SelectedFolderNotFound { get; } = "エラーが発生しました。選択中のフォルダがない";

        // --- ListAutoProcessRuleWindowViewModel ---
        // 自動処理ルールが選択されていません。
        public virtual string AutoProcessRuleNotSelected { get; } = "自動処理ルールが選択されていません。";

        // を削除しますか？
        public virtual string ConfirmDelete { get; } = "を削除しますか？";

        // "システム共通設定を保存しました。"
        public virtual string SavedSystemCommonSettings { get; } = "システム共通設定を保存しました。";

        // "システム共通設定の変更はありません。"
        public virtual string NoChangesToSystemCommonSettings { get; } = "システム共通設定の変更はありません。";

        // --- ListPythonScriptWindowViewModel ---
        // 実行
        public virtual string Execute { get; } = "実行";

        // スクリプトを選択してください
        public virtual string SelectScript { get; } = "スクリプトを選択してください";

        // --- SearchWindowViewModel ---
        // 検索フォルダ
        public virtual string SearchFolder { get; } = "検索フォルダ";

        // 標準
        public virtual string Standard { get; } = "標準";

        // SearchConditionRuleがNullです
        public virtual string SearchConditionRuleIsNull { get; } = "SearchConditionRuleがNullです";

        // 検索条件がありません
        public virtual string NoSearchConditions { get; } = "検索条件がありません";

        // --- TagSearchWindowViewModel ---
        // "タグが空です"
        public virtual string TagIsEmpty { get; } = "タグが空です";

        // "タグが既に存在します"
        public virtual string TagAlreadyExists { get; } = "タグが既に存在します";

        // バージョン情報
        public virtual string VersionInformation { get; } = "バージョン情報";

        // -- ClipboardApp.MainWindowViewModel --
        // "アプリケーションを再起動すると、表示モードが変更されます。"
        public virtual string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "アプリケーションを再起動すると、表示モードが変更されます。";
        // 情報
        public virtual string Information { get; } = "情報";

        // "終了しますか?"
        public virtual string ConfirmExit { get; } = "終了しますか?";

        // 選択中のアイテムがない"
        public virtual string NoItemSelected { get; } = "選択中のアイテムがない";

        // 切り取りました"
        public virtual string Cut { get; } = "切り取りました";

        // コピーしました"
        public virtual string Copied { get; } = "コピーしました";

        // 貼り付け先のフォルダがない
        public virtual string NoPasteFolder { get; } = "貼り付け先のフォルダがない";

        // "コピー元のフォルダがない"
        public virtual string NoCopyFolder { get; } = "コピー元のフォルダがない";

        // "選択中のアイテムを削除しますか?"
        public virtual string ConfirmDeleteSelectedItems { get; } = "選択中のアイテムを削除しますか?";

        // "削除しました"
        public virtual string Deleted { get; } = "削除しました";

        // --- ImageCHat ---
        // 画像を確認して以下の各文が正しいか否かを教えてください\n\n
        public virtual string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "画像を確認して以下の各文が正しいか否かを教えてください\n\n";

        // 画像ファイルが選択されていません。
        public virtual string NoImageFileSelected { get; } = "画像ファイルが選択されていません。";

        // プロンプトを送信します
        public virtual string SendPrompt { get; } = "プロンプトを送信します";
        // 画像ファイル名
        public virtual string ImageFileName { get; } = "画像ファイル名";

        // エラーが発生しました。
        public virtual string ErrorOccurred { get; } = "エラーが発生しました。";

        // 画像ファイルを選択してください
        public virtual string SelectImageFilePlease { get; } = "画像ファイルを選択してください";

        // すべてのファイル
        public virtual string AllFiles { get; } = "すべてのファイル";

        // "ファイルが存在しません。"
        public virtual string FileDoesNotExist { get; } = "ファイルが存在しません。";

        // -- EditPromptItemWindowViewModel --
        // プロンプト編集
        public virtual string EditPrompt { get; } = "プロンプト編集";

        // -- ListPromptTemplateWindow -- 
        // 新規プロンプトテンプレート
        public virtual string NewPromptTemplate { get; } = "新規プロンプトテンプレート";

        // RAG
        public virtual string RAG { get; } = "RAG";

        // -- DevFeatures.cs
        // テキスト以外のコンテンツはマスキングできません
        public virtual string CannotMaskNonTextContent { get; } = "テキスト以外のコンテンツはマスキングできません";

        // データをマスキングしました
        public virtual string MaskedData { get; } = "データをマスキングしました";

        // マスキングデータをもとに戻します
        public virtual string RestoreMaskingData { get; } = "マスキングデータをもとに戻します";

        // 画像が取得できません
        public virtual string CannotGetImage { get; } = "画像が取得できません";


        // -- ScriptAutoProcessItem.cs --
        // Pythonスクリプトを実行しました
        public virtual string ExecutedPythonScript { get; } = "Pythonスクリプトを実行しました";

        // -- SystemAutoProcessItem.cs --
        // AutoProcessItemが見つかりません
        public virtual string AutoProcessItemNotFound { get; } = "AutoProcessItemが見つかりません";

        // -- EnumDescription.cs --

        // Enum型ではありません
        public virtual string NotEnumType { get; } = "Enum型ではありません";

        // --- WindowsNotificationController.cs ---
        // アクセス拒否
        public virtual string AccessDenied { get; } = "アクセス拒否";

        // --- EditPromptItemWindowViewModel ---
        // 名前を入力してください
        public virtual string EnterName { get; } = "名前を入力してください";

        // --- EditRAGSourceWindowViewModel ---
        // RAGソース編集
        public virtual string EditRAGSource { get; } = "RAGソース編集";

        // ItemViewModelがnullです"
        public virtual string ItemViewModelIsNull { get; } = "ItemViewModelがnullです";

        // EditVectorDBWindowViewModel
        // Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません
        public virtual string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません";

        // プロンプトテンプレート一覧
        public virtual string PromptTemplateList { get; } = "プロンプトテンプレート一覧";

        // プロンプトテンプレートが選択されていません
        public virtual string NoPromptTemplateSelected { get; } = "プロンプトテンプレートが選択されていません";

        // ListVectorDBWindowViewModel
        // 編集するベクトルDBを選択してください
        public virtual string SelectVectorDBToEdit { get; } = "編集するベクトルDBを選択してください";

        // 削除するベクトルDBを選択してください
        public virtual string SelectVectorDBToDelete { get; } = "削除するベクトルDBを選択してください";

        // 選択中のベクトルDBを削除しますか？
        public virtual string ConfirmDeleteSelectedVectorDB { get; } = "選択中のベクトルDBを削除しますか？";

        // ベクトルDBを選択してください
        public virtual string SelectVectorDBPlease { get; } = "ベクトルDBを選択してください";

        // RAGManagementWindowViewModel
        // 編集するRAG Sourceを選択してください
        public virtual string SelectRAGSourceToEdit { get; } = "編集するRAG Sourceを選択してください";

        // 削除するRAG Sourceを選択してください
        public virtual string SelectRAGSourceToDelete { get; } = "削除するRAG Sourceを選択してください";

        // 選択中のRAG Sourceを削除しますか？
        public virtual string ConfirmDeleteSelectedRAGSource { get; } = "選択中のRAG Sourceを削除しますか？";

        // コミットを選択してください
        public virtual string SelectCommitPlease { get; } = "コミットを選択してください";

        // --- UpdateRAGIndexWindowViewModel
        // インデックス化対象ファイル
        public virtual string IndexingTargetFile { get; } = "インデックス化対象ファイル";

        // ファイル追加
        public virtual string AddFile { get; } = "ファイル追加";
        // ファイル削除
        public virtual string DeleteFile { get; } = "ファイル削除";

        // ファイル更新
        public virtual string UpdateFile { get; } = "ファイル更新";

        // 対象ファイル取得
        public virtual string GetTargetFile { get; } = "対象ファイル取得";

        // インデックス作成
        public virtual string CreateIndex { get; } = "インデックス作成";

        //  "戻る"
        public virtual string Back { get; } = "戻る";

        // RAGSourceItemViewModelが設定されていません
        public virtual string RAGSourceItemViewModelNotSet { get; } = "RAGSourceItemViewModelが設定されていません";

        // "開始コミットを指定してください"
        public virtual string SpecifyStartCommit { get; } = "開始コミットを指定してください";

        // "対象を選択してください"
        public virtual string SelectTarget { get; } = "対象を選択してください";

        // 処理ファイル数
        public virtual string ProcessedFileCount { get; } = "処理ファイル数";

        // インデックス作成中
        public virtual string CreatingIndex { get; } = "インデックス作成中";

        // 完了
        public virtual string Completed { get; } = "完了";

        // 未対応ファイルタイプのためスキップ
        public virtual string SkipUnsupportedFileType { get; } = "未対応ファイルタイプのためスキップ";

        // 失敗
        public virtual string Failed { get; } = "失敗";

        // "インデックス作成が完了しました"
        public virtual string IndexCreationCompleted { get; } = "インデックス作成が完了しました";

        // インデックス作成処理を中断しました
        public virtual string IndexCreationInterrupted { get; } = "インデックス作成処理を中断しました";

        // チャットの送信に失敗しました。
        public virtual string FailedToSendChat { get; } = "チャットの送信に失敗しました。";


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
        // 実行しますか？
        public virtual string ConfirmRun { get; } = "実行しますか？";

        // 設定チェック中
        public virtual string CheckingSettings { get; } = "設定チェック中";

        // 設定を保存しました。
        public virtual string SettingsSaved { get; } = "設定を保存しました。";

        // "キャンセルしました"
        public virtual string Canceled { get; } = "キャンセルしました";

        // MyStatusBarViewModel
        // ログ
        public virtual string Log { get; } = "ログ";

        // 統計
        public virtual string Statistics { get; } = "統計";

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
        // --- ClipboardAppVectorDBItem

        // --- ClipboardFolder.cs ---
        // クリップボード
        public virtual string Clipboard { get; } = "クリップボード";

        // チャット履歴
        public virtual string ChatHistory { get; } = "チャット履歴";

        // 自動処理でアイテムが削除または移動されました
        public virtual string ItemsDeletedOrMovedByAutoProcessing { get; } = "自動処理でアイテムが削除または移動されました";

        // "アイテムを追加しました"
        public virtual string AddedItems { get; } = "アイテムを追加しました";

        // 自動処理を適用します
        public virtual string ApplyAutoProcessing { get; } = "自動処理を適用します";

        // 自動処理でアイテムが削除されました
        public virtual string ItemsDeletedByAutoProcessing { get; } = "自動処理でアイテムが削除されました";

        // JSON文字列をパースできませんでした
        public virtual string FailedToParseJSONString { get; } = "JSON文字列をパースできませんでした";

        // ClipboardItem
        // Text以外のアイテムへのマージはできません
        public virtual string CannotMergeToNonTextItems { get; } = "Text以外のアイテムへのマージはできません";

        // "Text以外のアイテムが含まれているアイテムはマージできません"
        public virtual string CannotMergeItemsContainingNonTextItems { get; } = "Text以外のアイテムが含まれているアイテムはマージできません";

        // 作成日時
        public virtual string CreationDateTime { get; } = "作成日時";

        // ソースアプリ名
        public virtual string SourceAppName { get; } = "ソースアプリ名";

        // ピン留めしてます
        public virtual string Pinned { get; } = "ピン留めしてます";

        // フォルダを取得できません
        public virtual string CannotGetFolder { get; } = "フォルダを取得できません";

        // OS上のファイルに保存します
        public virtual string SaveToFileOnOS { get; } = "OS上のファイルに保存します";

        // Gitコミットしました
        public virtual string CommittedToGit { get; } = "Gitコミットしました";

        // リポジトリが見つかりませんでした
        public virtual string RepositoryNotFound { get; } = "リポジトリが見つかりませんでした";

        // コミットが空です
        public virtual string CommitIsEmpty { get; } = "コミットが空です";

        // OS上のファイルに保存しました
        public virtual string SavedToFileOnOS { get; } = "OS上のファイルに保存しました";


        // サマリー
        public virtual string Summary { get; } = "サマリー";

        

        // "OS上のファイルを削除します"
        public virtual string DeleteFileOnOS { get; } = "OS上のファイルを削除します";
        // OS上のファイルを削除しました
        public virtual string DeletedFileOnOS { get; } = "OS上のファイルを削除しました";



        // --- SystemAutoProcessItem.cs ---
        // 無視
        public virtual string Ignore { get; } = "無視";
        // "何もしません"
        public virtual string DoNothing { get; } = "何もしません";

        // フォルダにコピー
        public virtual string CopyToFolder { get; } = "フォルダにコピー";
        // クリップボードの内容を指定されたフォルダにコピーします
        public virtual string CopyClipboardContentToSpecifiedFolder { get; } = "クリップボードの内容を指定されたフォルダにコピーします";

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

        // --- TextSelector.cs ---
        // ファイルを実行できませんでした
        public virtual string FailedToRunFile { get; } = "ファイルを実行できませんでした";

        // テキストファイルとして開きます。
        public virtual string OpenAsTextFile { get; } = "テキストファイルとして開きます。";

        // --- EditChatItemWindow ---
        // チャットアイテム
        public virtual string ChatItem { get; } = "チャットアイテム";

        // --- ExportImportWindow ---
        // 以下の項目をエクスポートします
        public virtual string ExportTheFollowingItems { get; } = "以下の項目をエクスポートします";

        // 作業ディレクトリ
        public virtual string WorkingDirectory { get; } = "作業ディレクトリ";
        // リポジトリURL
        public virtual string RepositoryURL { get; } = "リポジトリURL";
        // 最後にインデックス化したコミット
        public virtual string LastIndexedCommit { get; } = "最後にインデックス化したコミット";
        // 新規RAGソース
        public virtual string NewRAGSource { get; } = "新規RAGソース";

        // インポート時に自動処理を実行します
        public virtual string ExecuteAutoProcessingOnImport { get; } = "インポート時に自動処理を実行します";


        // 例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。
        public virtual string ExampleGeneralVectorDB { get; } = "例：ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";


        // ドキュメントのチャンクサイズ
        public virtual string DocumentChunkSize { get; } = "ドキュメントのチャンクサイズ";

        // ベクトル検索結果の上限値
        public virtual string VectorSearchResultLimit { get; } = "ベクトル検索結果の上限値";

        // MultiVectorRetrieverを使用
        public virtual string UseMultiVectorRetriever { get; } = "MultiVectorRetrieverを使用";

        // DocStore用のSQLite3の場所
        public virtual string SQLite3LocationForDocStore { get; } = "DocStore用のSQLite3の場所";

        // 例：sqlite:///C:\Users\Username\sqlite3.db
        public virtual string ExampleSQLite3Location { get; } = "例：sqlite:///C:\\Users\\Username\\sqlite3.db";

        // マルチベクターリトリーバのドキュメントのチャンクサイズ
        public virtual string DocumentChunkSizeForMultiVectorRetriever { get; } = "マルチベクターリトリーバのドキュメントのチャンクサイズ";

        // 例：C:\Users\Username\vector.db
        public virtual string ExampleVectorDBLocationChroma { get; } = "例：C:\\Users\\Username\\vector.db";

        // 例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain
        public virtual string ExampleVectorDBLocationPostgres { get; } = "例：postgresql+psycopg://langchain:langchain@localhost:5432/langchain";

        // チャット履歴をクリア
        public virtual string ClearChatHistory { get; } = "チャット履歴をクリア";

        // 本文をクリア
        public virtual string ClearContent { get; } = "本文をクリア";

        // 本文を再読み込み
        public virtual string ReloadContent { get; } = "本文を再読み込み";

        // 抽出したテキスト
        public virtual string ExtractedText { get; } = "抽出したテキスト";

        // タブ削除
        public virtual string DeleteTab { get; } = "タブ削除";

    }
}

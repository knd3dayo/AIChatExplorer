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

        public virtual string AppName { get; } = "コピペアプリ";
        // ファイル
        public virtual string File { get; } = "ファイル";
        // 作成
        public virtual string Create { get; } = "作成";
        // アイテム作成
        public virtual string CreateItem { get; } = "アイテム作成";
        // 終了
        public virtual string Exit { get; } = "終了";
        // 編集
        public virtual string Edit { get; } = "編集";

        // タイトルを生成
        public virtual string GenerateTitle { get; } = "タイトルを生成";

        // 背景情報を生成
        public virtual string GenerateBackgroundInfo { get; } = "背景情報を生成";

        // サマリーを生成
        public virtual string GenerateSummary { get; } = "サマリーを生成";

        // ベクトル生成
        public virtual string GenerateVector { get; } = "ベクトル生成";


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

    }
}

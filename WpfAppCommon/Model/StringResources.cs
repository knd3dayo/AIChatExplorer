namespace WpfAppCommon.Model {
    public class StringResources {

        public static StringResources Instance { get; } = new StringResources();
        public string AppName { get; } = "コピペアプリ";
        // ファイル
        public string File { get; } = "ファイル";
        // 作成
        public string Create { get; } = "作成";
        // アイテム作成
        public string CreateItem { get; } = "アイテム作成";
        // 終了
        public string Exit { get; } = "終了";
        // 編集
        public string Edit { get; } = "編集";
        // 開始
        public string Start { get; } = "開始";
        // 停止
        public string Stop { get; } = "停止";
        // 選択
        public string Select { get; } = "選択";

        // クリップボード監視を開始しました
        public string StartClipboardWatch { get; } = "クリップボード監視を開始しました";
        // クリップボード監視を停止しました
        public string StopClipboardWatch { get; } = "クリップボード監視を停止しました";

        // タグ編集
        public string EditTag { get; } = "タグ編集";
        // 自動処理ルール編集
        public string EditAutoProcessRule { get; } = "自動処理ルール編集";
        // Pythonスクリプト編集
        public string EditPythonScript { get; } = "Pythonスクリプト編集";
        // プロンプトテンプレート編集
        public string EditPromptTemplate { get; } = "プロンプトテンプレート編集";
        // RAGソース編集
        public string EditGitRagSource { get; } = "RAGソース(git)編集";
        // ツール
        public string Tool { get; } = "ツール";
        // OpenAIチャット
        public string OpenAIChat { get; } = "OpenAIチャット";
        // 画像エビデンスチェッカー
        public string ImageEvidenceChecker { get; } = "画像エビデンスチェッカー";

        // 検索
        public string Search { get; } = "検索";
        // 設定
        public string Setting { get; } = "設定";
        // 削除
        public string Delete { get; } = "削除";
        // 追加
        public string Add { get; } = "追加";
        // OK
        public string OK { get; } = "OK";
        // キャンセル
        public string Cancel { get; } = "キャンセル";
        // 閉じる
        public string Close { get; } = "閉じる";
        // エクスポート
        public string Export { get; } = "エクスポート";
        // インポート
        public string Import { get; } = "インポート";
        // 自動処理ルール一覧
        public string ListAutoProcessRule { get; } = "自動処理ルール一覧";
        // Pythonスクリプト一覧
        public string ListPythonScript { get; } = "Pythonスクリプト一覧";

        // タグ一覧
        public string ListTag { get; } = "タグ一覧";

        // 新規タグ
        public string NewTag { get; } = "新規タグ";
        // タグ
        public string Tag { get; } = "タグ";

        // ベクトルDB一覧
        public string ListVectorDB { get; } = "ベクトルDB一覧";
        // ベクトルDB編集
        public string EditVectorDB { get; } = "ベクトルDB編集";

        // --- ToolTip ---
        // 開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。
        public string ToggleClipboardWatchToolTop { get; } = "開始：クリップボード監視を開始します。停止：クリップボード監視を停止します。";

        // 選択中のフォルダにアイテムを作成します。
        public string CreateItemToolTip { get; } = "選択中のフォルダにアイテムを作成します。";

        // アプリケーションを終了します。
        public string ExitToolTip { get; } = "アプリケーションを終了します。";
        // タグを編集します。
        public string EditTagToolTip { get; } = "タグを編集します。";

        // --- 画面タイトル ---

        // 自動処理ルール一覧
        public string ListAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {ListAutoProcessRule}";
            }
        }
        // 自動処理ルール編集
        public string EditAutoProcessRuleWindowTitle {
            get {
                return $"{AppName} - {EditAutoProcessRule}";
            }
        }
        // Pythonスクリプト一覧
        public string ListPythonScriptWindowTitle {
            get {
                return $"{AppName} - {ListPythonScript}";
            }
        }
        // Pythonスクリプト編集
        public string EditPythonScriptWindowTitle {
            get {
                return $"{AppName} - {EditPythonScript}";
            }
        }
        // 設定
        public string SettingWindowTitle {
            get {
                return $"{AppName} - {Setting}";
            }
        }
        // 設定チェック結果
        public string SettingCheckResultWindowTitle {
            get {
                return $"{AppName} - 設定チェック結果";
            }
        }

        // RAGソース(git)編集
        public string EditGitRagSourceWindowTitle {
            get {
                return $"{AppName} - {EditGitRagSource}";
            }
        }
        // RAGソース一覧
        public string ListGitRagSourceWindowTitle {
            get {
                return $"{AppName} - RAGソース(git)一覧";
            }
        }
        // ベクトルDB一覧
        public string ListVectorDBWindowTitle {
            get {
                return $"{AppName} - {ListVectorDB}";
            }
        }
        // ベクトルDB編集
        public string EditVectorDBWindowTitle {
            get {
                return $"{AppName} - {EditVectorDB}";
            }
        }
        // コミット選択
        public string SelectCommitWindowTitle {
            get {
                return $"{AppName} - コミット選択";
            }
        }
        // QAチャット
        public string QAChatWindowTitle {
            get {
                return $"{AppName} - {OpenAIChat}";
            }
        }

        // タグ一覧
        public string ListTagWindowTitle {
            get {
                return $"{AppName} - {ListTag}";
            }
        }

    }
}

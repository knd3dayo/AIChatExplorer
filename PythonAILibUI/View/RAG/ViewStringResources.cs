using QAChat.Resource;

namespace QAChat.View.RAG {
    public class ViewStringResources {
        public static CommonStringResources CommonStringResources { get; set; } = CommonStringResources.Instance;

        // Open
        public static string Open => CommonStringResources.Open;

        // Save
        public static string Save => CommonStringResources.Save;

        // Delete
        public static string Delete => CommonStringResources.Delete;

        // ChatItem
        public static string ChatItem => CommonStringResources.ChatItem;

        // Close
        public static string Close => CommonStringResources.Close;

        // Send
        public static string Send => CommonStringResources.Send;

        // Clear
        public static string Clear => CommonStringResources.Clear;

        // TheAnswerWillBeDisplayedHere
        public static string TheAnswerWillBeDisplayedHere => CommonStringResources.TheAnswerWillBeDisplayedHere;

        // PromptTemplate
        public static string PromptTemplate => CommonStringResources.PromptTemplate;

        // EnterYourQuestionHere
        public static string EnterYourQuestionHere => CommonStringResources.EnterYourQuestionHere;

        // ImageChat
        public static string ImageChat => CommonStringResources.ImageChat;

        // File
        public static string File => CommonStringResources.File;
        // Edit
        public static string Edit => CommonStringResources.Edit;

        // OK
        public static string OK => CommonStringResources.OK;

        // Cancel
        public static string Cancel => CommonStringResources.Cancel;
        // EditRAGSource
        public static string EditRAGSource => CommonStringResources.EditRAGSource;

        // 作業ディレクトリ
        public static string WorkingDirectory => CommonStringResources.WorkingDirectory;

        // GitリポジトリURL
        public static string GitRepositoryURL => "GitリポジトリURL";

        // 最後にインデックス化したコミット
        public static string LastIndexedCommit => "最後にインデックス化したコミット";

        // インデックス更新
        public static string UpdateIndex => "インデックス更新";

        // ベクトルDB
        public static string VectorDB => CommonStringResources.VectorDB;

        // ListGitRagSourceWindowTitle
        public static string ListGitRagSourceWindowTitle => CommonStringResources.ListGitRagSourceWindowTitle;

        // NewRAGSource
        public static string NewRAGSource => CommonStringResources.NewRAGSource;

        // SelectCommitWindowTitle
        public static string SelectCommitWindowTitle => CommonStringResources.SelectCommitWindowTitle;

        // コミット日時
        public static string CommitDateTime => "コミット日時";

        // コミットハッシュ
        public static string CommitHash => "コミットハッシュ";

        // メッセージ
        public static string Message => "メッセージ";

        // UpdateRAGIndexWindow
        public static string UpdateRAGIndexWindow => "UpdateRAGIndexWindow";

        // 最初のコミットから最新のコミットまでの全ファイルをインデックス化
        public static string IndexAllFilesFromFirstCommitToLatestCommit => "最初のコミットから最新のコミットまでの全ファイルをインデックス化";

        // 最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化
        public static string IndexFilesFromLastIndexedCommitToLatestCommit => "最後にインデックス化したコミットから最新のコミットまでのファイルをインデックス化";

        // 最後にインデックス化したコミットのハッシュ値
        public static string LastIndexedCommitHash => "最後にインデックス化したコミットのハッシュ値";

        // インデックス化するコミットの範囲を指定
        public static string SpecifyTheRangeOfCommitsToIndex => "インデックス化するコミットの範囲を指定";

        // インデックス化を開始するコミット
        public static string StartCommitForIndexing => "インデックス化を開始するコミット";

        // インデックス化処理の状況サマリー
        public static string IndexingSummary => "インデックス化処理の状況サマリー";

        // インデックス化処理の状況詳細
        public static string IndexingDetail => "インデックス化処理の状況詳細";


    }
}

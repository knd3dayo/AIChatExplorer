namespace QAChat.Resource {
    public partial class CommonStringResources {

        #region EditItemWindow関連
        // 新規アイテム
        public virtual string NewItem { get; } = "新規アイテム";
        #endregion

        #region FolderView関連
        // 自フォルダを参照先ベクトルDBに追加
        public virtual string AddMyFolderToTargetVectorDB { get; } = "自フォルダを参照先ベクトルDBに追加";

        // こにフォルダの説明を入力
        public virtual string InputDescriptionOfThisFolder { get; } = "こにフォルダの説明を入力";

        #endregion

    }
}

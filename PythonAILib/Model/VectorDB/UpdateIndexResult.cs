namespace PythonAILib.Model.VectorDB
{
    // Index更新処理結果を格納するクラス
    public class UpdateIndexResult
    {
        public enum UpdateIndexResultEnum
        {
            Success,
            Failed_FileNotFound,
            Failed_InvalidFileType,
            Failed_Other
        }
        public UpdateIndexResultEnum Result { get; set; } = UpdateIndexResultEnum.Failed_Other;

        public string Message { get; set; } = "";

        public int TokenCount { get; set; } = 0;
    }
}

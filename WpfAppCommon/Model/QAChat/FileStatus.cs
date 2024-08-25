namespace WpfAppCommon.Model.QAChat {
    public class FileStatus {
        public string Path { get; set; } = "";
        public FileStatusEnum Status { get; set; } = FileStatusEnum.Unknown;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Resources;

namespace LibPythonAI.Data {
    // 自動処理の引数用のクラス
    public class AutoProcessItemEntity {


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public AutoProcessItem.TypeEnum TypeName { get; set; } = AutoProcessItem.TypeEnum.CopyToFolder;
        [Column("DESTINATION_FOLDER_ID")]
        public string? DestinationFolderId { get; set; }

        public ContentFolderEntity? DestinationFolder { get; set; }

    }
}

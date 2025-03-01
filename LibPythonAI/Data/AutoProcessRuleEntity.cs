using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using PythonAILib.Model.AutoProcess;

namespace LibPythonAI.Data {

    public class AutoProcessRuleEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        [NotMapped]        
        public List<AutoProcessRuleConditionEntity> Conditions { get; set; } = [];

        public AutoProcessItemEntity? RuleAction { get; set; }


        [Column("TARGET_FOLDER_ID")]
        public string? TargetFolderId { get; set; }

        public ContentFolderEntity? TargetFolder { get; set; }



        // 移動またはコピー先のフォルダ
        [Column("DESTINATION_FOLDER_ID")]
        public string? DestinationFolderId { get; set; }

        public ContentFolderEntity? DestinationFolder { get; set; }

    }
}



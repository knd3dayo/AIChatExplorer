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

        public string? AutoProcessItemId { get; set; }

        public string? TargetFolderId { get; set; }

        public string? DestinationFolderId { get; set; }


        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoProcessRuleEntity other = (AutoProcessRuleEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}



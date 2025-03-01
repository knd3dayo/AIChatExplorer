using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PythonAILib.Common;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Git;

namespace LibPythonAI.Data {
    public class RAGSourceItemEntity {


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // ベクトルを格納するためのVectorDBItemのId
        [Column("VECTOR_DB_ITEM_ID")]
        protected string? VectorDBItemId { get; set; }
        // VectorDBIte

        public VectorDBItemEntity? VectorDBItem { get; set; }

        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";

    }
}

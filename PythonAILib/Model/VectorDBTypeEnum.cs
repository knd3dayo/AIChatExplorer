using System.ComponentModel;

namespace PythonAILib.Model {
    /// <summary>
    /// VectorDBの種類。現在はFaiss,Chroma(インメモリ)のみ
    /// </summary>
    public enum VectorDBTypeEnum {
        [Description("Faiss")]
        Faiss = 0,
        [Description("Chroma")]
        Chroma = 1,
        [Description("PGVector")]
        PGVector = 2,
        [Description("Custom")]
        Custom = 3,

    }
}

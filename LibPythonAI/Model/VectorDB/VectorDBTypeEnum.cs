using System.ComponentModel;

namespace LibPythonAI.Model.VectorDB {
    /// <summary>
    /// VectorDBの種類。現在はChroma(インメモリ)のみ
    /// </summary>
    public enum VectorDBTypeEnum
    {
        [Description("Chroma")]
        Chroma = 1,
        [Description("PGVector")]
        PGVector = 2,
        [Description("Custom")]
        Custom = 3,

    }
}

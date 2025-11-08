using LibMain.Model.VectorDB;

namespace LibMain.PythonIF.Request {
    public class EmbeddingRequest {

        public const string VECTOR_DB_NAME_KEY = "vector_db_name";
        public const string SOURCE_ID_KEY = "source_id";
        public const string CONTENT_KEY = "content";

        // metadata keys
        public const string METADATA_KEY = "metadata";
        public const string FOLDER_ID_KEY = "folder_id";
        public const string SOURCE_TYPE_KEY = "source_type";
        public const string DESCRIPTION_KEY = "description";
        public const string SOURCE_PATH_KEY = "source_path";
        public const string TAGS_KEY = "tags";


        public EmbeddingRequest(string vectorDBName, VectorEmbeddingItem embedding) {
            VectorDBName = vectorDBName;
            Embedding = embedding;
        }

        public string VectorDBName { get; set; } = "";

        public VectorEmbeddingItem Embedding { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict[VECTOR_DB_NAME_KEY] = VectorDBName;
            // source_id
            dict[SOURCE_ID_KEY] = Embedding.SourceId;
            dict[CONTENT_KEY] = Embedding.Content;

            // metadata
            Dictionary<string, object> medatada = [];

            medatada[SOURCE_PATH_KEY] = Embedding.SourcePath;
            medatada[SOURCE_TYPE_KEY] = Embedding.SourceType.ToString();
            medatada[DESCRIPTION_KEY] = Embedding.Description;

            // tags
            if (Embedding.Tags != null && Embedding.Tags.Count > 0) {
                medatada[TAGS_KEY] = Embedding.Tags;
            }
            dict[METADATA_KEY] = medatada;

            return dict;
        }
    }
}

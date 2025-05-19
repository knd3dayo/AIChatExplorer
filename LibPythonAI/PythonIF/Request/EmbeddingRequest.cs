using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    public class EmbeddingRequest {

        public const string NAME_KEY = "name";
        public const string MODEL_KEY = "model";
        public const string FOLDER_ID_KEY = "folder_id";
        public const string FOLDER_PATH_KEY = "folder_path";
        public const string SOURCE_ID_KEY = "source_id";
        public const string SOURCE_TYPE_KEY = "source_type";
        public const string DESCRIPTION_KEY = "description";
        public const string CONTENT_KEY = "content";
        public const string SOURCE_PATH_KEY = "source_path";
        public const string DOC_ID_KEY = "doc_id";
        public const string SCORE_KEY = "score";
        public const string SUB_DOCS_KEY = "sub_docs";


        public EmbeddingRequest(string vectorDBName, string model, VectorEmbeddingItem embedding) {
            Name = vectorDBName;
            Model = model;
            Embedding = embedding;
        }

        public string Name { get; set; } = "";
        public string Model { get; set; } = "text-embedding-3-small";

        public VectorEmbeddingItem Embedding { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict[NAME_KEY] = Name;
            dict[MODEL_KEY] = Model;

            dict[DOC_ID_KEY] = Embedding.DocId;
            // folder_id
            if (Embedding.FolderId != null) {
                dict[FOLDER_ID_KEY] = Embedding.FolderId;
            }
            // folder_path
            if (Embedding.FolderPath != null) {
                dict[FOLDER_PATH_KEY] = Embedding.FolderPath;
            }
            // source_id
            dict[SOURCE_ID_KEY] = Embedding.SourceId;
            dict[SOURCE_PATH_KEY] = Embedding.SourcePath;
            dict[SOURCE_TYPE_KEY] = Embedding.SourceType.ToString();
            dict[DESCRIPTION_KEY] = Embedding.Description;
            dict[CONTENT_KEY] = Embedding.Content;

            return dict;
        }
    }
}

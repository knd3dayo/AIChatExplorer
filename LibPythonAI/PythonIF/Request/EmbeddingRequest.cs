using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    public class EmbeddingRequest {

        public EmbeddingRequest(string vectorDBName, string model, VectorEmbedding embedding) {
            Name = vectorDBName;
            Model = model;
            Embedding = embedding;
        }

        public string Name { get; set; } = "";
        // Model
        public string Model { get; set; } = "text-embedding-3-small";

        // Embedding
        public VectorEmbedding Embedding { get; set; }



        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["Name"] = Name;
            dict["Model"] = Model;
            dict["Embedding"] = Embedding.ToDict();

            return dict;
        }

        public static List<Dictionary<string, object>> ToDictList(List<VectorSearchRequest> requests) {
            List<Dictionary<string, object>> dicts = [];
            foreach (var request in requests) {
                dicts.Add(request.ToDict());
            }
            return dicts;
        }
    }
}

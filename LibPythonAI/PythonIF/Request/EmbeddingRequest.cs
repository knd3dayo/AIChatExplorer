using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    public class EmbeddingRequest {

        public EmbeddingRequest(string vectorDBName, string model, VectorEmbedding embedding) {
            Name = vectorDBName;
            Model = model;
            Embedding = embedding;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        // Model
        [JsonPropertyName("model")]
        public string Model { get; set; } = "text-embedding-3-small";

        // Embedding
        [JsonPropertyName("embedding")]
        public VectorEmbedding Embedding { get; set; }



        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["name"] = Name;
            dict["model"] = Model;
            foreach (var kvp in Embedding.ToDict()) {
                dict[kvp.Key] = kvp.Value;
            }

            return dict;
        }
    }
}

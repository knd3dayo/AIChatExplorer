using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class CatalogRequest {

        public CatalogRequest(string catalogDBURL, string vectorDBURL, string collectionName, string folderId) {
            CatalogDBURL = catalogDBURL;
            VectorDBURL = vectorDBURL;
            CollectionName = collectionName;
            FolderId = folderId;
        }

        public string CatalogDBURL { get; private set; } = "";
        public string VectorDBURL { get; private set; } = "";
        public string CollectionName { get; private set; } = "";
        public string FolderId { get; private set; } = "";

        public string Description { get; set; } = "";

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["catalog_db_url"] = CatalogDBURL;
            dict["vector_db_url"] = VectorDBURL;
            dict["collection_name"] = CollectionName;
            dict["folder_id"] = FolderId;
            if (Description != "")
                dict["description"] = Description;
            return dict;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Content {
    public  class ContentCollection {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();
        
        public string Name { get; set; } = "";

        // 参照用のベクトルDBのリストのプロパティ
        private List<VectorDBItem> _referenceVectorDBItems = [];
        public List<VectorDBItem> ReferenceVectorDBItems {
            get {
                return _referenceVectorDBItems;
            }
            set {
                _referenceVectorDBItems = value;
            }
        }

    }
}

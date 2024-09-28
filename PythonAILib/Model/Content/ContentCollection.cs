using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonAILib.Model.Content {
    public  class ContentCollection {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;
        
        public string Name { get; set; } = "";

    }
}

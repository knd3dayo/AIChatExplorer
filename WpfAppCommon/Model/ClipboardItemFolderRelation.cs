using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WpfAppCommon.Model
{
    internal class ClipboardItemFolderRelation
    {
        public ObjectId? Id { get; set; }
        public string ParentCollectionName { get; set; } = "";
        public string ChildCollectionName { get; set; } = "";

        public ClipboardItemFolderRelation(string parentCollectionName, string childCollectionName)
        {
            ParentCollectionName = parentCollectionName;
            ChildCollectionName = childCollectionName;
        }
    }
}

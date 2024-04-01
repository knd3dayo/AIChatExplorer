using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class ClipboardItemFolderRelation
    {
        public int Id { get; set; }
        public string ParentCollectionName { get; set; } = "";
        public string ChildCollectionName { get; set; } = "";

        public ClipboardItemFolderRelation(string parentCollectionName, string childCollectionName)
        {
            ParentCollectionName = parentCollectionName;
            ChildCollectionName = childCollectionName;
        }
    }
}

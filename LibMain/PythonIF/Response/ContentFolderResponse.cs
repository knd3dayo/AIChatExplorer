using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibMain.Data;
using LibMain.PythonIF.Request;

namespace LibMain.PythonIF.Response {
    public  class ContentFolderResponse : ContentFolderRequest{

        public ContentFolderResponse(ContentFolderEntity entity) : base(entity) {
            Entity = entity;
        }

        public static ContentFolderResponse FromDict(Dictionary<string, object> dict) {
            ContentFolderEntity entity = ContentFolderEntity.FromDict(dict);
            return new ContentFolderResponse(entity);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1.Utils {
    public class ClipboardItemAppClient : ApiClient {

        public void Post(ClipboardItem clipboardItem) {
            string path = "/api/clipboard_item";
            string url = PythonExecutor.GetApiUrl(path);
            if (string.IsNullOrEmpty(url)) {
                Tools.Error("APIサーバーのURLが設定されていません");
                return;
            }
            var requestJsonNode = new JsonObject();
            requestJsonNode["item"] = clipboardItem.ToJsonNode();
            requestJsonNode["action"] = "mask_data";

            var responseJsonNode = base.Post(url, requestJsonNode);
            if (responseJsonNode == null) {
                Tools.Error("APIサーバーとの通信に失敗しました");
                return;
            }
            if (responseJsonNode["error"] != null) {
                string? message = responseJsonNode["error"]?.GetValue<string>();
                if (message != null) {
                    Tools.Error(message);
                    return;
                }
            }
            clipboardItem.FromJsonNode(responseJsonNode);
            return;
        }

    }
}

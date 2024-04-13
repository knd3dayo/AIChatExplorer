using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1.Utils {
    public class ClipboardItemAppClient : ApiClient {

        public ClipboardItem Post(ClipboardItem clipboardItem) {
            string path = "/api/clipboard_item";
            string url = PythonExecutor.GetApiUrl(path);
            if (string.IsNullOrEmpty(url)) {
                throw new ThisApplicationException("APIサーバーのURLが設定されていません。設定画面でAPIサーバーのURLを設定してください。");
            }

            var responseJsonNode = base.Post(url, ClipboardItem.ToJson(clipboardItem));
            if (responseJsonNode == null) {
                throw new Exception("APIサーバーとの通信に失敗しました");
            }
            ClipboardItem? resultItem = ClipboardItem.FromJson(responseJsonNode);
            if (resultItem == null) {
                throw new Exception("APIサーバーからのレスポンスが不正です");
            }
            return resultItem;
        }

    }
}

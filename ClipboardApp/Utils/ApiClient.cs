using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ClipboardApp.Utils {
    public class ApiClient {


        public string Post(string url, JsonNode jsonNode) {
            // HttpClientを使ってPOSTリクエストを送信する
            HttpClient client = new HttpClient();
            var content = new StringContent(jsonNode.ToString(), Encoding.UTF8, "application/json");
            try {
                var response = client.PostAsync(url, content).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;

                return responseContent;
            } catch (Exception e) {
                throw new ClipboardAppException("APIサーバーとの通信に失敗しました", e);
            }
        }
    }
}

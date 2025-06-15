using LibPythonAI.PythonIF;

namespace LibPythonAI.Model.Tag {
    public class TagItem {

        // Id
        public string Id { get; set; } = "";

        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

        public async Task DeleteAsync() {
            await Task.Run(() => {
                PythonExecutor.PythonAIFunctions.DeleteTagItemsAsync(new List<TagItem> { this });
            });
        }

        public async Task SaveAsync() { // 修正: 非同期メソッドは async Task に変更
            await Task.Run(() => {
                PythonExecutor.PythonAIFunctions.UpdateTagItemsAsync(new List<TagItem> { this });
            });
        }

        public static async Task<List<TagItem>> GetTagItemsAsync() { // 修正: メソッド名を非同期に合わせて変更
            return await Task.Run(() => {
                return PythonExecutor.PythonAIFunctions.GetTagItemsAsync();
            });
        }

        // タグを検索
        public static async Task<List<TagItem>> FilterTagAsync(string tag, bool exclude) { // 修正: 非同期メソッドに変更
            var items = await GetTagItemsAsync(); // 修正: 非同期メソッド呼び出し
            return items.Where(x => x.Tag.Contains(tag) == !exclude).ToList();
        }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new();
            dict["id"] = Id;
            dict["tag"] = Tag;
            dict["is_pinned"] = IsPinned.ToString();
            return dict;
        }

        public static List<Dictionary<string, object>> ToDictList(List<TagItem> items) {
            List<Dictionary<string, object>> dictList = new();
            foreach (var item in items) {
                dictList.Add(item.ToDict());
            }
            return dictList;
        }

        public static TagItem FromDict(Dictionary<string, object> dict) {
            TagItem item = new() {
                Id = dict["id"]?.ToString() ?? "",
                Tag = dict["tag"]?.ToString() ?? "",
                IsPinned = bool.Parse(dict["is_pinned"]?.ToString() ?? "false")
            };
            return item;
        }

        public static List<TagItem> FromDictList(List<Dictionary<string, object>> dictList) {
            List<TagItem> items = new();
            foreach (var dict in dictList) {
                items.Add(FromDict(dict));
            }
            return items;
        }
    }

}

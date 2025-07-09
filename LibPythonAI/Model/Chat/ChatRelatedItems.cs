using LibPythonAI.Model.Content;

namespace LibPythonAI.Model.Chat {
    public class ChatRelatedItems {


        // 関連アイテム情報を設定
        public List<ContentItemWrapper> ContentItems { get; set; } = [];
        public List<ContentItemDataDefinition> DataDefinitions { get; set; } = [];

        //　初回のリクエスト時のみ関連アイテムを送信するかどうか
        public bool SendRelatedItemsOnlyFirstRequest { get; set; } = true;


        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "content_item_ids", ContentItems.Select(item => item.Id).ToList() },
                { "data_definitions", DataDefinitions.Select(def => def.ToDict()).ToList() },
                { "send_related_items_only_first_request", SendRelatedItemsOnlyFirstRequest }
            };
            return dict;
        }
        // FromDict
        public static ChatRelatedItems FromDict(Dictionary<string, object> dict) {
            var relatedItems = new ChatRelatedItems();
            if (dict.TryGetValue("content_item_ids", out var contentItemIdsObj) && contentItemIdsObj is List<object> contentItemIds) {
                foreach (var itemId in contentItemIds) {
                    if (itemId is string id) {
                        ContentItemWrapper? contentItem = ContentItemWrapper.GetItem<ContentItemWrapper>(id);
                        if (contentItem != null) {
                            relatedItems.ContentItems.Add(contentItem);
                        }
                    }
                }
                if (dict.TryGetValue("data_definitions", out var dataDefinitionsObj) && dataDefinitionsObj is List<object> dataDefinitions) {
                    foreach (var dataDefinition in dataDefinitions) {
                        if (dataDefinition is Dictionary<string, dynamic> defDict) {
                            var contentItemDataDefinition = ContentItemDataDefinition.FromDict(defDict);
                            relatedItems.DataDefinitions.Add(contentItemDataDefinition);
                        }
                    }
                }
                if (dict.TryGetValue("send_related_items_only_first_request", out var sendOnlyFirstRequestObj) && sendOnlyFirstRequestObj is bool sendOnlyFirstRequest) {
                    relatedItems.SendRelatedItemsOnlyFirstRequest = sendOnlyFirstRequest;
                }
                return relatedItems;
            }
            return new ChatRelatedItems();
        }

    }
}

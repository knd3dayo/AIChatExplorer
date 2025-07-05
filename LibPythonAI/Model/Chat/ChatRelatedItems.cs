using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;

namespace LibPythonAI.Model.Chat {
    public class ChatRelatedItems {


        // 関連アイテム情報を設定
        public List<ContentItemWrapper> ContentItems { get; set; } = [];
        public List<ContentItemDataDefinition> DataDefinitions { get; set; } = [];

        //　初回のリクエスト時のみ関連アイテムを送信するかどうか
        public bool SendRelatedItemsOnlyFirstRequest { get; set; } = true;


    }
}

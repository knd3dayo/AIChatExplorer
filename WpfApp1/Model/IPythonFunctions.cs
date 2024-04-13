using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.View.OpenAIView;

namespace WpfApp1.Model {
    public interface IPythonFunctions {

        public string ExtractText(string path);
        public string MaskData(string text);
        public string OpenAIChat(List<JSONChatItem> jSONChatItems);

        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public List<string> ExtractEntity(string text);




    }
}

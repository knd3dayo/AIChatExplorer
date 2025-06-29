using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUIPythonAI.Resource;
using MS.WindowsAPICodePack.Internal;
using SQLitePCL;
using WpfAppCommon.Model;

namespace LibUINormalChat.Common {
    public class NormalChatUtil {

        public static async Task<ChatResponse?> ExecuteChat(
            ChatRequest chatRequest,
            ChatRequestContext chatRequestContext,  
            List<ContentItemWrapper> items, List<ContentItemDataDefinition> dataDefinition, Action<string> afterUpdate) {

            chatRequest.ApplyReletedItems(items, dataDefinition);


            return await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequest, chatRequestContext, afterUpdate);

        }



    }
}

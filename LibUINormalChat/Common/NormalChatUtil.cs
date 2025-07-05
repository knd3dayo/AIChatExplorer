using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Python;

namespace LibUINormalChat.Common {
    public class NormalChatUtil {

        public static async Task<ChatResponse?> ExecuteChat(
            ChatRequest chatRequest,
            ChatRequestContext chatRequestContext,
            ChatRelatedItems relatedItems, Action<string> afterUpdate) {

            chatRequest.ApplyReletedItems(relatedItems);


            return await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequest, chatRequestContext, afterUpdate);

        }



    }
}

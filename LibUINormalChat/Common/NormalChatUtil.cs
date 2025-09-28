using LibMain.Model.Chat;
using LibMain.PythonIF.Request;
using LibMain.PythonIF.Response;
using LibMain.Utils.Python;

namespace LibUINormalChat.Common {
    public class NormalChatUtil {

        public static async Task<ChatResponse?> ExecuteChat(
            ChatRequest chatRequest,
            ChatRequestContext chatRequestContext,
            ChatRelatedItems relatedItems, Action<string> afterUpdate) {

            await chatRequest.ApplyReletedItems(relatedItems);

            return await ChatUtil.ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequest, chatRequestContext, afterUpdate);

        }



    }
}

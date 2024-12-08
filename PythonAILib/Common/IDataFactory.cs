using LiteDB;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Script;
using PythonAILib.Model.Statistics;
using PythonAILib.Model.Tag;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Common {
    public interface IDataFactory {

        // Database
        public LiteDatabase GetDatabase();

        //-- ContentItem
        public ILiteCollection<T> GetItemCollection<T>() where T : ContentItem;

        // ContentFolder
        public ILiteCollection<T> GetFolderCollection<T>() where T : ContentFolder;

        // ShortCutFolder
        public ILiteCollection<T> GetShortCutFolderCollection<T>() where T : ContentFolder;

        // Prompt
        public ILiteCollection<T> GetPromptCollection<T>() where T : PromptItem;

        //----  RAGSourceItem
        public ILiteCollection<T> GetRAGSourceCollection<T>() where T : RAGSourceItem;

        //--- -  VectorDBItem
        public ILiteCollection<T> GetVectorDBCollection<T>() where T : VectorDBItem;

        // -- TagItem
        public ILiteCollection<T> GetTagCollection<T>() where T : TagItem;

        // --- Python Script
        public ILiteCollection<T> GetScriptCollection<T>() where T : ScriptItem;

        // --- Statistics
        public ILiteCollection<T> GetStatisticsCollection<T>() where T : MainStatistics;

        // --- AutoGenTools
        public ILiteCollection<T> GetAutoGenToolCollection<T>() where T : AutoGenTool;

        // --- AutoGenAgents
        public ILiteCollection<T> GetAutoGenAgentCollection<T>() where T : AutoGenAgent;

        // --- AutoGentGroupChat
        public ILiteCollection<T> GetAutoGenGroupChatCollection<T>() where T : AutoGenGroupChat;
    }
}

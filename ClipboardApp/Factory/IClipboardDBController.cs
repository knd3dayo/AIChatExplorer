using ClipboardApp.Model;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using LiteDB;
using PythonAILib.Common;

namespace ClipboardApp.Factory
{
    public interface IClipboardDBController : IDataFactory {

        // -- SearchRule
        public ILiteCollection<SearchRule> GetSearchRuleCollection();


        // -- AutoProcessRule
        public ILiteCollection<AutoProcessRule> GetAutoProcessRuleCollection();

    }

}

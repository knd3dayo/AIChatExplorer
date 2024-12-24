using ClipboardApp.Model;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using LiteDB;
using PythonAILib.Common;

namespace ClipboardApp.Factory
{
    public interface IClipboardDBController : IDataFactory {


        // -- AutoProcessRule
        public ILiteCollection<AutoProcessRule> GetAutoProcessRuleCollection();

    }

}

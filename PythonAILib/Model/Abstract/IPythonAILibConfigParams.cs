using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonAILib.Model.Abstract {
    public  interface IPythonAILibConfigParams {

        public string GetLang();
        public string GetPythonDllPath();
        public string GetDBPath();

        public string GetSystemVectorDBPath();

        public string GetSystemDocDBPath();

        public string GetPathToVirtualEnv();

        public IDataFactory GetDataFactory();

        public OpenAIProperties GetOpenAIProperties();

        public Action<string> GetInfoAction();

        public Action<string> GetWarnAction();

        public Action<string> GetErrorAction();


    }
}

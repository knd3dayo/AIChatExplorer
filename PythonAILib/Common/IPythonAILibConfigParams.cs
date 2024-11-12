namespace PythonAILib.Common
{
    public interface IPythonAILibConfigParams {

        public string GetLang();
        public string GetPythonDllPath();
        public string GetDBPath();

        public string GetSystemVectorDBPath();

        public string GetSystemDocDBPath();

        public string GetPathToVirtualEnv();

        public string GetAppDataPath();

        public IDataFactory GetDataFactory();

        public OpenAIProperties GetOpenAIProperties();

        public Action<string> GetInfoAction();

        public Action<string> GetWarnAction();

        public Action<string> GetErrorAction();


    }
}

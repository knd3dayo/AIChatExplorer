namespace LibUIMain.Resource {
    public partial class CommonStringResources   {


        private static CommonStringResources? _instance;
        public static CommonStringResources Instance {
            get {
                if (_instance == null || _LangChanged) {
                    _LangChanged = false;
                    if (Lang.ToLower().StartsWith("ja")) {
                        _instance = new CommonStringResourcesJa();
                    } else {
                        _instance = new CommonStringResources();
                    }
                }
                return _instance;
            }
        }

        private static bool _LangChanged = false;
        private static string _Lang = "ja-JP";
        public static string Lang {
            get { return _Lang; }
            set {
                if (_Lang != value) {
                    _LangChanged = true;
                }
                _Lang = value;
            }
        }




    }
}

namespace WpfAppCommon.Model {

    public partial class AutoProcessItem {
        // System処理の名前
        public class AutoProcessActionName {
            public bool IsSystemAction { get; set;}
            public string Name { get; set; }
           
            public  AutoProcessActionName(bool isSystemAction, string name) {
                IsSystemAction = isSystemAction;
                Name = name;
            }

            public static AutoProcessActionName CopyToFolder = new AutoProcessActionName(true, "CopyToFolder");
            public static AutoProcessActionName MoveToFolder = new AutoProcessActionName(true, "MoveToFolder");
            public static AutoProcessActionName ExtractText = new AutoProcessActionName(true, "ExtractText");
            public static AutoProcessActionName MaskData = new AutoProcessActionName(true, "GetMaskedString");
            public static AutoProcessActionName SplitPathToFolderAndFileName = new AutoProcessActionName(true, "SplitPathToFolderAndFileName");
            public static AutoProcessActionName MergeAllItems = new AutoProcessActionName(true, "MergeAllItems");
            // 同じSourceApplicationTitleを持つアイテムをマージする
            public static AutoProcessActionName MergeItemsWithSameSourceApplicationTitle = new AutoProcessActionName(true, "MergeItemsWithSameSourceApplicationTitle");

            public static AutoProcessActionName RunPythonScript = new AutoProcessActionName(false, "RunPythonScript");

        }
    }

}

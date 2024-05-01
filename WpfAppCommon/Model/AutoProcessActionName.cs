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

            public static AutoProcessActionName CopyToFolder = new(true, "CopyToFolder");
            public static AutoProcessActionName MoveToFolder = new (true, "MoveToFolder");
            public static AutoProcessActionName ExtractText = new (true, "ExtractText");
            public static AutoProcessActionName MaskData = new (true, "GetMaskedString");
            public static AutoProcessActionName SplitPathToFolderAndFileName = new (true, "SplitPathToFolderAndFileName");
            public static AutoProcessActionName MergeAllItems = new (true, "MergeAllItems");
            // 同じSourceApplicationTitleを持つアイテムをマージする
            public static AutoProcessActionName MergeItemsWithSameSourceApplicationTitle = new (true, "MergeItemsWithSameSourceApplicationTitle");
            public static AutoProcessActionName RunPythonScript = new (false, "RunPythonScript");

        }
    }

}

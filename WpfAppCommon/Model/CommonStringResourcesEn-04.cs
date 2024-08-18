namespace WpfAppCommon.Model {
    public partial class CommonStringResourcesEn {


        // --- EditPythonScriptWindowViewModel ---
        public override string EnterDescription { get; } = "Enter description";

        // --- FolderEditWindowViewModel ---
        public override string FolderNotSpecified { get; } = "Folder not specified";
        public override string EnterFolderName { get; } = "Enter folder name";

        // --- FolderSelectWindowViewModel ---
        public override string FolderSelectWindowViewModelInstanceNotFound { get; } = "Error occurred. FolderSelectWindowViewModel instance not found";
        public override string SelectedFolderNotFound { get; } = "Error occurred. Selected folder not found";

        // --- ListAutoProcessRuleWindowViewModel ---
        public override string AutoProcessRuleNotSelected { get; } = "Auto process rule not selected";
        public override string ConfirmDelete { get; } = "Confirm delete";
        public override string SavedSystemCommonSettings { get; } = "Saved system common settings";
        public override string NoChangesToSystemCommonSettings { get; } = "No changes to system common settings";

        // --- ListPythonScriptWindowViewModel ---
        public override string Execute { get; } = "Execute";
        public override string SelectScript { get; } = "Select script";

        // --- SearchWindowViewModel ---
        public override string SearchFolder { get; } = "Search folder";
        public override string Standard { get; } = "Standard";
        public override string SearchConditionRuleIsNull { get; } = "Search condition rule is null";
        public override string NoSearchConditions { get; } = "No search conditions";

        // --- TagSearchWindowViewModel ---
        public override string TagIsEmpty { get; } = "Tag is empty";
        public override string TagAlreadyExists { get; } = "Tag already exists";

        // --- ClipboardApp.MainWindowViewModel ---
        public override string DisplayModeWillChangeWhenYouRestartTheApplication { get; } = "Display mode will change when you restart the application";
        public override string Information { get; } = "Information";
        public override string ConfirmExit { get; } = "Confirm exit";
        public override string NoItemSelected { get; } = "No item selected";
        public override string Cut { get; } = "Cut";
        public override string Copied { get; } = "Copied";
        public override string NoPasteFolder { get; } = "No paste folder";
        public override string NoCopyFolder { get; } = "No copy folder";
        public override string ConfirmDeleteSelectedItems { get; } = "Confirm delete selected items";
        public override string Deleted { get; } = "Deleted";

        // --- ImageCHat ---
        public override string ConfirmTheFollowingSentencesAreCorrectOrNot { get; } = "Confirm if the following sentences are correct or not";
        public override string NoImageFileSelected { get; } = "No image file selected";
        public override string SendPrompt { get; } = "Send prompt";
        public override string ImageFileName { get; } = "Image file name";
        public override string ErrorOccurred { get; } = "Error occurred";
        public override string SelectImageFilePlease { get; } = "Select image file";
        public override string AllFiles { get; } = "All files";
        public override string FileDoesNotExist { get; } = "File does not exist";

        // -- EditPromptItemWindowViewModel --
        public override string EditPrompt { get; } = "Edit prompt";

        // -- ListPromptTemplateWindow -- 
        public override string NewPromptTemplate { get; } = "New prompt template";
        public override string RAG { get; } = "RAG";

        // -- DevFeatures.cs
        public override string CannotMaskNonTextContent { get; } = "Cannot mask non-text content";
        public override string MaskedData { get; } = "Masked data";
        public override string RestoreMaskingData { get; } = "Restore masking data";
        public override string CannotGetImage { get; } = "Cannot get image";
        public override string NoRemoteRepositorySet { get; } = "No remote repository set";
        public override string NoWorkingDirectorySpecified { get; } = "No working directory specified";
        public override string SpecifiedDirectoryDoesNotExist { get; } = "Specified directory does not exist";
        public override string SpecifiedDirectoryIsNotAGitRepository { get; } = "Specified directory is not a Git repository";
        public override string NoVectorDBSet { get; } = "No vector DB set";

        // -- ScreenShotCheckCondition.cs --
        public override string CheckTypeEqual { get; } = "Equal";
        public override string CheckTypeNotEqual { get; } = "Not equal";
        public override string CheckTypeInclude { get; } = "Include";
        public override string CheckTypeNotInclude { get; } = "Not include";
        public override string CheckTypeStartWith { get; } = "Start with";
        public override string CheckTypeNotStartWith { get; } = "Not start with";
        public override string CheckTypeEndWith { get; } = "End with";
        public override string CheckTypeNotEndWith { get; } = "Not end with";
        public override string CheckTypeEmpty { get; } = "Empty";
        public override string CheckTypeCheckBox { get; } = "Check box";

        // -- ScriptAutoProcessItem.cs --
        public override string ExecutedPythonScript { get; } = "Executed Python script";

        // -- SystemAutoProcessItem.cs --
        public override string AutoProcessItemNotFound { get; } = "Auto process item not found";

        // -- EnumDescription.cs --
        public override string NotEnumType { get; } = "Not enum type";

        // --- WindowsNotificationController.cs ---
        public override string AccessDenied { get; } = "Access denied";

        // --- EditPromptItemWindowViewModel ---
        public override string EnterName { get; } = "Enter name";

        // --- EditRAGSourceWindowViewModel ---
        public override string EditRAGSource { get; } = "Edit RAG source";
        public override string ItemViewModelIsNull { get; } = "ItemViewModel is null";

        // EditVectorDBWindowViewModel
        public override string OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported { get; } = "Only Chroma (in-memory) vector DB type is currently supported";
        public override string PromptTemplateList { get; } = "Prompt template list";
        public override string NoPromptTemplateSelected { get; } = "No prompt template selected";

        // ListVectorDBWindowViewModel
        public override string SelectVectorDBToEdit { get; } = "Select vector DB to edit";
        public override string SelectVectorDBToDelete { get; } = "Select vector DB to delete";
        public override string ConfirmDeleteSelectedVectorDB { get; } = "Confirm delete selected vector DB";
        public override string SelectVectorDBPlease { get; } = "Select vector DB";

        // RAGManagementWindowViewModel
        public override string SelectRAGSourceToEdit { get; } = "Select RAG source to edit";
        public override string SelectRAGSourceToDelete { get; } = "Select RAG source to delete";
        public override string ConfirmDeleteSelectedRAGSource { get; } = "Confirm delete selected RAG source";
        public override string SelectCommitPlease { get; } = "Select commit please";
        public override string SelectTarget { get; } = "Select target";
        public override string ProcessedFileCount { get; } = "Processed file count";
        public override string CreatingIndex { get; } = "Creating index";
        public override string Completed { get; } = "Completed";
        public override string SkipUnsupportedFileType { get; } = "Skip unsupported file type";
        public override string Failed { get; } = "Failed";
        public override string IndexCreationCompleted { get; } = "Index creation completed";
        public override string IndexCreationInterrupted { get; } = "Index creation interrupted";
        public override string FailedToSendChat { get; } = "Failed to send chat";

    }

}

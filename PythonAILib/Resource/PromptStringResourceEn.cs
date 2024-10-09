namespace PythonAILib.Resource {
    public class PromptStringResourceEn : PromptStringResource {

        // Instance 
        public static PromptStringResourceEn Instance { get; set; } = new();

        // For any unclear points about the above text, please refer to the following related information
        public override string RelatedInformation { get; } = "Related information. Please note that it may contain inaccurate information.";

        // Please analyze the following text and identify sentences containing undefined terms. " +
        // "Undefined terms are those whose genus, specific difference, cause, purpose, function, or components are unclear." +
        // "Return the output as a list in the following JSON format. If there is no text to analyze or analysis is not possible, return an empty list\n" +
        // "{'result':[{'sentence':'Sentence containing undefined term','reason':'Reason for determining that the term is undefined'}]}"

        public override string AnalyzeAndDictionarizeRequest { get; } = "Please analyze the following text and identify sentences containing undefined terms." +
            "Undefined terms are those whose genus, specific difference, cause, purpose, function, or components are unclear." +
            "Return the output as a list in the following JSON format. If there is no text to analyze or analysis is not possible, return an empty list\n" +
            "{'result':[{'sentence':'Sentence containing undefined term','reason':'Reason for determining that the term is undefined'}]}";

        // For unclear sentences, please refer to the following explanation
        public override string UnknownContentDescription { get; } = "For unclear sentences, please refer to the following explanation";

        // SummaryGeneration
        public override string SummaryGeneration { get; } = "Summary";
        // Please generate a summary of 100 to 200 characters from the following text.\n
        public override string SummaryGenerationPrompt { get; } = "Please generate a summary of 100 to 200 characters from the following text.\n";


        // TODO List Generation
        public override string TasksGeneration { get; } = "TODO List";

        // "Please generate a TODO list from the following text.\n"
        public override string TasksGenerationPrompt { get; } = "Please generate a TODO list from the following text.\nOutput as a list of strings in JSON format {result:[list items]}.\n";

        // Json形式で文字列のリストを生成するプロンプト
        public override string JsonStringListGenerationPrompt { get; } = "Please generate a list of strings in JSON format {result:[list items]} as bullet points.\n";


        // BackgroundInformationGeneration
        public override string BackgroundInformationGeneration { get; } = "Background information";

        // Please generate background information (such as circumstances, purpose, cause, components, who, when, where, etc.) from the following text.\n
        public override string BackgroundInformationGenerationPrompt { get; } = "Please generate background information (such as circumstances, purpose, cause, components, who, when, where, etc.) from the following text.\n";

        public override string AnalyzeJapaneseSentenceRequest { get; } = "* A proposition is a sentence that expresses that the subject (or topic) is something.\r\n" +
            "* In general, Japanese has the following structure:\r\n " +
            "Japanese structure = [Topic] is [something + particle] + predicate + [tense, modality, aspect]\r\n" +
            "* In Japanese, parts that already have common recognition between the speaker and the listener may be omitted, and you can have a conversation with just the predicate.\r\n" +
            "* \"Modality\" refers to the way the speaker expresses their judgment about the content of the sentence or how they convey it to the listener.\r\n" +
            "* Modality can be broadly classified into four types:\r\n" +
            "   - Modality that expresses communicative differentiation of the sentence: Expressive modality\r\n" +
            "   - Modality that expresses the way of perceiving the situation: Evaluative modality, Recognitional modality\r\n" +
            "   - Modality that expresses the association between the sentence and the preceding context: Explanatory modality\r\n" +
            "   - Modality that expresses the way of conveying to the listener: Politeness modality, Attitudinal modality\r\n" +
            "* \"Expressive modality\" represents the basic nature of the sentence, such as [narrative], [volition], [imperative], and [interrogative].\r\n" +
            "  - [Narrative] Read the textbook.\r\n  - [Volition] Let's read the textbook.\r\n  - [Imperative] Read the textbook.\r\n  - [Interrogative] Will you read the textbook?\r\n" +
            "* \"Evaluative modality\" represents the speaker's evaluative perception of the situation, such as necessary or unnecessary.\r\n" +
            "  - In the library, you must be quiet.\r\n" +
            "* \"Recognitional modality\" represents how the speaker perceives the content of the proposition.\r\n" +
            "  - It will probably be sunny tomorrow.\r\n" +
            "* \"Explanatory modality\" represents that the modality in the sentence is related to the preceding sentence.\r\n" +
            "  - Is it snowing? No wonder it's cold.\r\n" +
            "* \"Politeness modality\" refers to the style choice of whether to convey the sentence in [plain form] or [polite form] to the listener.\r\n" +
            "  - [Plain form] I read this book today.\r\n  - [Polite form] I read this book today.\r\n" +
            "* \"Attitudinal modality\" refers to fine adjustments when conveying to the listener or expressing the speaker's recognition state.\r\n" +
            "  - Look at this.\r\n  - What a beautiful view.\r\n\r\n" +
            "Please perform the following processing for the following sentence:\r\n" +
            "- Complete the omitted parts and create a list of propositions in the format of the above \"Japanese structure\". Also, explain the modality of each proposition.\r\n" +
            "  If there are multiple expected results, list up to 10 of the most likely ones.";


        public override string GenerateQuestionRequest { get; } = "Please analyze the text and generate questions.\r\nExample:\r\n# Questions about definition (genus and specific difference)\r\n Text: Ponchororin soup is delicious.\r\n Question: Does Ponchororin soup belong to the category of soup dishes? Or what makes it different from other items in the category?\r\n# Questions about purpose and reason\r\n Text: The task of XX must be completed by the end of today.\r\n Question: What is the reason for needing to complete the task of XX by the end of today? Also, what is the purpose of performing the task of XX?\r\n# Questions about cause, background, and history\r\n Text: Tokugawa Ieyasu is a shogun.\r\n Question: What is the cause of Tokugawa Ieyasu becoming a shogun?\r\n# Questions about components and functions\r\n Text: Ponchororin soup is good for health.\r\n Question: What ingredients are used to make Ponchororin soup? And what effects does it have?";

        public override string AnswerRequest { get; } = "Please answer the following questions.\n";

        // Title generation
        public override string TitleGeneration { get; } = "Title generation";
        // Please generate a title from the following text.\n
        public override string TitleGenerationPrompt { get; } = "Please generate a title from the following text.\n";

        // Please extract the text from this image.\n
        public override string ExtractTextRequest { get; } = "Please extract the text from this image.\n";


    }
}

namespace PythonAILib.Resource {
    public class PromptStringResourceEn : PromptStringResource {

        // For any unclear points about the above text, please refer to the following related information
        public override string RelatedInformation { get; } = "Related information. Please note that it may contain inaccurate information.";

        // Please analyze the following text and identify sentences containing undefined terms. " +
        // "Undefined terms are those whose genus, specific difference, cause, purpose, function, or components are unclear." +
        // "Return the output as a list in the following JSON format. If there is no text to analyze or analysis is not possible, return an empty list\n" +
        // "{'result':[{'sentence':'Sentence containing undefined term','reason':'Reason for determining that the term is undefined'}]}"

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


        public override string GenerateQuestionRequest { get; } = "Please analyze the text and generate questions.\r\nExample:\r\n# Questions about definition (genus and specific difference)\r\n Text: Ponchororin soup is delicious.\r\n Question: Does Ponchororin soup belong to the category of soup dishes? Or what makes it different from other items in the category?\r\n# Questions about purpose and reason\r\n Text: The task of XX must be completed by the end of today.\r\n Question: What is the reason for needing to complete the task of XX by the end of today? Also, what is the purpose of performing the task of XX?\r\n# Questions about cause, background, and history\r\n Text: Tokugawa Ieyasu is a shogun.\r\n Question: What is the cause of Tokugawa Ieyasu becoming a shogun?\r\n# Questions about components and functions\r\n Text: Ponchororin soup is good for health.\r\n Question: What ingredients are used to make Ponchororin soup? And what effects does it have?";

        public override string AnswerRequest { get; } = "Please answer the following questions.\n";

        // Title generation
        public override string TitleGeneration { get; } = "Title generation";
        // Please generate a title from the following text.\n
        public override string TitleGenerationPrompt { get; } = "Please generate a title from the following text.\n";

        // Please extract the text from this image.\n
        public override string ExtractTextRequest { get; } = "Please extract the text from this image.\n";

        // Document reliability
        public override string DocumentReliability { get; } = "Document Reliability";

        // Document reliability judgment
        public override string DocumentReliabilityCheckPrompt { get; } = "# Information Reliability Judgment" +
            "## Overview\r\nThe level at which information can be used as evidence for other information." +
            "## How to Judge" +
            "First, set the following indicators as rough guidelines." +
            "### Judgment based on the source and scope of the text" +
            "* If it is written by an authoritative organization, institution, or person and is generally available information, the reliability level is high (reliability: 90-100%)." +
            "  However, further classification of sites with high reliability is required." +
            "* Information from sites that require reliable information, such as Wikipedia, is of medium to high reliability (reliability: 70-90%)." +
            "  However, further classification of sites with medium to high reliability is required." +
            "* Information from sites such as StackOverflow, which may contain errors but can be checked by many people, is of low to medium reliability (reliability: 40-60%)." +
            "* If it is written by an organization or person within the company and the scope of disclosure is limited to the organization, the reliability level is low to high (reliability: 40-90%)." +
            "  * Documents that are expected to be seen by many people within the organization, such as emails, Teams chats for requests or confirmations, notifications, and research presentation materials." +
            "  * The information may include works in progress or unreviewed information." +
            "* If the assumed scope of disclosure is unknown or the text is considered to be between individuals, the reliability level is low (reliability: 10-30%)." +
            "  * Personal ideas, memos, and texts with unclear context." +
            "### Judgment based on the content" +
            "* The reliability of texts at each reliability level can be influenced by their content." +
            "  * Information that can be determined to be correct based on existing logic, mathematical laws, or natural scientific laws should have the upper limit of reliability within the level." +
            "  * Information that can be determined to be somewhat correct based on general sociological laws, customs, etc. should have the middle value of reliability within the level." +
            "  * Information for which correctness cannot be determined and verification is required should have the lower limit of reliability within the level." +
            "" +
            "\"Based on the above, please determine the reliability level of the following text and output the reliability score (0-100) along with the reason for the reliability determination.";

        // Prompt to get reliability from the document reliability check result
        public override string DocumentReliabilityDictonaryPrompt { get; } = "The following text is the result of determining the reliability of a document. Please output the final reliability score (0-100)." +
            "Please format the output in the following JSON format: {\"reliability\": reliability score, \"reason\": \"reason for the reliability score\"}";


    }
}

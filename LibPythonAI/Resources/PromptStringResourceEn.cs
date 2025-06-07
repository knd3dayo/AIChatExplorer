namespace PythonAILib.Resources {
    public class PromptStringResourceEn : PromptStringResource {

        // For any unclear points about the above text, please refer to the following related information
        public override string RelatedInformation {
            get {
                return "------ The following is reference information.---\nInformation may have a reliability score assigned. If there is conflicting content in the reference information, please prioritize the information with the higher reliability. The definition of reliability is as follows:\n" +
                    DocumentReliabilityDefinition + "\n " +
                    "To inform the user of the reliability of the information, please provide the reference information used with the response, sorted by reliability score\n ------";
            }
        }

        // List of existing tags  
        public override string TagListPrompt(string tags) {
            return $"The list of existing tags is as follows:\n{tags}\n";
        }



        // ベクトルDBシステムメッセージ。 
        public override string VectorDBSystemMessage(string description) {
            string message = $"The following information is stored in the vector DB. It searches for data that matches the string specified by the user\n\n {description}";
            return message;
        }

        public override string UnknownContentDescription { get; } = "For unclear sentences, please refer to the following explanation";

        // RelatedInformationByVectorSearch
        public override string RelatedInformationByVectorSearch { get; } = "------ The following is the result of searching for information related to the text from the vector DB.---\n";
        // SummarizePromptText
        public override string SummarizePromptText { get; } = "Since it is merely a concatenation, there might be sections where the text does not flow well. Please restructure the text to improve its coherence. The output language should be English.\n";

        // Json形式で文字列のリストを生成するプロンプト
        public override string JsonStringListGenerationPrompt { get; } = "Please generate a list of strings in JSON format {result:[list items]} as bullet points.\n";


        public override string GenerateQuestionRequest { get; } = "Please analyze the text and generate questions.\r\nExample:\r\n# Questions about definition (genus and specific difference)\r\n Text: Ponchororin soup is delicious.\r\n Question: Does Ponchororin soup belong to the category of soup dishes? Or what makes it different from other items in the category?\r\n# Questions about purpose and reason\r\n Text: The task of XX must be completed by the end of today.\r\n Question: What is the reason for needing to complete the task of XX by the end of today? Also, what is the purpose of performing the task of XX?\r\n# Questions about cause, background, and history\r\n Text: Tokugawa Ieyasu is a shogun.\r\n Question: What is the cause of Tokugawa Ieyasu becoming a shogun?\r\n# Questions about components and functions\r\n Text: Ponchororin soup is good for health.\r\n Question: What ingredients are used to make Ponchororin soup? And what effects does it have?";

        public override string AnswerRequest { get; } = "Please answer the following questions.\n";

        // Please extract the text from this image.\n
        public override string ExtractTextRequest { get; } = "Please extract the text from this image.\n";

        // Prompt to get reliability from the document reliability check result
        public override string DocumentReliabilityDictionaryPrompt { get; } = "The following text is the result of determining the reliability of a document. Please output the final reliability score (0-100)." +
            "Please format the output in the following JSON format: {\"reliability\": reliability score, \"reason\": \"reason for the reliability score\"}";

        public override string DocumentReliabilityDefinition { get; } = "Document reliability indicates the level at which a document can be considered as a basis for another document.\n" +
            "### Determination based on document origin and publication scope\n" +
            "* High reliability level (reliability: 90-100%) if the document is written by authoritative organizations or individuals and publicly available information.\n" +
            "* Medium to high reliability level (reliability: 70-90%) if the information is from sites like Wikipedia where reliable information is required.\n" +
            "* Low to high reliability level (reliability: 40-90%) if the document is written by internal organizations or individuals and the scope of publication is limited to the organization.\n" +
            "* Low reliability level (reliability: 10-30%) if the publication scope is unclear or if the document is believed to be personal communication.\n" +
            "## Determination based on content\n" +
            "* The reliability of documents at each level can vary based on their content.\n" +
            "  * Information that can be determined to be correct based on existing logical, mathematical, or natural scientific laws is assigned the upper limit of reliability within the level.\n" +
            "  * Information that can be somewhat reliably determined based on general sociological laws or customs is assigned the middle value of reliability within the level.\n" +
            "  * Information whose accuracy cannot be determined and requires verification is assigned the lower limit of reliability within the level.\n";

    }
}

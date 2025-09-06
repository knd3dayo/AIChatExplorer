namespace LibMain.Resources {
    public class PromptStringResource {

        // Instance
        public static PromptStringResource Instance { get; set; } = new();



        // List of existing tags  
        public virtual string TagListPrompt(string tags) {
            return $"The list of existing tags is as follows:\n{tags}\n";
        }
        public virtual string SummarizePromptText { get; } = "Since it is merely a concatenation, there might be sections where the text does not flow well. Please restructure the text to improve its coherence. The output language should be English.\n";

        // Json形式で文字列のリストを生成するプロンプト
        public virtual string JsonStringListGenerationPrompt { get; } = "Please generate a list of strings in JSON format {result:[list items]} as bullet points.\n";

        // Please extract the text from this image.\n
        public virtual string ExtractTextRequest { get; } = "Please extract the text from this image.\n";

        // Prompt to get reliability from the document reliability check result
        public virtual string DocumentReliabilityDictionaryPrompt { get; } = "The following text is the result of determining the reliability of a document. Please output the final reliability score (0-100)." +
            "Please format the output in the following JSON format: {\"reliability\": reliability score, \"reason\": \"reason for the reliability score\"}";

        public virtual string DocumentReliabilityDefinition { get; } = "Document reliability indicates the level at which a document can be considered as a basis for another document.\n" +
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

from dotenv import load_dotenv
import os

def get_props():
    load_dotenv()
    props = {
        "OpenAIKey": os.getenv("OPENAI_API_KEY"),
        "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
        "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
        "AzureOpenAI": os.getenv("AZURE_OPENAI"),
        "OpenAICompletionBaseURL": os.getenv("OPENAI_COMPLETION_BASE_URL"),
        "OpenAIEmbeddingBaseURL": os.getenv("OPENAI_EMBEDDING_BASE_URL"),
        "VectorDBURL" : os.getenv("VECTOR_DB_URL"),
        "ToolsJSONPath" : os.getenv("TOOLS_JSON_PATH"),

    }
    return props
from dotenv import load_dotenv
import os

def get_props():
    load_dotenv()
    props = {
        "OpenAIKey": os.getenv("OPENAI_API_KEY"),
        "AzureOpenAIEndpoint": os.getenv("AZURE_OPENAI_ENDPOINT"),
        "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
        "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
        "AzureOpenAI": os.getenv("AZURE_OPENAI"),
        "OpenAIBaseURL": os.getenv("OPENAI_BASE_URL"),
        "VectorDBURL" : os.getenv("VECTOR_DB_URL"),
        "ToolsJSONPath" : os.getenv("TOOLS_JSON_PATH"),

    }
    return props
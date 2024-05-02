from dotenv import load_dotenv
import os, json

def get_props():
    load_dotenv()
    props = {
        "OpenAIKey": os.getenv("OPENAI_API_KEY"),
        "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
        "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
        "AzureOpenAI": os.getenv("AZURE_OPENAI"),
        "OpenAICompletionBaseURL": os.getenv("OPENAI_COMPLETION_BASE_URL"),
        "OpenAIEmbeddingBaseURL": os.getenv("OPENAI_EMBEDDING_BASE_URL"),
    }
    return props

def get_vector_db_settings():
    load_dotenv()
    settings = {}
    # name
    settings["name"] = "test"
    
    # vector_db_url
    settings["vector_db_url"] = os.getenv("VECTOR_DB_URL")
    # description
    settings["description"] = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
    # vector_db_type_string
    settings["vector_db_type_string"] = "Faiss"
    
    # json文字列にする
    settings_json = json.dumps([settings], ensure_ascii=False)
    return settings_json

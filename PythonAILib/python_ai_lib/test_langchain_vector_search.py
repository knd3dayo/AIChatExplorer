from ai_app_langchain.ai_app_langchain_util import LangChainUtil
import os
from ai_app_langchain.langchain_vector_db_chroma import LangChainVectorDBChroma
from ai_app_openai.ai_app_openai_util import OpenAIProps 
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps

if __name__ == '__main__':
    props:OpenAIProps  = OpenAIProps.env_to_props()
    vector_db_item: VectorDBProps = VectorDBProps.get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = LangChainUtil.run_vector_search(props, [vector_db_item], question1, search_kwargs = {"k":10, "score_threshold":0.0})

    print(result1)


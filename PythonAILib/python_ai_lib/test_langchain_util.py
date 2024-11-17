from ai_app_openai.ai_app_openai_util import OpenAIProps 
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_langchain.ai_app_langchain_util import LangChainUtil
if __name__ == '__main__':

    props:OpenAIProps = OpenAIProps.env_to_props()
    vector_db_items:VectorDBProps = VectorDBProps.get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = LangChainUtil.langchain_chat(props, [vector_db_items], question1)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
    
    verbose = result1.get("verbose", "")
    print(verbose)


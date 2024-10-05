from openai_props import OpenAIProps, VectorDBProps, env_to_props, get_vector_db_settings
from langchain_util import langchain_chat
if __name__ == '__main__':

    props:OpenAIProps = env_to_props()
    vector_db_items:VectorDBProps = get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = langchain_chat(props, [vector_db_items], question1)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
    
    verbose = result1.get("verbose", "")
    print(verbose)


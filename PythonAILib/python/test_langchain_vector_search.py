from openai_props import OpenAIProps, VectorDBProps, env_to_props, get_vector_db_settings
from langchain_util import vector_search

if __name__ == '__main__':
    props:OpenAIProps  = env_to_props()
    vector_db_item: VectorDBProps = get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = vector_search(props, vector_db_item, question1)

    print(result1)


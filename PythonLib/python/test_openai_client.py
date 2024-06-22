from openai_client import OpenAIClient

if __name__ == "__main__":
    
    # テスト用のjson
    input_json_001 = """
    [
        {
            "role": "system",
            "content": "You are a helpful assistant."
        },
        {
            "role": "user",
            "content": "What is your name?"
        }
    ]
    """
    input_json_002 = """
    [
        {
            "role": "system",
            "content": "You are a helpful assistant."
        },
        {
            "role": "user",
            "content": "Please output json format. What is your name?"
        }
    ]
    """
    
    
    import openai_props
    # envファイルからpropsを取得する
    props = openai_props.get_props()
    openai_client = OpenAIClient(props)

    # chatを実行
    # openai_chatのパラメーターを作成
    result = openai_client.openai_chat(input_json_001)
    print(result)
    # embeddingを実行
    result = openai_client.openai_embedding("Hello, world")
    print(result)
    # json_modeのchatを実行
    result = openai_client.openai_chat(input_json_002, True)
    print(result)
    # list_openai_modelsを実行
    result = openai_client.list_openai_models()
    print(result)
    
    # gpt4-vのchatを実行
    result = openai_client.openai_chat_with_vision("画像に「Hello World!」という文字列が含まれている場合Yes,そうでない場合はNoと答えてください", ["../TestData/extract_test.png"])
    print(result)

    
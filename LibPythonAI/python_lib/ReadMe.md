## APIの説明

### request
```json
{
    "autogen_props": {
        "work_dir": "autogenの作業用ディレクトリ",
        "tool_dir": "ツール出力ディレクトリ",
        "chat_history_folder_id": "47bbf240-5022-470f-9b8b-0de0e600595a",
        "chat_type": "group",
        "chat_name": "default",
        "terminate_msg": "TERMINATE",
        "max_msg": 15,
        "timeout": 120,
        "session_token": "5aeb0538-a082-43de-9eb0-cc8452a087d9"
    },
    "openai_props": {
        "OpenAIKey": "str: OpenAIのAPIキー",
        "AzureOpenAI": "bool: Azure OpenAIを使用するか否か",
        "AzureOpenAIAPIVersion": "str: AureOpenAIのAPIバージョン",
        "AzureOpenAIEndpoint": "str: AureOpenAIのエンドポイントURL",
        "OpenAIBaseURL": "str: カスタムモデル用のBaseURL",
    },
    "vector_search_requests": [
    {
        "Name": "default",
        "model": "text-embedding-3-small",
        "input_text": "検索文字列",
        "SearchKwargs": {
            "k": 10,
            "filter": {
                "folder_id": "47bbf240-5022-470f-9b8b-0de0e600595a"
            }
        }
    }
    ],
    "embedding_request":
        {
        "Name": "default",
        "model": "text-embedding-3-small",
        "Embedding": {
            "FolderId": "47bbf240-5022-470f-9b8b-0de0e600595a",
            "source_id": "",
            "source_type": 0,
            "description": "",
            "content": "",
            "source_path": "",
            "git_repository_url": "",
            "git_relative_path": "",
            "image_url": "",
            "doc_id": "",
            "score": 0,
            "sub_docs": []
        }
    },
    "vector_db_item_request" : {

    },
    "chat_request": {
        "model": "gpt-4o-mini",
        "messages": [
        {
            "role": "user",
            "content": [
            {
                "type": "text",
                "text": "\n\n"
            }
            ]
        }
        ],
        "temperature": 0.5
    },
    "chat_request_context": {
        "prompt_template_text": "",
        "chat_mode": "Normal",
        "split_mode": "None",
        "summarize_prompt_text": "単純に結合しただけなので、文章のつながりがよくない箇所があるかもしれません。 文章のつながりがよくなるように整形してください。 出力言語は日本語にしてください。\n",
        "related_information_prompt_text": "------ 以下は本文に関連する情報をベクトルDBから検索した結果です。---\n",
        "split_token_count": 8000,
    },

}
```

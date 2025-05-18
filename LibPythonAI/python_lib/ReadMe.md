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
        "name": "default",
        "model": "text-embedding-3-small",
        "query": "検索文字列",
        "search_kwargs": {
            "k": 10,
            "filter": {
                "folder_id": "47bbf240-5022-470f-9b8b-0de0e600595a",
                "folder_path" : "フォルダのパス"
            }
        }
    }
    ],
    "embedding_request":
        {
        "name": "default",
        "model": "text-embedding-3-small",
        "folder_id": "47bbf240-5022-470f-9b8b-0de0e600595a",
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
    },
    "vector_db_item_request" : {
        "id": "ベクトルDBのID",
        "name": "ベクトルDBの名前",
        "description": "ベクトルDBの説明",
        "vector_db_url": "ベクトルDBのURLまたはパス",
        "is_use_multi_vector_retriever":　"MultiVectorRetrieverを使用するかどうか",
        "doc_store_url": "MultiVectorRetriever用のDBのURL",
        "vector_db_type": "0:Chroma, 1:PGVector",
        "collection_name": "ベクトル格納用のコレクション名",
        "chunk_size": "チャンクサイズ。データはこの単位に分割してベクトル化する",
        "default_search_result_limit": "デフォルトの検索結果表示数",
        "is_enabled": "このベクトルDBを有効にするか否か。",
        "is_system": "システム用のベクトルDBか否か"
    },
    "autogen_llm_config_request": {
        "name": "名前",
        "api_type" : "APIのタイプ",
        "api_version" : "APIバージョン",
        "model" : "モデル",
        "api_key" : "APIキー",
        "base_url" : "BaseURL"
    },

    "autogen_tool_request": {
        "name": "名前",
        "path": "パス",
        "description": "説明"
    },

    "autogen_agent_request": {
        "name": "名前",
        "description": "説明",
        "system_message": "システムメッセージ",
        "code_execution" : "コード実行",
        "llm_config_name" : "LLMの設定",
        "tool_names" : "ツール名",
        "vector_db_props" : "vector_db_props"
    },

    "autogen_group_chat_request": {
        "name": "名前",
        "description": "説明",
        "llm_config_name": "LLMの設定",
        "agent_names": "AIエージェント"
    },

    "tag_item_requests" : [
      {  "Id" : "タグID",
        "Tag" : "タグ名",
        "IsPinned" : "ピン留め状態"
        }
    ],

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

### response




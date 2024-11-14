import os, json
from typing import Any
from ai_app_openai_util import OpenAIProps
from ai_app_vector_db_util import VectorDBProps
from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor
import autogen 
from ai_app_autogen_util import AutoGenProps, AutoGenTools, AutoGenAgents

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

import os
import tempfile
import sys
import getopt

if __name__ == '__main__':

    # getoptsでオプション引数の解析
    # -p オプションでOpenAIプロパティファイル(JSON)を指定する
    # -v オプションでVectorDBプロパティファイル(JSON)を指定する
    # -o オプションで出力ファイルを指定する
    # -m オプションでメッセージを指定する
    message = None
    output_file = None
    props_file = None

    opts, args = getopt.getopt(sys.argv[1:], "m:o:p:")
    for opt, arg in opts:
        if opt == "-m":
            message = arg
        elif opt == "-o":
            output_file = arg
        elif opt == "-p":
            props_file = arg
    
    # プロパティファイル(JSON)を読み込む
    open_ai_props_dict = {}
    vector_db_props_dict = []
    request = {}

    if props_file:
        print(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)
            open_ai_props_dict = props_dict.get("open_ai_props", {})
            open_ai_props = OpenAIProps(open_ai_props_dict)

            vector_db_props_dict = props_dict.get("vector_db_props", [])
            vector_db_props_list = [VectorDBProps(props) for props in vector_db_props_dict]

            request = props_dict.get("chat_request", {})

    else:
            open_ai_props: OpenAIProps = OpenAIProps.env_to_props()
            vector_db_props_list = [VectorDBProps.get_vector_db_settings()]

    input_text = ""
    # messageが指定されている場合は, messageを入力テキストとする
    if message:
        input_text = message
    else:
        # requestの[messages][0][content]の最後の要素を入力テキストとする
        messages = request.get("messages", [])
        if messages:
            last_content = messages[0].get("content",[])[-1]
            input_text = last_content.get("text", "")

    # メッセージが指定されていない場合は入力メッセージがない旨を表示して終了
    if not input_text:
        print("Input message is not specified.")
        sys.exit(1)

    # メッセージを表示
    print(f"Input message: {input_text}")

    # Create a temporary directory to store the code files.
    temp_dir = tempfile.TemporaryDirectory()

    # Create a local command line code executor.
    executor = LocalCommandLineCodeExecutor(
        timeout=10,  # Timeout for each code execution in seconds.
        work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
    )

    autogenProps: AutoGenProps = AutoGenProps(open_ai_props)
    client = AutoGenAgents(autogenProps.llm_config, executor, temp_dir.name)

    client.enable_code_writer_and_executor()
    client.enable_web_searcher()
    client.enable_azure_document_searcher()
    client.enable_vector_searcher(open_ai_props, vector_db_props_list)
    client.enable_file_extractor()

    if output_file:
        client.set_output_file(output_file)
        print(f"Output file: {output_file}")

    group_chat = client.execute_group_chat(input_text, 10)
    # Print the messages in the group chat.
    for message in group_chat.messages:
        # roleがuserまたはassistantの場合はrole, name, contentを表示
        if message["role"] in ["user", "assistant"]:
            print(f"role:[{message['role']}] name:[{message['name']}]")
            print ("------------------------------------------")
            print(f"content:{message['content']}\n")

    # Delete the temporary directory.
    temp_dir.cleanup()
    



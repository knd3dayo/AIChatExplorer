import sys
import ai_app_wrapper
import json

if __name__ == "__main__":
    request_file = sys.argv[1]
    with open(request_file, 'r', encoding="utf8") as file:
        request = file.read()
        request_dict = json.loads(request)
    context_json = json.dumps(request_dict['context'], ensure_ascii=False, indent=4)
    response = ai_app_wrapper.get_autogen_default_definition(context_json)

    print(response)
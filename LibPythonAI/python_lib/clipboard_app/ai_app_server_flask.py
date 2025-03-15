import os, sys
from typing import Any
from flask_cors import CORS # type: ignore
from flask import Flask, Response, request, render_template
from flask_socketio import SocketIO, emit, send # type: ignore
import clipboard_app.ai_app_wrapper as ai_app_wrapper
import asyncio
from clipboard_app.autogen_modules import AutoGenProps

app = Flask(__name__)
CORS(app)  # すべてのオリジンからのアクセスを許可


app.config["JSON_AS_ASCII"] = False
app.config['ASCII'] = False
app.config['SECRET_KEY'] = 'secret!'

socketio = SocketIO(app)

@app.route('/api/openai_chat', methods=['POST'])
def openai_chat():
    request_json = request.data
    response = ai_app_wrapper.openai_chat(request_json)
    print(response)
    return Response(response, mimetype='application/json')

@app.route('/api/get_token_count', methods=['POST'])
def get_token_count():
    request_json = request.data
    response = ai_app_wrapper.get_token_count(request_json)
    print(response)
    return Response(response, mimetype='application/json')

@app.route('/api/langchain_chat', methods=['POST'])
def langchain_chat():
    request_json = request.data
    response = ai_app_wrapper.langchain_chat(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# vector_search
@app.route('/api/vector_search', methods=['POST'])
def vector_search():
    request_json = request.data
    response = ai_app_wrapper.vector_search(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# delete_collection
@app.route('/api/delete_collection', methods=['POST'])
def delete_collection():
    request_json = request.data
    response = ai_app_wrapper.delete_collection(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# delete_embeddings
@app.route('/api/delete_embeddings', methods=['POST'])
def delete_embeddings():
    request_json = request.data
    response = ai_app_wrapper.delete_embeddings(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# update_embeddings
@app.route('/api/update_embeddings', methods=['POST'])
def update_embeddings():
    request_json = request.data
    response = ai_app_wrapper.update_embeddings(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# get_mime_type
@app.route('/api/get_mime_type', methods=['POST'])
def get_mime_type():
    request_json = request.data
    response = ai_app_wrapper.get_mime_type(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# get_sheet_names
@app.route('/api/get_sheet_names', methods=['POST'])
def get_sheet_names():
    request_json = request.data
    response = ai_app_wrapper.get_sheet_names(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# extract_excel_sheet
@app.route('/api/extract_excel_sheet', methods=['POST'])
def extract_excel_sheet():
    request_json = request.data
    response = ai_app_wrapper.extract_excel_sheet(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# extract_text_from_file
@app.route('/api/extract_text_from_file', methods=['POST'])
def extract_text_from_file():
    request_json = request.data
    response = ai_app_wrapper.extract_text_from_file(request_json)
    print(response)
    return Response(response, mimetype='application/json')


# extract_base64_to_text
@app.route('/api/extract_base64_to_text', methods=['POST'])
def extract_base64_to_text():
    request_json = request.data
    response = ai_app_wrapper.extract_base64_to_text(request_json)
    print(response)
    return Response(response, mimetype='application/json')


# extract_webpage
@app.route('/api/extract_webpage', methods=['POST'])
def extract_webpage():
    request_json = request.data
    response = ai_app_wrapper.extract_webpage(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# export_to_excel
@app.route('/api/export_to_excel', methods=['POST'])
def export_to_excel():
    request_json = request.data
    response = ai_app_wrapper.export_to_excel(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# import_from_excel
@app.route('/api/import_from_excel', methods=['POST'])
def import_from_excel():
    request_json = request.data
    response = ai_app_wrapper.import_from_excel(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# hello_world
@app.route('/api/hello_world', methods=['POST'])
def hello_world():
    request_json = request.data
    response = ai_app_wrapper.hello_world(request_json)
    print(response)
    return Response(response, mimetype='application/json')

@app.route('/')
def index():
    return render_template('index.html')

def handle_message(msg):
    print('Message received: ' + msg)
    emit('response', msg, broadcast=True)


@app.route('/api/cancel_autogen_chat', methods=['POST'])
def cancel_autogen_chat():
    import json
    request_json = request.data
    request_data = json.loads(request_json)
    session_token = request_data.get("session_token")
    print(f"cancel_autogen_chat: {session_token}")
    # session_toknes
    print (AutoGenProps.session_tokens)
    AutoGenProps.remove_session_token(session_token)

@socketio.on('autogen_chat')
def autogen_group_chat(request_json: str):
    async def task():
        try:

            async for response in ai_app_wrapper.autogen_chat(request_json):
                emit("response", response)

        except Exception as e:
            import traceback
            emit("error", traceback.format_exc())
        finally:
            print("close", file=sys.stderr)
            emit("close", "close")

    asyncio.run(task())


@app.route('/api/shutdown', methods=['POST', 'GET'])
def shutdown_server():
    pid = os.getpid()
    # Ctrl+CでSIGINTを送信してもらう
    os.kill(pid, 2)

def pf_trace():
    pf_trace = os.getenv("PF_TRACE", "false").upper() == "TRUE"
    print(f"pf_trace={pf_trace}", file=sys.stderr)
    if pf_trace == True:
        os.environ["PF_DISABLE_TRACING"] = "false"
        os.environ["NO_PROXY"] = "*"
        try:
            from promptflow.tracing import start_trace # type: ignore
            # instrument OpenAI
            start_trace(collection="ai_app_server")
        except Exception as e:
            print(e.message)
            print("Failed to start tracing")
            

if __name__ == ('__main__'):
    flask_port = os.getenv("FLASK_PORT", "5000")
    print(f"port={flask_port}", file=sys.stderr)
    # pf_trace()
    socketio.run(app, debug=True, host='0.0.0.0', port=flask_port, allow_unsafe_werkzeug=True)
    # app.run(debug=True, host='0.0.0.0', port=flask_port, threaded=True)
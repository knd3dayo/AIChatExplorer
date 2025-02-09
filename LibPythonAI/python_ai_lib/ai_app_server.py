import os, sys
from typing import Any
from flask_cors import CORS
from flask import Flask, Response, request, render_template
from flask_socketio import SocketIO, emit


app = Flask(__name__)
CORS(app)  # すべてのオリジンからのアクセスを許可

import ai_app_wrapper

app.config["JSON_AS_ASCII"] = False

app = Flask(__name__)
app.config['ASCII'] = False

socketio = SocketIO(app)

@app.route('/')
def index():
    return render_template('index.html')

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

@app.route('/api/autogen_group_chat', methods=['POST'])
def autogen_group_chat():
    request_json = request.data
    response = ai_app_wrapper.autogen_group_chat(request_json)
    print(response)
    return Response(response, mimetype='application/json')

@app.route('/api/langchain_chat', methods=['POST'])
def langchain_chat():
    request_json = request.data
    response = ai_app_wrapper.langchain_chat(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# get_catalog_list
@app.route('/api/get_catalog_list', methods=['POST'])
def get_catalog_list():
    request_json = request.data
    response = ai_app_wrapper.get_catalog_list(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# get_catalog
@app.route('/api/get_catalog', methods=['POST'])
def get_catalog():
    request_json = request.data
    response = ai_app_wrapper.get_catalog(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# get_catalog_description
@app.route('/api/get_catalog_description', methods=['POST'])
def get_catalog_description():
    request_json = request.data
    response = ai_app_wrapper.get_catalog_description(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# update_catalog_description
@app.route('/api/update_catalog_description', methods=['POST'])
def update_catalog_description():
    request_json = request.data
    response = ai_app_wrapper.update_catalog_description(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# vector_search
@app.route('/api/vector_search', methods=['POST'])
def vector_search():
    request_json = request.data
    response = ai_app_wrapper.vector_search(request_json)
    print(response)
    return Response(response, mimetype='application/json')

# update_collection
@app.route('/api/update_collection', methods=['POST'])
def update_collection():
    request_json = request.data
    response = ai_app_wrapper.update_collection(request_json)
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

@socketio.on('connect', namespace='/api/autogen_group_chat')
def autogen_group_chat(request_json: str):
    for response in ai_app_wrapper.autogen_group_chat(request_json):
        emit("response", response)
    emit("close", "close")


@app.route('/api/shutdown', methods=['POST', 'GET'])
def shutdown_server():
    pid = os.getpid()
    # Ctrl+CでSIGINTを送信してもらう
    os.kill(pid, 2)

def pf_trace():
    pf_trace = os.getenv("PF_TRACE", "false").upper() == "TRUE"
    print(f"pf_trace={pf_trace}", file=sys.stderr)
    if pf_trace == True:
        from promptflow.tracing import start_trace
        # instrument OpenAI
        start_trace(collection="ai_app_server")


if __name__ == ('__main__'):
    flask_port = os.getenv("FLASK_PORT", "5000")
    print(f"port={flask_port}", file=sys.stderr)
    # pf_trace()
    socketio.run(app, debug=True, host='0.0.0.0', port=flask_port, allow_unsafe_werkzeug=True)
    # app.run(debug=True, host='0.0.0.0', port=flask_port, threaded=True)
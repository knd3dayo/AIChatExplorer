import os, json
from typing import Any
from collections.abc import Generator
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


@socketio.on('connect', namespace='/api/autogen_group_chat')
def autogen_group_chat(request_json: str):
    for response in ai_app_wrapper.autogen_group_chat(request_json):
        emit("response", response)
    emit("close", "close")


if __name__ == ('__main__'):
    import sys
    if len(sys.argv) > 1:
        port = int(sys.argv[1])
    else:
        port = 5000
    print(f"port={port}")
    socketio.run(app, debug=True, host='0.0.0.0', port=port, allow_unsafe_werkzeug=True)

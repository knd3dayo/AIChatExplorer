import os, json
from typing import Any
from collections.abc import Generator
from io import StringIO
import sys

import ai_app_wrapper


from flask import Flask, render_template
from flask_socketio import SocketIO, send

app = Flask(__name__)
app.config['SECRET_KEY'] = 'secret!'
socketio = SocketIO(app)

@app.route('/')
def index():
    return render_template('index.html')

@socketio.on('openai_embedding')
def openai_embedding(msg):
    print('Message received: ' + msg)
    send(msg, broadcast=True)

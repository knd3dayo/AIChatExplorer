import os, sys
from typing import Any
import logging

from aiohttp import web, WSMsgType
from aiohttp.web import WebSocketResponse, Request, Response
import socketio # type: ignore
import ai_chat_explorer.api_modules.ai_app_wrapper as ai_app_wrapper
import ai_chat_explorer.api_modules.ai_app_util as ai_app_util
from ai_chat_explorer.autogen_modules import AutoGenProps

routes = web.RouteTableDef()
app = web.Application()
sio = socketio.AsyncServer(async_mode='aiohttp')
sio.attach(app)
logger = logging.getLogger(__name__)

@routes.post('/api/get_tag_items')
async def get_tag_items(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_tag_items(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/update_tag_items')
async def update_tag_items(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.update_tag_items(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/delete_tag_items')
async def delete_tag_items(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.delete_tag_items(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/get_root_content_folders')
async def get_root_content_folders(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_root_content_folders(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/openai_chat')
async def openai_chat(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.openai_chat(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/get_token_count')
async def get_token_count(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_token_count(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

@routes.post('/api/langchain_chat')
async def langchain_chat(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.langchain_chat(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# update_vector_db
@routes.post('/api/update_vector_db_item')
async def update_vector_db(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.update_vector_db(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# delete_vector_db
@routes.post('/api/delete_vector_db_item')
async def delete_vector_db(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.delete_vector_db(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# get_vector_db_items
@routes.post('/api/get_vector_db_items')
async def get_vector_db_items(request: Request) -> Response:
    response = ai_app_wrapper.get_vector_db_items()
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# get_vector_db_by_id
@routes.post('/api/get_vector_db_item_by_id')
async def get_vector_db_by_id(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_vector_db_item_by_id(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# get_vector_db_by_name
@routes.post('/api/get_vector_db_item_by_name')
async def get_vector_db_by_name(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_vector_db_item_by_name(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# vector_search
@routes.post('/api/vector_search')
async def vector_search(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.vector_search(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# delete_collection
@routes.post('/api/delete_collection')
async def delete_collection(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.delete_collection(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# delete_embeddings
@routes.post('/api/delete_embeddings')
async def delete_embeddings(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.delete_embeddings(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# update_embeddings
@routes.post('/api/update_embeddings')
async def update_embeddings(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.update_embeddings(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')


# get_mime_type
@routes.post('/api/get_mime_type')
async def get_mime_type(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_mime_type(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# get_sheet_names
@routes.post('/api/get_sheet_names')
async def get_sheet_names(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.get_sheet_names(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# extract_excel_sheet
@routes.post('/api/extract_excel_sheet')
async def extract_excel_sheet(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.extract_excel_sheet(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# extract_text_from_file
@routes.post('/api/extract_text_from_file')
async def extract_text_from_file(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.extract_text_from_file(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')


# extract_base64_to_text
@routes.post('/api/extract_base64_to_text')
async def extract_base64_to_text(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.extract_base64_to_text(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')


# extract_webpage
@routes.post('/api/extract_webpage')
async def extract_webpage(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.extract_webpage(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# export_to_excel
@routes.post('/api/export_to_excel')
async def export_to_excel(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.export_to_excel(request_json)
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')

# import_from_excel
@routes.post('/api/import_from_excel')
async def import_from_excel(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.import_from_excel(request_json)
    print(response)
    return web.Response(body=response, status=200, content_type='application/json')

# hello_world
@routes.post('/api/hello_world')
async def hello_world(request: Request) -> Response:
    request_json = await request.text()
    response = ai_app_wrapper.hello_world()
    logger.debug(response)
    return web.Response(body=response, status=200, content_type='application/json')


@routes.post('/api/cancel_autogen_chat')
async def cancel_autogen_chat(request: Request) -> Response:
    import json
    request_json = await request.text()
    request_data = json.loads(request_json)
    session_token = request_data.get("session_token")
    logger.debug(f"cancel_autogen_chat: {session_token}")
    # session_toknes
    logger.debug (AutoGenProps.session_tokens)
    AutoGenProps.remove_session_token(session_token)
    return web.Response(body="{}", status=200, content_type='application/json')

@sio.on('autogen_chat')
async def autogen_group_chat(sid, request_json: str):
    try:
        async for response in ai_app_wrapper.autogen_chat(request_json):
            logger.debug(f"session_token:{AutoGenProps.session_tokens}")
            await sio.emit("response", response, room=sid)

    except Exception as e:
        import traceback
        await sio.emit("error", traceback.format_exc(), room=sid)
    finally:
        logger.debug("close")
        await sio.emit("close", "close", room=sid)
        await sio.disconnect(sid)


@routes.post('/api/shutdown')
async def shutdown_server(request: Request) -> Response:
    pid = os.getpid()
    # Ctrl+CでSIGINTを送信してもらう
    os.kill(pid, 2)
    return web.Response(body="{}", status=200, content_type='application/json')

if __name__ == ('__main__'):
    # 第１引数はAPP_DATA_PATH
    if len(sys.argv) > 1:
        os.environ["APP_DATA_PATH"] = sys.argv[1]

    # APP_DATA_PATHを取得
    app_data_path = os.getenv("APP_DATA_PATH", None)
    if not app_data_path:
        raise ValueError("APP_DATA_PATH is required")

    # アプリケーション初期化
    ai_app_util.init_app()

    logging.basicConfig(level=logging.INFO)
    port = os.getenv("API_SERVER_PORT", "5000")
    logger.info(f"port={port}")

    app.add_routes(routes)
    web.run_app(app, port=int(port) )
    

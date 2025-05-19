import os, json
from typing import Any
from collections.abc import Generator, AsyncGenerator
from io import StringIO
import sys
from ai_chat_lib.db_modules import MainDB

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

# アプリケーション初期化時に呼び出される関数
def init_app() -> None:
    MainDB.init()

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> str:
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        result = {}
        try:
            # debug用
            # HTTPS_PROXY環境変数
            logger.debug(f"HTTPS_PROXY:{os.environ.get('HTTPS_PROXY')}")
            # NO_PROXY環境変数
            logger.debug(f"NO_PROXY:{os.environ.get('NO_PROXY')}")

            result = func(*args, **kwargs)
            # resultがdictでない場合は例外をスロー
            if not isinstance(result, dict):
                raise ValueError("result must be dict")
        except Exception as e:
            # エラーが発生した場合はエラーメッセージを出力
            logger.debug(e)
            import traceback
            logger.debug(traceback.format_exc())
            result["error"] = "\n".join(traceback.format_exception(type(e), e, e.__traceback__))

        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
            
        # resultにlogを追加して返す
        result["log"] = buffer.getvalue()
        # jsonを返す
        return json.dumps(result, ensure_ascii=False, indent=4)

    return wrapper

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr_async(func):
    async def wrapper(*args, **kwargs) -> str:
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        result = {}
        try:
            # debug用
            # HTTPS_PROXY環境変数
            logger.debug(f"HTTPS_PROXY:{os.environ.get('HTTPS_PROXY')}")
            # NO_PROXY環境変数
            logger.debug(f"NO_PROXY:{os.environ.get('NO_PROXY')}")

            result = await func(*args, **kwargs)
            # resultがdictでない場合は例外をスロー
            if not isinstance(result, dict):
                raise ValueError("result must be dict")
        except Exception as e:
            # エラーが発生した場合はエラーメッセージを出力
            logger.error(e)
            import traceback
            logger.error(traceback.format_exc())
            result["error"] = "\n".join(traceback.format_exception(type(e), e, e.__traceback__))

        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
            
        # resultにlogを追加して返す
        result["log"] = buffer.getvalue()
        # jsonを返す
        return json.dumps(result, ensure_ascii=False, indent=4)

    return  wrapper

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_generator_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> Generator[str, None, None]:

        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        result = None # 初期化
        for result in func(*args, **kwargs):
            try:
                # resultがdictでない場合は例外をスロー
                if not isinstance(result, dict):
                    raise ValueError("result must be dict")
                
                # strout,stderrorを元に戻す
                sys.stdout = sys.__stdout__
                sys.stderr = sys.__stderr__
                
                # resultにlogを追加して返す
                result["log"] = buffer.getvalue()
                # bufferをクリア
                buffer.truncate(0)

                json_string = json.dumps(result, ensure_ascii=False, indent=4)
                logger.debug(json_string)
                yield json_string

            except Exception as e:
                # エラーが発生した場合はエラーメッセージを出力
                import traceback
                logger.error(e)
                logger.error(traceback.format_exc())
                result = {}
                result["error"] = str(e)
            finally:
                # strout,stderrorを元に戻す
                sys.stdout = sys.__stdout__
                sys.stderr = sys.__stderr__
                # bufferをクリア
                buffer.truncate(0)


    return wrapper

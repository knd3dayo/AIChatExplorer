import logging
import os
default_log_level = logging.ERROR
log_format = "%(asctime)s - %(levelname)s - %(filename)s -  %(lineno)d - %(funcName)s - %(message)s"


def getLogger(name: str) -> logging.Logger:
    """
    Get a logger with the specified name.
    :param name: Name of the logger.
    :return: Logger object.
    """
    logger = logging.getLogger(name)
    # When the environment variable LOGLEVEL is set, use its value as the log level.
    # Otherwise, use the default log level.
    log_level = os.environ.get("LOGLEVEL", default_log_level)
    logger.setLevel(log_level)
    handler = logging.StreamHandler()
    handler.setLevel(log_level)
    formatter = logging.Formatter(log_format)
    handler.setFormatter(formatter)
    logger.addHandler(handler)
    return logger


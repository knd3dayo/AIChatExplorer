from setuptools import setup, Extension,find_packages # type: ignore

def get_requirements_from_file():
    with open("./requirements.txt") as f_in:
        requirements = f_in.read().splitlines()
    return requirements

setup(
    name = 'ai_chat_lib',
    version = '0.1.0',
    author= 'knd3dayo',
    author_email = 'knd3dayo@gmail.com',
    packages=find_packages(where='.'),
    package_dir={'ai_chat_lib': 'ai_chat_lib'},
    install_requires=get_requirements_from_file(),
)

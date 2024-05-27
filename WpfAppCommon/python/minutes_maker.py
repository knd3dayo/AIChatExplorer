import sys
sys.path.append('..')
from common.open_ai_util import OpenAIUtil
from transcribe import transcribe_util, audio_util

from pydub import AudioSegment
import tempfile
import os

from dotenv import load_dotenv

class MinutesMaker:
    def __init__(self, input_file, n_speakers=5):
            """
            Initializes the MinutesMaker object.

            Parameters:
            - input_file (str): The path to the input file (must be an mp4 file).
            - n_speakers (int): The number of speakers in the video (default is 5).
            """
            mime = magic.from_file(input_file, mime=True)
            if mime == "video/mp4":
                self.input_file = input_file
            else:
                raise ValueError("input file must be mp4 file.")
            
            load_dotenv()
            self.client = OpenAIUtil().client
            self.output_language = os.getenv("OPENAI_OUTPUT_LANGUAGE")
            self.chat_model_name = os.getenv("CHAT_MODEL_NAME")

            self.n_speakers = n_speakers
            self.transcription_json = None
            self.transcription_with_speaker = None
            self.minutes = None


    def __prepare_audio(self, file_path, tmp_dir_path):
            """
            Prepare the audio file for transcription.

            Args:
                file_path (str): The path to the input audio file.
                tmp_dir_path (str): The path to the temporary directory.

            Returns:
                list: A list of paths to the split audio files.
            """
            target_audio_file_path = audio_util.mp4tomp3(file_path, tmp_dir_path)
            split_length = 1800
            audio_files = audio_util.split_audio_file(
                target_audio_file_path, "mp3", split_length, tmp_dir_path)

            return audio_files

    def __create_transcription_with_speaker(
                self, transcriptions, audio_files, tmp_dir_path="." , n_speakers=5, split_length=1800):
            """
            Create a modified transcription with speaker information.

            Args:
                transcriptions (list): List of transcriptions.
                audio_files (list): List of audio files.
                tmp_dir_path (str, optional): Temporary directory path. Defaults to ".".
                n_speakers (int, optional): Number of speakers. Defaults to 5.
                split_length (int, optional): Length of each split in seconds. Defaults to 1800.

            Returns:
                str: Modified transcription with speaker information.
            """

            modifled_transcription = ""

            for i in range(len(transcriptions)):
                from_audio_file = audio_files[i]
                segment_list = transcriptions[i]["segments"]

                # mp3ファイルをwavファイルに変換する
                audio_file_wav = audio_util.mp3towav(from_audio_file, tmp_dir_path)

                target_audio = AudioSegment.from_file(audio_file_wav, "wav")

                modifled_transcription += transcribe_util.create_transcription_from_wav(
                    segment_list, target_audio, n_speakers, preset_time=i*split_length)

            return modifled_transcription


    def create_transcription(self, n_speakers=5):
            """
            Create a transcription for the input audio file.

            Args:
                n_speakers (int): The number of speakers in the audio file. Default is 5.

            Returns:
                None
            """
            with tempfile.TemporaryDirectory() as tmp_dir_path:
                audio_files = self.__prepare_audio(self.input_file, tmp_dir_path)
            
                if self.transcription_json is None:
                    self.transcription_json = transcribe_util.get_transcription_json(
                        self.client, audio_files)

                self.transcription_with_speaker = self.__create_transcription_with_speaker(
                    self.transcription_json, audio_files, tmp_dir_path, n_speakers
                    )
        
    def run(self):
            """
            Runs the minutes_maker process.

            If the transcription with speaker is not available, it creates the transcription.
            Then, it creates the minutes using the created transcription.

            Args:
                None

            Returns:
                None
            """
            if self.transcription_with_speaker is None:
                self.create_transcription()
            
            self.minutes = transcribe_util.create_minutes(
                self.client, self.chat_model_name,
                self.transcription_with_speaker, self.output_language)

    def create_minutes_from_transcription(self, transcription):
            """
            Creates minutes from the given transcription.

            Args:
                transcription (str): The transcription text.

            Returns:
                str: The generated minutes.
            """
            self.transcription_with_speaker = transcription
            self.minutes = transcribe_util.create_minutes(
                self.client, self.chat_model_name,
                self.transcription_with_speaker, self.output_language)
            return self.minutes
    
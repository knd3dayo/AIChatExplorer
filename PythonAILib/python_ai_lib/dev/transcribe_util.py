
from pydub import AudioSegment #type: ignore
from moviepy.editor import VideoFileClip #type: ignore
import numpy as np
import tempfile
import json
import sys
from speechbrain.inference.classifiers import EncoderClassifier #type: ignore
from sklearn.cluster import AgglomerativeClustering #type: ignore
from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_util import VectorDBProps
import torch #type: ignore

sys.path.append("..")

import os
import sys


def split_audio_file(audio_file_path, audio_format="mp3", length=1800, output_dir_path="."):

    audio_segment = AudioSegment.from_file(audio_file_path, audio_format)
    audio_files = []

    segment_list = split_audio_segment(audio_segment, length)
    if len(segment_list) == 1:
        audio_files.append(audio_file_path)
        return audio_files
    
    print(f"Splitting {audio_file_path} into {len(segment_list)} chunks...", file=sys.stderr)
    for i in range(len(segment_list)):
        chunk_path = os.path.join(output_dir_path, f"audio_chunk_{i+1}.{audio_format}")
        segment_list[i].export(chunk_path, format=audio_format)
        audio_files.append(chunk_path)
        
    return audio_files

def split_audio_segment(audio_segment, length=1800):
    audio_segments = []
    duration_seconds = audio_segment.duration_seconds
    end_time = 0
    while True:
        start_time = end_time
        end_time = start_time + (length * 1000)
        if end_time > duration_seconds * 1000:
            end_time = duration_seconds * 1000
        
        chunk = audio_segment[start_time:end_time]
        audio_segments.append(chunk)
        if end_time == duration_seconds * 1000:
            break

    return audio_segments

    
def convert_movie_to_wav(video_file_path, wave_file_path):
    # ビデオファイルを読み込む
    clip = VideoFileClip(video_file_path)
    
    # ビデオからオーディオを抽出し、wavファイルとして保存する
    clip.audio.write_audiofile(wave_file_path, 44100, 2, 2000,"pcm_s32le")


def convert_wav_to_mp3(wave_file_path, mp3_file_path, bitrate="32k"):

    wav_file = AudioSegment.from_file(wave_file_path, "wav") 
    wav_file.export(mp3_file_path, format="mp3" , bitrate=bitrate)

def convert_mp3_to_wav(mp3_file_path, wave_file_path):

    mp3_file = AudioSegment.from_file(mp3_file_path, "mp3") 
    mp3_file.export(wave_file_path, format="wav")

class Transcriber:
    
    def __init__(self, props: OpenAIProps):
        
        self.props = props
        self.client = OpenAIClient(self.props)
        

    def __get_transcription__(self , audio_file: str) -> dict:

        # OpenAIのクライアントからwhisper用のクライアントを取得する
        client = self.client.get_whisper_client()
        
        print(f"Transcribing {audio_file}...", file=sys.stderr)
        with open(audio_file, "rb") as f:
            response = client.audio.transcriptions.create(
                model=self.props.OpenAIWhisperModel,
                file=f,
                response_format="verbose_json"
        )

        return json.loads(response.model_dump_json())

    # 音声ファイル名を引数にとり、話者認識した結果の書き起こしを作成する関数
  
    def create_speaker_analyzed_transcription(self, audio_file: str, n_speakers: int, chunk_size=1800):

        # audio_fileをmp3形式に変換する
        audio_file = self.__convert_audio_file__(audio_file)    
        # audio_file(mp3)からAudioSegmentを作成する
        target_audio = AudioSegment.from_file(audio_file)
        # target_audioのdurationを取得する
        duration = target_audio.duration_seconds
    
        # duration > chunk_sizeの場合は分割する
        if duration > chunk_size:
        # 分割した音声ファイルのリストを取得する
            audio_files = split_audio_segment(target_audio, length=chunk_size)
        else:
            audio_files = [audio_file]   
            

        ###############
        # audio_file毎の処理
        ###############
        for target_audit_file in audio_files:    
            # 音声ファイルから書き起こしを取得する        
            transcription_dict = self.__get_transcription__(target_audit_file)
            print(transcription_dict, file=sys.stderr)
            # 音声ファイルからaudit_segmentを取得
            transcription_text = self.__create_transcription_from_audio__(target_audit_file, n_speakers)
            print(transcription_text, file=sys.stderr)
            
            

    def __convert_audio_file__(self, audio_file):
        # audio_fileがMP4の場合はmp3に変換する
        if audio_file.endswith(".mp4"):
            # audio_fileのbasenameを取得して、拡張子をwavにする。
            wav_file_name = os.path.basename(audio_file).replace(".mp4", ".wav")
            # 出力ファイルは一時ディレクトリにする。
            wave_file_path = os.path.join(tempfile.gettempdir(), wav_file_name)
            
            # mp4ファイルをwavファイルに変換する
            convert_movie_to_wav(audio_file, wave_file_path)
            # wavファイルをmp3ファイルに変換する
            mp3_file_name = os.path.basename(audio_file).replace(".mp4", ".mp3")
            mp3_file_path = os.path.join(tempfile.gettempdir(), mp3_file_name)
            convert_wav_to_mp3(wave_file_path, mp3_file_path)

            audio_file = mp3_file_path

            # wavファイルを削除する
            os.remove(wave_file_path)

            return audio_file
        
        elif audio_file.endswith(".wav"):
        # wavファイルをmp3ファイルに変換する
            mp3_file_name = os.path.basename(audio_file).replace(".wav", ".mp3")
            mp3_file_path = os.path.join(tempfile.gettempdir(), mp3_file_name)
            convert_wav_to_mp3(audio_file, mp3_file_path)
            audio_file = mp3_file_path
        elif audio_file.endswith(".mp3"):
            pass   
        else:
            raise Exception("Unsupported file format")    
        
        return audio_file
     
    def __create_transcription_from_audio__(self, audio_file, n_speakers, preset_time=0):
        segment = AudioSegment.from_file(audio_file)    
        print("Clustering segments by speaker similarity...", file=sys.stderr)
        transcription_list , cluster_labels = self.__cluster_segments_by_speaker_similarity__(segment, n_speakers)
        modifled_transcription = self.__create_modified_transcription__(transcription_list, preset_time)

        return modifled_transcription

    def __cluster_segments_by_speaker_similarity__(self, segment, n_speakers):
        # __create_transcription_from_audio__から呼び出される

        # Hierarchical clustering of embedding
        embeddings_list = self.__get_audio_embedding__(segment)

        # 凝集型クラスタリングを行う
        cluster = AgglomerativeClustering(n_clusters=n_speakers).fit([embeddings_list])

        transcribed_text_list = []

        for i in range(len(cluster.labels_)):
            label = cluster.labels_[i]
            start = embeddings_list[i]["start"]
            end = embeddings_list[i]["end"]
            text = embeddings_list[i]["text"]
            transcribed_text_list.append({"start": start, "end": end, "label": label, "text": text})
            print(start, end, label, text, file=sys.stderr)
        return transcribed_text_list, cluster.labels_


    def __get_audio_embedding__(self, audio_segiment: AudioSegment):
        # __cluster_segments_by_speaker_similarity__から呼び出される
        
        classifier = EncoderClassifier.from_hparams(source="speechbrain/spkrec-ecapa-voxceleb")
        # ndarrayをtensorに変換する
        tensor = torch.from_numpy(np.array(audio_segiment.get_array_of_samples()))
        embedding_list = classifier.encode_batch(tensor)
        
        return embedding_list.reshape(192,)


    def __create_transcription__(speakers_text_list):
        # どこからも呼び出されない    
        return "\n".join([f"{item['start']} {item['end']} {item['label']} {item['text']}" for item in speakers_text_list])


    def __create_modified_transcription__(self, transcription_list, preset_time=0):
        # __create_transcription_from_audio__から呼び出される    
        modifled_transcription_list = []
        for item in transcription_list:
            label = item["label"]
            label = f"Speaker:{label + 1}"
            start = "{:.3f}".format(float(item["start"]) + preset_time)
            end = "{:.3f}".format(float(item["end"]) + preset_time)

            text = item["text"]
            modifled_transcription_list.append(f"{start} {end} {label} {text}")
            print( start, end, label, text , file=sys.stderr)
        return "\n".join(modifled_transcription_list) 


if __name__ == "__main__":
    # テストコード
    import os
    from ai_app_openai_util import OpenAIProps, OpenAIClient 
    from ai_app_vector_db_util import VectorDBProps

    props = OpenAIProps.env_to_props()
    props.AzureOpenAI = False
    props.OpenAIWhisperModel = "whisper-1"
    
    audio_file = sys.argv[1]
    transcriber = Transcriber(props)
    
    transcriber.create_speaker_analyzed_transcription(audio_file, 2)
        
    

import os
import sys
from pydub import AudioSegment #type: ignore
from moviepy.editor import VideoFileClip #type: ignore


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

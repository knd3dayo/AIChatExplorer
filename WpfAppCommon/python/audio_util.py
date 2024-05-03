
from pydub import AudioSegment
import os
import sys

def split_audio_file(audio_file_path, audio_format="mp3", length=1800, output_dir_path="."):
    """
    Split an audio file into smaller chunks.

    Parameters:
    audio_file_path (str): The path to the audio file.
    audio_format (str, optional): The format of the audio file. Defaults to "mp3".
    length (int, optional): The length of each chunk in seconds. Defaults to 1800.

    Returns:
    list: A list of paths to the generated audio chunks.
    """
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
    """
    Split an audio segment into smaller chunks.

    Parameters:
    audio_segment (AudioSegment): The audio segment to split.
    length (int, optional): The length of each chunk in seconds. Defaults to 1800.

    Returns:
    list: A list of AudioSegment objects representing the generated audio chunks.
    """
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

    
def mp4tomp3(file_path, output_dir_path="."):
    """
    Convert an MP4 audio file to MP3 format.

    Args:
        file_path (str): The path to the input MP4 file.
        output_dir_path (str, optional): The directory path to save the output MP3 file. Defaults to the current directory.

    Returns:
        str: The path to the output MP3 file.
    """

    import ffmpeg
    input_file = ffmpeg.input(file_path)
    basename = os.path.basename(file_path).split(".")[0]
    output_file_path = os.path.join(output_dir_path, f'{basename}.mp3')
    output_file = ffmpeg.output(
        input_file, output_file_path, ac=1, ar='16k', ab='128k', loglevel='error')
    ffmpeg.run(output_file)
    return output_file_path

def wavtomp3(file_path, output_dir_path="."):
    """
    Convert a WAV file to MP3 format.

    Args:
        file_path (str): The path to the input WAV file.
        output_dir_path (str, optional): The directory path to save the output MP3 file. Defaults to the current directory.

    Returns:
        str: The path to the output MP3 file.
    """

    basename = os.path.basename(file_path).split(".")[0]

    output_file_path = os.path.join(output_dir_path, f'{basename}.mp3')
    wav_file = AudioSegment.from_file(file_path, "wav") 
    wav_file.export(output_file_path, format="mp3")
    return output_file_path

def mp3towav(file_path, output_dir_path="."):
    """
    Convert an MP3 audio file to WAV format.

    Args:
        file_path (str): The path to the MP3 audio file.
        output_dir_path (str, optional): The directory path to save the converted WAV file. 
        Defaults to the current directory.

    Returns:
        str: The path to the converted WAV file.
    """
    basename = os.path.basename(file_path).split(".")[0]

    output_file_path = os.path.join(output_dir_path, f'{basename}.wav')
    mp3_file = AudioSegment.from_file(file_path, "mp3") 
    mp3_file.export(output_file_path, format="wav")
    return output_file_path

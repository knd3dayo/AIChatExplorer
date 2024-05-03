
from pydub import AudioSegment
import numpy as np
import torch
import json
import sys
sys.path.append("..")

from openai import OpenAI, AzureOpenAI

from openai.types.audio.transcription import Transcription

def get_transcription_json(client:OpenAI|AzureOpenAI , audio_files):
    """
    Get the transcription of audio files in JSON format.

    Args:
        audio_files (list): List of audio file paths.
        save_json (bool, optional): Whether to save the transcription as JSON files. Defaults to False.
        load_json (bool, optional): Whether to load existing JSON files instead of transcribing again. Defaults to False.

    Returns:
        list: List of transcription JSON objects.
    """
    transcripts = []

    for audio_file in audio_files:
        print(f"Transcribing {audio_file}...", file=sys.stderr)
        with open(audio_file, "rb") as f:
            transcript : Transcription = client.audio.transcriptions.create(
            model="whisper-1",
            file=f,
            response_format="verbose_json"
        )
        transcripts.append(json.loads(transcript.model_dump_json()))

    return transcripts


def cluster_segments_by_speaker_similarity(segment_list, target_audio, n_speakers):
    """
    Clusters the segments of a meeting transcription based on speaker similarity.

    Args:
        segment_list (list): A list of dictionaries containing segment information.
            Each dictionary should have the keys "start", "end", and "text".
        target_audio (AudioSegment): The audio of the meeting.
        n_clusters (int, optional): The number of speaker clusters to create. Defaults to 5.

    Returns:
        tuple: A tuple containing two elements:
            - speakers_text_list (list): A list of dictionaries containing the clustered segments.
                Each dictionary has the keys "start", "end", "label", and "text".
            - cluster.labels_ (ndarray): The labels assigned to each segment based on clustering.
    """

    # Hierarchical clustering of embeddings_list
    from sklearn.cluster import AgglomerativeClustering
    embeddings_list = get_audio_embeddings(segment_list, target_audio)

    cluster = AgglomerativeClustering(n_clusters=n_speakers).fit(embeddings_list)
    # cluster = AgglomerativeClustering(n_clusters=None, distance_threshold=0.5).fit(embeddings_list)
    # print(cluster.labels_)

    transcribed_text_list = []

    for i in range(len(cluster.labels_)):
        label = cluster.labels_[i]
        start = segment_list[i]["start"]
        end = segment_list[i]["end"]
        text = segment_list[i]["text"]
        transcribed_text_list.append({"start": start, "end": end, "label": label, "text": text})
        print(start, end, label, text, file=sys.stderr)
    return transcribed_text_list, cluster.labels_

def get_audio_embeddings(segment_list, target_audio):
    """
    Extracts audio embeddings from a target audio using a pretrained encoder classifier.

    Args:
        segment_list (list): A list of segments containing start, end, and text information.
        target_audio (AudioSegment): The target audio to extract embeddings from.

    Returns:
        list: A list of audio embeddings.

    Raises:
        None
    """
    from speechbrain.pretrained import EncoderClassifier
    classifier = EncoderClassifier.from_hparams(source="speechbrain/spkrec-ecapa-voxceleb")
    embeddings_list = []
    for item in segment_list:
        start = item["start"]
        end = item["end"]
        print(start, end, item["text"], file=sys.stderr)
        clip : AudioSegment = target_audio[start * 1000:end * 1000]
        print('channel==>', clip.channels, file=sys.stderr)
        print('frame_rate==>', clip.frame_rate, file=sys.stderr)
        print('duration==>', clip.duration_seconds, file=sys.stderr)
        if clip.duration_seconds == 0:
            continue
        # ndarrayをtensorに変換する
        tensor = torch.from_numpy(np.array(clip.get_array_of_samples()))
        embeddings = classifier.encode_batch(tensor)
        embeddings_list.append(embeddings.reshape(192,))
    return embeddings_list

def create_minutes(client:OpenAI|AzureOpenAI, chat_model_name, transcription, output_language):
    """
    Create minutes of a meeting using OpenAI or AzureOpenAI API.

    Args:
        client (OpenAI|AzureOpenAI): The client object for accessing the OpenAI or AzureOpenAI API.
        chat_model_name (str): The name of the chat model to use for generating the minutes.
        transcription (str): The transcription of the meeting.
        output_language (str): The language in which the minutes should be output.

    Returns:
        str: The generated minutes of the meeting.
    """
    
    # speackers_text_listの内容をopenai apiで会議の議事録として成形する。
    message_template = f"""
    Below are the minutes of the meeting. The start time, end time, speaker, and content of the speech are recorded.
    Please output in the language specified in Output language.
    1 Please summarize the content of each speaker's statement.
    2 Please summarize your decisions.
    3 Please summarize your homework items.
    ---
    {transcription}
    ---
    Output language: {output_language}
    Output format: 
    1. Summary of each speaker's speech
    [Summary of each speaker's speech]
    2. Summary of decisions
    [Summary of decisions]
    3. Summary of homework items
    [Summary of homework items]
    ---

    """
    messages=[
        {"role": "system", "content": "You are a helpful assistant."},
        {"role": "user", "content": f"{message_template}"},
    ]
    response = client.chat.completions.create(
        model = chat_model_name,
        messages=messages,
    )
    return response.choices[0].message.content


def create_transcription(speakers_text_list):
    """
    Create a message body by converting each element of speakers_text_list into a string of the format 'start end label text',
    and joining them with newline characters.

    Args:
        speakers_text_list (list): A list of dictionaries containing 'start', 'end', 'label', and 'text' keys.

    Returns:
        str: The message body created by joining the formatted strings with newline characters.
    """
    return "\n".join([f"{item['start']} {item['end']} {item['label']} {item['text']}" for item in speakers_text_list])


def create_transcription_from_wav(segment_list, target_audio, n_speakers, preset_time=0):
    """
    Create meeting minutes from a list of audio segments and a target audio file.

    Args:
        segment_list (list): A list of audio segments.
        target_audio (str): The path to the target audio file.

    Returns:
        tuple: A tuple containing the modified transcription list and the response from creating minutes.
    """
    print("Clustering segments by speaker similarity...", file=sys.stderr)
    transcription_list , cluster_labels = cluster_segments_by_speaker_similarity(segment_list, target_audio, n_speakers)
    modifled_transcription = create_modified_transcription(transcription_list, preset_time)

    return modifled_transcription

def create_modified_transcription(transcription_list, preset_time=0):
    """
    Create a modified transcription list by adding speaker labels and formatting the start, end, and text values.

    Args:
        transcription_list (list): A list of dictionaries representing transcriptions.

    Returns:
        list: A modified transcription list with speaker labels and formatted values.
    """
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

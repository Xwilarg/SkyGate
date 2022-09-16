using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace SkyGate.Music
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _player;

        public SongData CurrentSong { get; set; }

        /// <summary>
        /// Is the Audio Source currently playing
        /// </summary>
        public bool IsPlaying => _player.isPlaying;

        public float SongDuration => _player.clip.length;
        public float TimeElapsed => _player.time;

        /// <summary>
        /// Is the extension given in parameter allowed to be played in the Audio Source
        /// </summary>
        public bool IsExtensionAllowed(string ext)
            => _extensionMapping.ContainsKey(ext);

        private readonly Dictionary<string, AudioType> _extensionMapping = new()
        {
            { "mp3", AudioType.MPEG },
            { "wav", AudioType.WAV }
        };

        private void Awake()
        {
            Instance = this;
        }

        public void Play()
        {
            if (!_player.isPlaying)
            {
                _player.UnPause();
            }
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
        }

        public void SaveSong()
        {
            CurrentSong.Save();
        }

        public IEnumerator LoadSong(SongData songData)
        {
            CurrentSong = songData;
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{songData.AudioClipPath}", _extensionMapping[songData.MusicFileExtension]);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                _player.clip = DownloadHandlerAudioClip.GetContent(req);
                _player.Play();
            }
            else
            {
                Debug.LogError($"Failed to fetch file: {req.responseCode}");
            }
        }
    }
}

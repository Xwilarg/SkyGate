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

        public SongData CurrentSong { get; private set; }

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

        private void Update()
        {
            if (!_player.isPlaying && _player.clip != null && _player.clip.loadState == AudioDataLoadState.Loaded)
            {
                _player.Play();
            }
        }

        public void Play()
        {
            _player.Play();
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

        public void LoadSong(SongData data)
        {
            CurrentSong = data;
        }

        public IEnumerator LoadSong(FileInfo file)
        {
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{file.FullName}", _extensionMapping[file.Extension[1..]]);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                _player.clip = DownloadHandlerAudioClip.GetContent(req);
                CurrentSong = SongData.FromFileInfo(file);
            }
            else
            {
                Debug.LogError($"Failed to fetch file: {req.responseCode}");
            }
        }
    }
}

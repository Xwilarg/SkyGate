using SkyGate.Game;
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

        private SongData _currentSong;

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
                _player.Play();
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

        public void UpdateBPM(int newValue)
        {
            _currentSong.BPM = newValue;
            Stop();
            GridManager.Instance.ResetGrid();
        }

        public int BPM => _currentSong.BPM;

        /// <summary>
        /// Save song data to persistent storage
        /// </summary>
        public void SaveSong()
        {
            _currentSong.Save();
        }

        /// <summary>
        /// Load a song into the game and start playing it
        /// </summary>
        public IEnumerator LoadSong(SongData songData)
        {
            _currentSong = songData;
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{songData.AudioClipPath}", _extensionMapping[songData.MusicFileExtension]);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                _player.clip = DownloadHandlerAudioClip.GetContent(req);
                GridManager.Instance.ResetGrid();
                _player.Play();
            }
            else
            {
                Debug.LogError($"Failed to fetch file: {req.responseCode}");
            }
        }
    }
}

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

        [SerializeField]
        private GameObject _songDataCategory;

        [SerializeField]
        private TMP_InputField _songName;
        [SerializeField]
        private TMP_InputField _bpm;

        private SongData _currentSong;

        /// <summary>
        /// Is the Audio Source currently playing
        /// </summary>
        public bool IsPlaying => _player.isPlaying;

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
            _songDataCategory.SetActive(false);
        }

        private void Update()
        {
            if (!_player.isPlaying && _player.clip != null && _player.clip.loadState == AudioDataLoadState.Loaded)
            {
                _player.Play();
            }
        }

        public void SaveSong()
        {
            _currentSong.Save();
        }

        public void LoadSong(SongData data)
        {
            _currentSong = data;
            LoadMetadata();
        }

        public void LoadSong(FileInfo file)
        {
            StartCoroutine(LoadSongInternal(file));
        }

        private IEnumerator LoadSongInternal(FileInfo file)
        {
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{file.FullName}", _extensionMapping[file.Extension[1..]]);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                _player.clip = DownloadHandlerAudioClip.GetContent(req);
                _currentSong = SongData.FromFileInfo(file);
                LoadMetadata();
            }
            else
            {
                Debug.LogError($"Failed to fetch file: {req.responseCode}");
            }
        }

        private void LoadMetadata()
        {
            _songDataCategory.SetActive(true);
            _songName.text = _currentSong.Name;
            _bpm.text = _currentSong.BPM.ToString();
        }
    }
}

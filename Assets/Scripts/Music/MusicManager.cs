using SkyGate.Game;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _player.Play();
        }

        public void Pause()
        {
            _player.Pause();
        }

        public void Stop()
        {
            _player.Stop();
            GridManager.Instance.ResetGrid();
        }

        public void Move(float value)
        {
            var nextValue = _player.time + value;
            if (nextValue < 0f)
            {
                SetValue(0f);
            }
            else if (nextValue > SongDuration)
            {
                SetValue(SongDuration);
            }
            else
            {
                SetValue(nextValue);
            }
        }

        public void SetValue(float value)
        {
            _player.time = value;
            GridManager.Instance.ResetGrid();
        }

        public void UpdateBPM(int newValue)
        {
            _currentSong.BPM = newValue;
            Stop();
        }

        public void ToggleNote(NoteData note)
        {
            if (_currentSong.Notes.Any(x => x.Y == note.Y && x.Line == note.Line))
            {
                _currentSong.Notes.RemoveAll(x => x.Y == note.Y && x.Line == note.Line);
                Debug.Log($"Removed note at {note.Line} ; {note.Y}");
            }
            else
            {
                _currentSong.Notes.Add(note);
                Debug.Log($"Added note at {note.Line} ; {note.Y}");
            }
            GridManager.Instance.ResetGrid();
        }

        public IReadOnlyList<NoteData> Notes => _currentSong.Notes.AsReadOnly();

        public int BPM => _currentSong.BPM;

        public bool IsSongSet => _player.clip != null;

        /// <summary>
        /// Save song data to persistent storage
        /// </summary>
        public void SaveSong()
        {
            _currentSong.Save();
        }

        public void EditMetadata(string name, string musicAuthor, string mapAuthor)
        {
            _currentSong.Name = name;
            _currentSong.MusicAuthor = musicAuthor;
            _currentSong.MapAuthor = mapAuthor;
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

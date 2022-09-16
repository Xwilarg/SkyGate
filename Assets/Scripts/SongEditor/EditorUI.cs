using SkyGate.Game;
using SkyGate.Music;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkyGate.SongEditor
{
    public class EditorUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _filePath;

        [SerializeField]
        private GameObject _songDataCategory;

        [SerializeField]
        private TMP_InputField _songName;
        [SerializeField]
        private TMP_InputField _bpm;
        [SerializeField]
        private TMP_Text _duration;

        private void Awake()
        {
            _songDataCategory.SetActive(false);
        }

        private string AddZero(int value)
            => (value < 10 ? "0" : "") + value;

        private string FormatTime(float time)
            => $"{(int)(time / 60f)}:{AddZero((int)(time % 60))}";

        private void Update()
        {
            if (MusicManager.Instance.IsPlaying)
            {
                _duration.text = $"{FormatTime(MusicManager.Instance.TimeElapsed)} / {FormatTime(MusicManager.Instance.SongDuration)}";
            }
        }

        public void LoadSong()
        {
            var path = _filePath.text;
            if (File.Exists(path))
            {
                var file = new FileInfo(path);
                var ext = file.Extension[1..];
                if (ext == "bin") // Binary file, contains data about a song
                {
                    StartCoroutine(LoadSongInternal(SongData.FromGameFile(file)));
                }
                else if (MusicManager.Instance.IsExtensionAllowed(ext)) // Path to a music file, create a new song project
                {
                    StartCoroutine(LoadSongInternal(SongData.FromMusicFile(file)));
                }
                else
                {
                    Debug.LogWarning($"Unknown file of type {ext}");
                }
            }
            else
            {
                Debug.LogWarning($"Failed to load {path}");
            }
        }

        private IEnumerator LoadSongInternal(SongData songData)
        {
            yield return MusicManager.Instance.LoadSong(songData);
            LoadMetadata(songData);
        }

        public void SaveSong()
        {
            MusicManager.Instance.SaveSong();
        }

        private void LoadMetadata(SongData song)
        {
            _songDataCategory.SetActive(true);
            _songName.text = song.Name;
            _bpm.text = song.BPM.ToString();
        }

        public void OnBPMChange(string fieldValue)
        {
            var newValue = int.Parse(fieldValue);
            if (newValue > 0)
            {
                MusicManager.Instance.UpdateBPM(newValue);
            }
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                var mousePos = Mouse.current.position.ReadValue();
                Debug.Log($"Line pressed: {PlayerController.Instance.GetLine(mousePos.x)}");
            }
        }
    }
}

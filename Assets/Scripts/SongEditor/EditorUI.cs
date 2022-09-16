using SkyGate.Music;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;

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

        private void Update()
        {
            if (MusicManager.Instance.CurrentSong != null)
            {
                _duration.text = $"{MusicManager.Instance.TimeElapsed:0.00} / {MusicManager.Instance.SongDuration:0.00}";
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
                    MusicManager.Instance.LoadSong(SongData.FromFile(path));
                    LoadMetadata(MusicManager.Instance.CurrentSong);
                }
                else if (MusicManager.Instance.IsExtensionAllowed(ext)) // Path to a music file, create a new song project
                {
                    StartCoroutine(LoadSongInternal(file));
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

        private IEnumerator LoadSongInternal(FileInfo file)
        {
            yield return MusicManager.Instance.LoadSong(file);
            LoadMetadata(MusicManager.Instance.CurrentSong);
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
    }
}

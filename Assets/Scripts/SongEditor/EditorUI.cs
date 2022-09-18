using SkyGate.Game;
using SkyGate.Music;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        private TMP_InputField _songAuthor;
        [SerializeField]
        private TMP_InputField _mapAuthor;
        [SerializeField]
        private TMP_InputField _bpm;
        [SerializeField]
        private TMP_Text _duration;

        [SerializeField]
        private Scrollbar _progression;

        [SerializeField]
        private Transform _fileExplorerContainer;
        [SerializeField]
        private GameObject _fileExplorerPrefab;

        private void Awake()
        {
            _songDataCategory.SetActive(false);

            if (Directory.Exists($"{Application.persistentDataPath}/Songs"))
            {
                foreach (var folder in Directory.GetDirectories($"{Application.persistentDataPath}/Songs"))
                {
                    var button = Instantiate(_fileExplorerPrefab, _fileExplorerContainer);
                    var dataPath = $"{folder}/data.bin";
                    var song = SongData.FromGameFile(new FileInfo(dataPath));
                    button.GetComponentInChildren<TMP_Text>().text = $"{song.Name}\nBy {song.MusicAuthor}";
                    button.GetComponent<Button>().onClick.AddListener(new(() =>
                    {
                        LoadSongFromFile(dataPath);
                    }));
                }
            }
        }

        private string AddZero(int value)
            => (value < 10 ? "0" : "") + value;

        private string FormatTime(float time)
            => $"{(int)(time / 60f)}:{AddZero((int)(time % 60))}";

        private void Update()
        {
            if (MusicManager.Instance.IsSongSet)
            {
                _duration.text = $"{FormatTime(MusicManager.Instance.TimeElapsed)} / {FormatTime(MusicManager.Instance.SongDuration)}";
                _progression.value = MusicManager.Instance.TimeElapsed / MusicManager.Instance.SongDuration;
            }
        }

        public void LoadSong()
        {
            LoadSongFromFile(_filePath.text);
            
        }

        private void LoadSongFromFile(string path)
        {
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
            MusicManager.Instance.EditMetadata(_songName.text, _songAuthor.text, _mapAuthor.text);
            MusicManager.Instance.SaveSong();
        }

        private void LoadMetadata(SongData song)
        {
            _songDataCategory.SetActive(true);
            _songName.text = song.Name;
            _songAuthor.text = song.MusicAuthor;
            _mapAuthor.text = song.MapAuthor;
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
            if (value.performed && MusicManager.Instance.IsSongSet)
            {
                var mousePos = Mouse.current.position.ReadValue();
                var line = GridManager.Instance.GetLine(mousePos.x);
                if (line != -1)
                {
                    var y = GridManager.Instance.GetGridIndex(mousePos.y);

                    MusicManager.Instance.ToggleNote(new NoteData(line, y));
                }
            }
        }

        public void OnPause(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (MusicManager.Instance.IsPlaying)
                {
                    MusicManager.Instance.Pause();
                }
                else
                {
                    MusicManager.Instance.Play();
                }
            }
        }

        public void OnBackward(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                MusicManager.Instance.Move(-1f);
            }
        }

        public void OnForeward(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                MusicManager.Instance.Move(1f);
            }
        }
    }
}

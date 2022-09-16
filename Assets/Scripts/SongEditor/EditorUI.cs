using SkyGate.Music;
using System.IO;
using TMPro;
using UnityEngine;

namespace SkyGate.SongEditor
{
    public class EditorUI : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _filePath;

        public void LoadSong()
        {
            var path = _filePath.text;
            if (File.Exists(path))
            {
                var file = new FileInfo(path);
                var ext = file.Extension[1..];
                if (ext == "bin")
                {
                    MusicManager.Instance.LoadSong(SongData.FromFile(path));
                }
                else if (MusicManager.Instance.IsExtensionAllowed(ext))
                {
                    MusicManager.Instance.LoadSong(file);
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

        public void SaveSong()
        {
            MusicManager.Instance.SaveSong();
        }
    }
}

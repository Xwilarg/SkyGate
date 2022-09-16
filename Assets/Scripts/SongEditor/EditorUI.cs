using SkyGate.Game;
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
                MusicManager.Instance.LoadSong(path);
            }
            else
            {
                Debug.LogWarning($"Failed to load {path}");
            }
        }
    }
}

using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SkyGate.Game
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField]
        private AudioSource _player;

        private void Awake()
        {
            Instance = this;
        }

        public void LoadSong(string path)
        {
            StartCoroutine(LoadSongInternal(path));
        }

        private IEnumerator LoadSongInternal(string path)
        {
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", AudioType.AUDIOQUEUE);
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
                Debug.Log("Song loaded");
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

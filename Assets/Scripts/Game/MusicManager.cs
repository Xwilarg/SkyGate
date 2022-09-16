﻿using System.Collections;
using System.IO;
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
            using UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip($"file://{path}", Path.GetExtension(path)[1..] switch
            {
                "mp3" => AudioType.MPEG,
                "wav" => AudioType.WAV,
                _=> throw new System.Exception($"Invalid format {Path.GetExtension(path)[1..]}")
            });
            yield return req.SendWebRequest();
            if (req.responseCode == 200)
            {
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
using SkyGate.Music;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkyGate.Game
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeReference]
        private GameObject _horizontalLinePrefab;

        [SerializeField]
        private Transform _gridContainer;

        private readonly List<Transform> _horizontalLines = new();

        private void Awake()
        {
            Instance = this;
        }

        public void ResetGrid()
        {
            foreach (var line in _horizontalLines)
            {
                Destroy(line.gameObject);
            }
            _horizontalLines.Clear();
            for (int i = 0; i < (Screen.height / MusicManager.Instance.BPM) + MusicManager.Instance.BPM; i++)
            {
                var go = Instantiate(_horizontalLinePrefab, _gridContainer);
                ((RectTransform)go.transform).anchoredPosition = new Vector2(0f, i * MusicManager.Instance.BPM);
                _horizontalLines.Add(go.transform);
            }
        }

        private void Update()
        {
            if (MusicManager.Instance.IsPlaying)
            {
                foreach (var line in _horizontalLines)
                {
                    var rTransform = (RectTransform)line;
                    rTransform.anchoredPosition = new Vector2(0f, rTransform.anchoredPosition.y - (Time.deltaTime * MusicManager.Instance.BPM));
                    if (rTransform.anchoredPosition.y < 0f)
                    {
                        rTransform.anchoredPosition = new(0f, _horizontalLines.Max(x => ((RectTransform)x).anchoredPosition.y) + MusicManager.Instance.BPM);
                    }
                }
            }
        }
    }
}

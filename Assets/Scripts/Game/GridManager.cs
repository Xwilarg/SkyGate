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
        private GameObject _horizontalLinePrefab, _lightHorizontalLinePrefab;

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

                for (int j = 0; j < 3; j++)
                {
                    var goLight = Instantiate(_lightHorizontalLinePrefab, _gridContainer);
                    ((RectTransform)goLight.transform).anchoredPosition = new Vector2(0f, (i * MusicManager.Instance.BPM) + ((j + 1) * MusicManager.Instance.BPM / 4f));
                    _horizontalLines.Add(goLight.transform);
                }
            }
        }

        private void Update()
        {
            if (MusicManager.Instance.IsPlaying)
            {
                for (int i = 0; i < _horizontalLines.Count; i++)
                {
                    var previous = i == 0 ? _horizontalLines.Last() : _horizontalLines[i - 1];
                    var rTransform = (RectTransform)_horizontalLines[i];
                    rTransform.anchoredPosition = new Vector2(0f, rTransform.anchoredPosition.y - (Time.deltaTime * MusicManager.Instance.BPM));
                    if (rTransform.anchoredPosition.y < 0f)
                    {
                        rTransform.anchoredPosition = new(0f, ((RectTransform)previous.transform).anchoredPosition.y + MusicManager.Instance.BPM / 4f);
                    }
                }
            }
        }

        public float GetGridIndex(float y)
        {
            return Mathf.Floor(y / MusicManager.Instance.BPM * 4f) / 4f;
        }
    }
}

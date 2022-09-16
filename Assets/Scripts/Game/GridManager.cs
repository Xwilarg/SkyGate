using SkyGate.Music;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkyGate.Game
{
    public class GridManager : MonoBehaviour
    {
        [SerializeReference]
        private GameObject _horizontalLinePrefab;

        [SerializeField]
        private Transform _gridContainer;

        private readonly List<Transform> _horizontalLines = new();

        private const float _scrollingSpeed = 200f;
        private const float _padding = 100f;

        private void Start()
        {
            for (int i = 0; i < (Screen.height / _padding) + _padding; i++)
            {
                var go = Instantiate(_horizontalLinePrefab, _gridContainer);
                ((RectTransform)go.transform).anchoredPosition = new Vector2(0f, i * _padding);
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
                    rTransform.anchoredPosition = new Vector2(0f, rTransform.anchoredPosition.y - (Time.deltaTime * _scrollingSpeed));
                    if (rTransform.anchoredPosition.y < 0f)
                    {
                        rTransform.anchoredPosition = new(0f, _horizontalLines.Max(x => ((RectTransform)x).anchoredPosition.y) + _padding);
                    }
                }
            }
        }
    }
}

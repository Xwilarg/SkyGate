using System.Collections.Generic;
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

        private void Start()
        {
            var padding = 100;
            for (int i = 0; i < (Screen.height / padding) + padding; i++)
            {
                var go = Instantiate(_horizontalLinePrefab, _gridContainer);
                ((RectTransform)go.transform).anchoredPosition = new Vector2(0f, i * padding);
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
                        rTransform.anchoredPosition = new(0f, rTransform.anchoredPosition.y + Screen.height);
                    }
                }
            }
        }
    }
}

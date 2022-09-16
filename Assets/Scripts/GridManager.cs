using UnityEngine;

namespace SkyGate
{
    public class GridManager : MonoBehaviour
    {
        [SerializeReference]
        private GameObject _horizontalLinePrefab;

        [SerializeField]
        private Transform _gridContainer;

        private void Start()
        {
            var padding = 100;
            for (int i = 0; i < (Screen.height / padding) + padding; i++)
            {
                var go = Instantiate(_horizontalLinePrefab, _gridContainer);
                ((RectTransform)go.transform).anchoredPosition = new Vector2(0f, i * padding);
            }
        }
    }
}

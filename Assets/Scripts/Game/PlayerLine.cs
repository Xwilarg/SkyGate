using UnityEngine;
using UnityEngine.UI;

namespace SkyGate.Game
{
    public class PlayerLine : MonoBehaviour
    {
        [SerializeField]
        private Image _hitMark;

        public float YPos { private set; get; }

        private void Awake()
        {
            YPos = ((RectTransform)transform).anchoredPosition.y;
        }

        public void ShowMark(bool value)
        {
            _hitMark.color = value ? Color.green : Color.white;
        }
    }
}
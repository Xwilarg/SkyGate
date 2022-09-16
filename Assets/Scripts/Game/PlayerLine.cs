using UnityEngine;
using UnityEngine.UI;

namespace SkyGate.Game
{
    public class PlayerLine : MonoBehaviour
    {
        [SerializeField]
        private Image _hitMark;

        public void ShowMark(bool value)
        {
            _hitMark.color = value ? Color.green : Color.white;
        }
    }
}
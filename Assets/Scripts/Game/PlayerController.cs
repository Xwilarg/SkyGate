using UnityEngine;
using UnityEngine.InputSystem;

namespace SkyGate.Game
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [SerializeField]
        private PlayerLine[] _lines;

        private void Awake()
        {
            Instance = this;
        }

        public int GetLine(float x)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                var line = _lines[i];
                var rTransform = (RectTransform)line.transform;
                var demSize = rTransform.sizeDelta.x / 2f;
                if (x >= rTransform.position.x - demSize && x <= rTransform.position.x + demSize)
                {
                    return i;
                }
            }
            return -1;
        }

        private void PressLine(InputAction.CallbackContext value, int id)
        {
            if (value.phase == InputActionPhase.Started)
            {
                _lines[id].ShowMark(true);
            }
            else if (value.phase == InputActionPhase.Canceled)
            {
                _lines[id].ShowMark(false);
            }
        }

        public void OnFirstLine(InputAction.CallbackContext value) => PressLine(value, 0);
        public void OnSecondLine(InputAction.CallbackContext value) => PressLine(value, 1);
        public void OnThirdLine(InputAction.CallbackContext value) => PressLine(value, 2);
        public void OnFourthLine(InputAction.CallbackContext value) => PressLine(value, 3);
        public void OnFifthLine(InputAction.CallbackContext value) => PressLine(value, 4);
        public void OnSixthLine(InputAction.CallbackContext value) => PressLine(value, 5);
    }
}

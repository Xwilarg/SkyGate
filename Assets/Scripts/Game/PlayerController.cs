using UnityEngine;
using UnityEngine.InputSystem;

namespace SkyGate.Game
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerLine[] _lines;

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

        public void OnFirstLine(InputAction.CallbackContext value)
        {
                PressLine(value, 0);
        }
    }
}

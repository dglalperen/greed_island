using UnityEngine;
using UnityEngine.InputSystem;

namespace GreedIsland.Camera
{
    public sealed class GameplayCursorLock : MonoBehaviour
    {
        [SerializeField] private bool lockCursorOnEnable = true;
        [SerializeField] private bool allowEscapeUnlock = true;
        [SerializeField] private bool allowLeftClickRelock = true;

        private void OnEnable()
        {
            if (lockCursorOnEnable)
            {
                SetCursorLock(true);
            }
        }

        private void OnDisable()
        {
            SetCursorLock(false);
        }

        private void Update()
        {
            if (allowEscapeUnlock && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetCursorLock(false);
                return;
            }

            if (allowLeftClickRelock &&
                Cursor.lockState != CursorLockMode.Locked &&
                Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                SetCursorLock(true);
            }
        }

        private static void SetCursorLock(bool shouldLock)
        {
            Cursor.lockState = shouldLock ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !shouldLock;
        }
    }
}

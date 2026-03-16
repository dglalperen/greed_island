using UnityEngine;
using UnityEngine.InputSystem;

namespace GreedIsland.UI
{
    public sealed class QuickInstructionsPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Key toggleKey = Key.F1;
        [SerializeField] private bool visible = true;

        private void Start()
        {
            ApplyVisibility();
        }

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current[toggleKey].wasPressedThisFrame)
            {
                return;
            }

            visible = !visible;
            ApplyVisibility();
        }

        public void Configure(GameObject root, bool startVisible)
        {
            panelRoot = root;
            visible = startVisible;
            ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            if (panelRoot != null)
            {
                panelRoot.SetActive(visible);
            }
        }
    }
}

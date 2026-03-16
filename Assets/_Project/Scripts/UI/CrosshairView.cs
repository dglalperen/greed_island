using UnityEngine;
using UnityEngine.UI;

namespace GreedIsland.UI
{
    public sealed class CrosshairView : MonoBehaviour
    {
        [SerializeField] private Graphic crosshairGraphic;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color focusedColor = Color.yellow;

        public void SetFocused(bool focused)
        {
            if (crosshairGraphic == null)
            {
                return;
            }

            crosshairGraphic.color = focused ? focusedColor : normalColor;
        }
    }
}

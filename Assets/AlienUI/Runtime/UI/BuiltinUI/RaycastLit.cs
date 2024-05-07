using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements.ToolsScript
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RaycastLit : Graphic
    {
        public bool EnableColor;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (!EnableColor)
                vh.Clear();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements.ToolsScript
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RaycastLit : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }
    }
}

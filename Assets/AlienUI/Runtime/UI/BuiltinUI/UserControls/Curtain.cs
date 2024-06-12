using UnityEngine;

namespace AlienUI.UIElements
{
    public class Curtain : UserControl
    {
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Curtain");

        protected override void OnInitialized()
        {
            OnDrag += Curtain_OnDrag;
        }

        private void Curtain_OnDrag(object sender, Events.OnDragEvent e)
        {
            var tempRoot = m_templateInstance.TemplateRoot.Get(m_templateInstance.Document) as VisualElement;
            var delta = new Vector2(0, e.EvtData.delta.y / Canvas.scaleFactor);
            tempRoot.Offset += delta;
        }
    }
}
using AlienUI.Models.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "ve")]
    public class VisualElement : UIElement
    {
        protected override void OnInitialized() { }

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2(100, 100);
        }
    }
}

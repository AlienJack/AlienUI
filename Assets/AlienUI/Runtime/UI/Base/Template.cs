using AlienUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Template : UIElement
    {
        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(string), typeof(Template), string.Empty);


        protected override void OnInitialized()
        {
            this.Horizontal = eHorizontalAlign.Stretch;
            this.Vertical = eVerticalAlign.Stretch;
        }

        protected override Vector2 CalcDesireSize()
        {
            return default;
        }
    }
}

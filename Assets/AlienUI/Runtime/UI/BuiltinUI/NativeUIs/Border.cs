using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Border : UIElement
    {
        protected override void OnInitialized()
        {
            throw new System.NotImplementedException();
        }

        protected override Vector2 CalcDesireSize()
        {
            return new Vector2(100,100);
        }
    }
}

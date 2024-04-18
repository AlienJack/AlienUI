using AlienUI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements.Containers
{
    public abstract class Container : UIElement
    {
        protected override void OnInitialized()
        {
            m_childRoot.gameObject.AddComponent<RectMask2D>();
        }
    }
}

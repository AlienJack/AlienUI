using AlienUI.Models;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class ItemPresent<T> : UIElement
    {
        public IList<T> SourceData { get; set; }
    }
}

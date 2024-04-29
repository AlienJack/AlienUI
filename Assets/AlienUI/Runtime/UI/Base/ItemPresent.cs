using System.Collections.Generic;

namespace AlienUI.UIElements
{
    public abstract class ItemPresent<T> : UIElement
    {
        public IList<T> SourceData { get; set; }
    }
}

using AlienUI.Core.Resources;
using AlienUI.Core.Triggers;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Editors
{
    public abstract class ElementEditor
    {
        public abstract Type AdapterType { get; }

        public abstract void Draw(UIElement host, AmlNodeElement element);
    }
    public abstract class ElementExtraEdit<T> : ElementEditor where T : AmlNodeElement
    {
        public override sealed Type AdapterType => typeof(T);

        public sealed override void Draw(UIElement host, AmlNodeElement element)
        {
            OnDraw(host, element as T);
        }

        protected abstract void OnDraw(UIElement host, T element);
    }
}

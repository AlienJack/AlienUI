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
    public abstract class ResourceEdit<T> : ElementEditor where T : Resource
    {
        public override sealed Type AdapterType => typeof(T);

        public sealed override void Draw(UIElement host, AmlNodeElement element)
        {
            OnDraw(host, element as T);
        }

        protected abstract void OnDraw(UIElement host, T element);
    }
    public abstract class TriggerEdit<T> : ElementEditor where T : Trigger
    {
        public override sealed Type AdapterType => typeof(T);
        public sealed override void Draw(UIElement host, AmlNodeElement element)
        {
            OnDraw(host, element as T);
        }

        protected abstract void OnDraw(UIElement host, T element);
    }
}

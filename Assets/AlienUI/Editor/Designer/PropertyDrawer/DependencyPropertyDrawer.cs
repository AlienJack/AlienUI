using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AlienUI.Editors.PropertyDrawer
{
    public abstract class PropertyDrawerBase
    {
        public abstract Type ValueType { get; }
        public abstract object Draw(UIElement host, string label, object value);
        public abstract object OnSceneGUI(UIElement host, string label, object value);
    }

    public abstract class PropertyDrawer<T> : PropertyDrawerBase
    {
        public sealed override Type ValueType => typeof(T);

        public sealed override object Draw(UIElement host, string label, object value)
        {
            return OnDraw(host, label, (T)value);
        }

        public override sealed object OnSceneGUI(UIElement host, string label, object value)
        {
            return OnDrawSceneGUI(host, label, (T)value);
        }

        protected abstract T OnDraw(UIElement host, string label, T value);
        protected virtual T OnDrawSceneGUI(UIElement host, string label, T value) { return value; }
    }
}

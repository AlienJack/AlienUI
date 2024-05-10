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
        public abstract object Draw(Rect rect, object value);
    }

    public abstract class PropertyDrawer<T> : PropertyDrawerBase
    {
        public sealed override Type ValueType => typeof(T);

        public sealed override object Draw(Rect rect, object value)
        {
            return OnDraw(rect, (T)value);
        }

        protected abstract T OnDraw(Rect rect, T value);
    }
}

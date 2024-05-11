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
        public abstract object Draw(object value);
    }

    public abstract class PropertyDrawer<T> : PropertyDrawerBase
    {
        public sealed override Type ValueType => typeof(T);

        public sealed override object Draw(object value)
        {
            return OnDraw((T)value);
        }

        protected abstract T OnDraw(T value);
    }
}

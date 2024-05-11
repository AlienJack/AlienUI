using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Models
{
    public class PropertyMeta
    {
        public object DefaultValue { get; private set; }
        public string Group { get; internal set; }
        public bool IsReadOnly { get; private set; }

        public PropertyMeta(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public PropertyMeta(object defaultValue, string group) : this(defaultValue)
        {
            Group = group;
        }

        public PropertyMeta ReadOnly()
        {
            IsReadOnly = true;
            return this;
        }
    }
}

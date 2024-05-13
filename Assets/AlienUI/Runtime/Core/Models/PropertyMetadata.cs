using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Models
{
    public class PropertyMetadata
    {
        public object DefaultValue { get; private set; }
        public string Group { get; internal set; }
        public bool IsReadOnly { get; private set; }
        public bool NotAllowEdit {  get; private set; }
        public PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public PropertyMetadata(object defaultValue, string group) : this(defaultValue)
        {
            Group = group;
        }

        public PropertyMetadata ReadOnly()
        {
            IsReadOnly = true;
            return this;
        }

        public PropertyMetadata DisableEdit()
        {
            NotAllowEdit = true;
            return this;
        }
    }
}

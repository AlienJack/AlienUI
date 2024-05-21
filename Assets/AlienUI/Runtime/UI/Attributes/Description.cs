using System;
using System.Diagnostics;

namespace AlienUI.Models.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class DescriptionAttribute : Attribute
    {
        public string Icon { get; set; }
        public string Des { get; set; }
    }
}

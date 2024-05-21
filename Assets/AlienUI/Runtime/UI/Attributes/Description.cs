using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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

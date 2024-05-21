using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace AlienUI.Models.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AllowChildAttribute : Attribute
    {
        public Type[] childTypes { get; private set; }

        public AllowChildAttribute(params Type[] types)
        {
            childTypes = types;
        }
    }

}

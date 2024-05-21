using System;
using System.Diagnostics;

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

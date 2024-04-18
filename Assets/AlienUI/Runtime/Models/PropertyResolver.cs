using System;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class PropertyResolver
    {
        public abstract Type GetResolveType();
        public abstract object Resolve(string originStr);

        public PropertyResolver() { }
    }
}

using System;
using UnityEngine;

namespace AlienUI.Models
{
    public abstract class PropertyResolver
    {
        public abstract Type GetResolveType();
        public abstract object Resolve(string originStr);
        public abstract object Lerp(object from, object to, float progress);

        public PropertyResolver() { }
    }

    public abstract class PropertyTypeResolver<T> : PropertyResolver
    {
        public sealed override Type GetResolveType()
        {
            return typeof(T);
        }

        protected abstract T OnResolve(string originStr);
        public sealed override object Resolve(string originStr)
        {
            return OnResolve(originStr);
        }

        protected abstract T OnLerp(T from, T to, float progress);
        public sealed override object Lerp(object from, object to, float progress)
        {
            return OnLerp((T)from, (T)to, progress);
        }

    }
}

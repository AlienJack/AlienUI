using System;

namespace AlienUI.Models
{
    public abstract class PropertyResolver
    {
        public abstract Type GetResolveType();
        public abstract object Resolve(string originStr, Type valueType);
        public abstract object Lerp(object from, object to, float progress);

        public PropertyResolver() { }
    }

    public abstract class PropertyResolver<T> : PropertyResolver
    {
        public sealed override Type GetResolveType()
        {
            return typeof(T);
        }

        protected abstract T OnResolve(string originStr);
        public sealed override object Resolve(string originStr, Type resolveType)
        {
            return OnResolve(originStr);
        }

        protected abstract T OnLerp(T from, T to, float progress);
        public sealed override object Lerp(object from, object to, float progress)
        {
            return OnLerp((T)from, (T)to, progress);
        }
    }

    public class EnumResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(Enum);
        }

        public override object Resolve(string originStr, Type enumType)
        {
            Enum.TryParse(enumType, originStr, true, out var result);
            return result;
        }

        public override object Lerp(object from, object to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }
    }
}

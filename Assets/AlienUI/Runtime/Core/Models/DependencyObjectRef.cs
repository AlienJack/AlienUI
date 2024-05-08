﻿namespace AlienUI.Models
{
    public struct DependencyObjectRef
    {
        private string m_uniqueTag;

        public DependencyObjectRef(string uniqueTag)
        {
            m_uniqueTag = uniqueTag;
        }

        public readonly DependencyObject Get(IDependencyObjectResolver resolver)
        {
            if (string.IsNullOrWhiteSpace(m_uniqueTag)) return null;
            return resolver.Resolve(m_uniqueTag);
        }
    }

    public interface IDependencyObjectResolver
    {
        DependencyObject Resolve(string resolveKey);
    }
}
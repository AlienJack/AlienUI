using System.Collections;
using UnityEngine;

namespace AlienUI.Models
{
    public struct DependencyObjectRef
    {
        private string m_uniqueTag;

        public DependencyObjectRef(string uniqueTag)
        {
            m_uniqueTag = uniqueTag;
        }

        public DependencyObject Get(IDependencyObjectResolver resolver)
        {
            return resolver.Resolve(m_uniqueTag);
        }
    }

    public interface IDependencyObjectResolver
    {
        DependencyObject Resolve(string resolveKey);
    }
}
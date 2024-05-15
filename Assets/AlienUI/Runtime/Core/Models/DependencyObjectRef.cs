namespace AlienUI.Models
{
    public struct DependencyObjectRef
    {
        private string m_uniqueTag;
        private DependencyObject m_referenceCache;

        public readonly string GetUniqueTag() => m_uniqueTag;

        public DependencyObjectRef SetUniqueTag(string tag)
        {
            m_uniqueTag = tag;
            return this;
        }

        public DependencyObjectRef(string uniqueTag)
        {
            m_uniqueTag = uniqueTag;
            m_referenceCache = null;
        }

        public DependencyObjectRef(DependencyObject referenceCache)
        {
            m_uniqueTag = null;
            m_referenceCache = referenceCache;
        }

        public readonly DependencyObject Get(IDependencyObjectResolver resolver)
        {
            if (m_referenceCache != null) return m_referenceCache;

            if (resolver == null) return null;

            if (string.IsNullOrWhiteSpace(m_uniqueTag)) return null;
            return resolver.Resolve(m_uniqueTag);
        }
    }

    public interface IDependencyObjectResolver
    {
        DependencyObject Resolve(string resolveKey);
    }
}
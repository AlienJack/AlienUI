using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RefMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, AlienUI.UIElements.ToolsScript.ReferenceManifest>>;

namespace AlienUI.UIElements.ToolsScript
{
    [Serializable]
    public class UnityReference
    {
        [SerializeField]
        private List<ReferenceManifest> refList;

        [NonSerialized]
        internal RefMap m_refMap;

        internal void OptimizeData()
        {
            m_refMap = new RefMap();
            foreach (ReferenceManifest manifest in refList)
            {
                if (manifest.RefObject == null) return;

                var group = manifest.Group;
                if (!m_refMap.ContainsKey(group)) m_refMap[group] = new Dictionary<string, ReferenceManifest>();

                var name = manifest.Name;
                m_refMap[group][name] = manifest;
            }
        }

        public T GetAsset<T>(string group, string name) where T : UnityEngine.Object
        {
            m_refMap.TryGetValue(group, out var assets);
            if (assets == null) return null;

            assets.TryGetValue(name, out var asset);
            return asset as T;
        }

#if UNITY_EDITOR
        internal void AddAsset(ReferenceManifest manifest)
        {
            if (manifest.RefObject == null) return;

            if (refList.Contains(manifest)) return;

            if (manifest.Group == null) return;
            if (manifest.Name == null) return;

            refList.Add(manifest);
        }

        internal void AddAsset(string group, string name, UnityEngine.Object asset)
        {
            refList.Add(new ReferenceManifest { Group = group, Name = name, RefObject = asset });
        }
#endif
    }
    [Serializable]
    public class ReferenceManifest
    {
        [SerializeField]
        public UnityEngine.Object RefObject;
        [SerializeField]
        public string Group;
        [SerializeField]
        public string Name;

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RefObject, Group, Name);
        }
    }
}

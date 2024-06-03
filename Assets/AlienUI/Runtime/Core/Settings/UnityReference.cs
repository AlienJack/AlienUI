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
            refList = refList.Where(r => r.IsValid).Distinct().ToList();
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
            if (asset == null) return null;
            return asset.RefObject as T;
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

        internal void AddAsset(string group, string name, Type assetType, UnityEngine.Object asset)
        {
            refList.Add(new ReferenceManifest { Group = group, Name = name, RefObject = asset, TypeName = assetType.FullName });
        }

        internal bool GetUnityAssetPath(UnityEngine.Object obj, out string group, out string assetName)
        {
            group = string.Empty;
            assetName = string.Empty;

            foreach (var item in refList)
            {
                if (item.RefObject != obj) continue;

                group = item.Group;
                assetName = item.Name;
                return true;
            }

            return false;
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
        [SerializeField]
        public string TypeName;

        public bool IsValid
        {
            get
            {
                if (RefObject == null) return false;
                if (string.IsNullOrWhiteSpace(Group)) return false;
                if (string.IsNullOrWhiteSpace(Name)) return false;
                if (string.IsNullOrWhiteSpace(TypeName)) return false;

                return true;
            }
        }

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

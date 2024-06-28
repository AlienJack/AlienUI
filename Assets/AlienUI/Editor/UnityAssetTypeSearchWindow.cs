using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AlienUI.Editors
{
    public class UnityAssetTypeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        public List<Type> Options;

        public Action<Type> OnSelectType;

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> result = new();
            var groupEntry = new SearchTreeGroupEntry(new GUIContent($"Select Type"), 0);
            result.Add(groupEntry);

            foreach (var type in Options)
            {
                var item = new SearchTreeEntry(new GUIContent($"{type.FullName}"));
                item.userData = type;
                item.level = 1;
                result.Add(item);
            }

            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            OnSelectType?.Invoke(SearchTreeEntry.userData as Type);

            return true;
        }
    }
}

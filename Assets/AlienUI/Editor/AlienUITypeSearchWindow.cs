using AlienUI.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AlienUITypeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        List<Type> Options;

        Action<Type> m_onSelectType;

        public static void OpenWindow<T>(Action<Type> OnSelect) where T : AmlNodeElement
        {
            var privoder = CreateInstance<AlienUITypeSearchWindow>();
            privoder.Options = Settings.Get().m_collector.GetTypesOfSubType(typeof(T));
            privoder.m_onSelectType = OnSelect;
            
            SearchWindow.Open(new SearchWindowContext(Input.mousePosition, 200, 400), privoder);
        }

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
            m_onSelectType?.Invoke(SearchTreeEntry.userData as Type);

            return true;
        }
    }
}

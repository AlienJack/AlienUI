using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace AlienUI.Editors
{
    public class AssetSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public UIElement Target { get; internal set; }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> result = new();
            var groupEntry = new SearchTreeGroupEntry(new GUIContent("Add UI"), 0);
            result.Add(groupEntry);
            foreach (var item in AssetInventory.GetAssets())
            {
                var group = item.Key;
                result.Add(new SearchTreeGroupEntry(new GUIContent(group), 1));
                foreach (var asset in item.Value)
                {
                    result.Add(new SearchTreeEntry(new GUIContent(asset.AssetType.Name, asset.AssetType.GetIcon())) { level = 2, userData = asset });
                }
            }
            result.Add(new SearchTreeEntry(new GUIContent("Delete", Settings.Get().GetIcon("danger"))) { level = 1, userData = Target });

            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (Target != null)
            {
                if (SearchTreeEntry.userData is AssetInventory.Asset asset)
                    Designer.AddChild(asset.AssetType, Target);
                else if (SearchTreeEntry.content.text == "Delete" && SearchTreeEntry.userData is UIElement deleteTarget)
                {
                    if (deleteTarget.UIParent != null)
                    {
                        deleteTarget.UIParent.RemoveChild(deleteTarget);
                        Designer.SaveToAml(Designer.Instance);
                    }
                }
            }

            return true;
        }
    }
}

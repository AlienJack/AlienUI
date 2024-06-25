using AlienUI.UIElements;
using System.Collections.Generic;
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
            var groupEntry = new SearchTreeGroupEntry(new GUIContent($"Edit {Target.Name ?? Target.GetType().Name}"), 0);
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
            if (Target is UserControl uc)
            {
                if (uc.Template.Valid)
                    result.Add(new SearchTreeEntry(new GUIContent("Edit Template", AlienEditorUtility.GetIcon("amlicon"))) { level = 1, userData = Target });
                else if (uc.DefaultTemplate.Valid)
                    result.Add(new SearchTreeEntry(new GUIContent("Create&Edit Default Template Copy", AlienEditorUtility.GetIcon("amlicon"))) { level = 1, userData = Target });
            }
            result.Add(new SearchTreeEntry(new GUIContent("Delete", AlienEditorUtility.GetIcon("danger"))) { level = 1, userData = Target });

            return result;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (Target != null)
            {
                if (SearchTreeEntry.userData is AssetInventory.Asset asset)
                    Designer.AddChild(asset.AssetType, Target);
                else if (SearchTreeEntry.content.text == "Edit Template" && SearchTreeEntry.userData is UserControl EditTemplateTarget)
                {
                    var tempAsset = Settings.Get().GetTemplateAsset(EditTemplateTarget.Template.Name);
                    if (tempAsset != null)
                    {
                        Designer.Instance.Restart(tempAsset);
                    }
                }
                else if (SearchTreeEntry.content.text == "Create&Edit Default Template Copy" && SearchTreeEntry.userData is UserControl createCopyTarget)
                {
                    PerformCreateAndEditTemplateAction(createCopyTarget);
                }
                else if (SearchTreeEntry.content.text == "Delete" && SearchTreeEntry.userData is UIElement deleteTarget)
                {
                    if (deleteTarget.UIParent != null)
                    {
                        deleteTarget.UIParent.RemoveChild(deleteTarget);
                        deleteTarget.Close();
                        Designer.SaveToAml();
                    }
                }
            }

            return true;
        }

        private static void PerformCreateAndEditTemplateAction(UserControl createCopyTarget)
        {
            var src = Settings.Get().GetTemplateAsset(createCopyTarget.DefaultTemplate.Name);
            if (src == null) return;

            var newAsset = Settings.Get().CreateTemplateAml(src);

            if (newAsset != null)
            {
                createCopyTarget.Template = new ControlTemplate(newAsset.name);
                Designer.Instance.Restart(newAsset);
            }

        }
    }
}

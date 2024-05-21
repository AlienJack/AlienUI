using AlienUI.Core;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.Progress;

namespace AlienUI.Editors
{
    internal class LogicTree : TreeView
    {
        private UIElement m_root;
        private Dictionary<int, UIElement> m_uiMaps = new Dictionary<int, UIElement>();
        public LogicTree(UIElement uiRoot)
            : base(new TreeViewState())
        {
            m_root = uiRoot;

            Reload();
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;
        protected override bool CanRename(TreeViewItem item) => true;
        protected override void RenameEnded(RenameEndedArgs args)
        {
            var selectionID = this.GetSelection().FirstOrDefault();
            m_uiMaps.TryGetValue(selectionID, out UIElement selection);
            if (selection == null) return;

            if (!args.acceptedRename) return;

            if ($"[{selection.GetType().Name}]" == args.newName)
            {
                selection.Name = null;
            }
            else
            {
                args.newName = args.newName.TrimStart('#');

                if (string.IsNullOrWhiteSpace(args.newName))
                {
                    selection.Name = null;
                }
                else
                {
                    selection.Name = args.newName;
                }
            }

            Designer.SaveToAml(Designer.Instance);

            Reload();
        }

        protected override void ContextClickedItem(int id)
        {
            m_uiMaps.TryGetValue(id, out UIElement selection);
            if (selection == null) return;

            var pos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
            pos.x += 130;
            SearchWindow.Open(new SearchWindowContext(pos, 200, 400), AssetInventory.GetSearchProvider(selection));

            Event.current.Use();
        }

        public IEnumerable<UIElement> GetUIs()
        {
            return m_uiMaps.Values;
        }

        public event Action<UIElement> OnSelectItem;

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            UIElement selection = null;
            if (selectedIds.Count > 0)
            {
                m_uiMaps.TryGetValue(selectedIds[0], out selection);
            }
            OnSelectItem?.Invoke(selection);
        }

        public void SelectWithoutNotify(UIElement ui)
        {
            foreach (var item in m_uiMaps)
            {
                if (item.Value == ui)
                {
                    SetSelection(new List<int> { item.Key }, TreeViewSelectionOptions.RevealAndFrame);
                    return;
                }
            }
            Repaint();
        }

        protected override TreeViewItem BuildRoot()
        {
            var rootItem = new TreeViewItem { id = 0, depth = -1 };

            var allItems = new List<TreeViewItem>();
            var treeItem = ToTreeViewItem(m_root, 0);
            allItems.Add(treeItem);
            m_uiMaps[treeItem.id] = m_root;
            FillTreeItemData(m_root, m_root.Document, ref allItems, 1);
            DepthNormalizer.NormalizeDepths(allItems);
            SetupParentsAndChildrenFromDepths(rootItem, allItems);
            return rootItem;
        }

        private void FillTreeItemData(UIElement parent, Document doc, ref List<TreeViewItem> items, int depth)
        {
            foreach (var child in parent.UIChildren)
            {
                var treeItem = ToTreeViewItem(child, depth);

                if (child.Document == doc)
                {
                    items.Add(treeItem);
                    m_uiMaps[treeItem.id] = child;
                }

                FillTreeItemData(child, doc, ref items, depth + 1);
            }
        }

        private TreeViewItem ToTreeViewItem(UIElement ui, int depth)
        {
            var treeItem = new TreeViewItem
            {
                id = ui.GetHashCode(),
                depth = depth,
                displayName = ui.Name != null ? $"#{ui.Name}" : $"[{ui.GetType().Name}]",
                icon = ui.GetIcon()
            };

            return treeItem;
        }

        class DepthNormalizer
        {
            public static void NormalizeDepths(List<TreeViewItem> items)
            {
                // Get the distinct depths and sort them
                var sortedDepths = items.Select(item => item.depth).Distinct().OrderBy(depth => depth).ToList();

                // Create a dictionary to map the original depth to its new value
                var depthMapping = sortedDepths.Select((depth, index) => new { depth, index })
                                               .ToDictionary(d => d.depth, d => d.index);

                // Update the items with the new depth values
                items.ForEach(item => item.depth = depthMapping[item.depth]);
            }
        }
    }
}

using AlienUI.Core;
using AlienUI.UIElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

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
        protected override bool CanRename(TreeViewItem item) => false;
        protected override void RenameEnded(RenameEndedArgs args) { }

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
            };

            return treeItem;
        }
    }
}

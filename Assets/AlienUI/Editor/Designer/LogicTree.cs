using AlienUI.Core;
using AlienUI.Models;
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
        private AmlAsset m_indentParent;
        private UIElement m_root;
        private Dictionary<int, UIElement> m_uiMaps = new Dictionary<int, UIElement>();
        public LogicTree(UIElement uiRoot, AmlAsset indentParent)
            : base(new TreeViewState())
        {
            m_root = uiRoot;
            m_indentParent = indentParent;
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            if (m_indentParent != null && args.item.id == m_indentParent.GetHashCode())
            {
                var rect = new Rect(args.rowRect);
                rect.position += new Vector2(15, 0);
                rect.width -= 15;
                if (GUI.Button(rect, new GUIContent(args.item.displayName,AlienEditorUtility.GetIcon("uparrow"))))
                {
                    Designer.Instance.BackIndent();
                }
            }
            else
            {
                base.RowGUI(args);
            }
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;
        protected override bool CanRename(TreeViewItem item)
        {
            return m_indentParent == null || item.id != m_indentParent.GetHashCode();
        }
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

            Designer.SaveToAml();

            Reload();
        }

        public void SelectItem(UIElement sel)
        {
            List<int> selectIds = new List<int>();
            foreach (var item in m_uiMaps)
            {
                if (item.Value == sel)
                {
                    selectIds.Add(item.Key);
                    break;
                }
            }

            SetSelection(selectIds, TreeViewSelectionOptions.RevealAndFrame | TreeViewSelectionOptions.FireSelectionChanged);
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
            if (selection != null)
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

            int startDepth = 0;
            if (m_indentParent != null)
            {
                var indepenItem = new TreeViewItem(m_indentParent.GetHashCode(), startDepth, m_indentParent.name);
                allItems.Add(indepenItem);

                startDepth = 1;
            }

            var treeItem = ToTreeViewItem(m_root, startDepth);
            allItems.Add(treeItem);
            m_uiMaps[treeItem.id] = m_root;

            FillTreeItemData(m_root, m_root.Document, ref allItems, startDepth + 1);
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

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            m_uiMaps.TryGetValue(args.draggedItem.id, out var dragElement);
            if (dragElement == null) return false;

            if (dragElement.UIParent == null) return false;

            return true;
        }
        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            m_uiMaps.TryGetValue(args.draggedItemIDs[0], out var dragElement);
            if (dragElement == null) return;

            this.SetSelection(new List<int> { args.draggedItemIDs[0] }, TreeViewSelectionOptions.FireSelectionChanged);
            DragAndDrop.StartDrag(dragElement.Name ?? dragElement.GetType().Name);
            DragAndDrop.SetGenericData("InternalLogicTreeMove", dragElement);
        }
        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            if (DragAndDrop.GetGenericData("InternalLogicTreeMove") is UIElement dragElement)
            {
                return HandleLogicTreeMove(args, dragElement);
            }
            else if (DragAndDrop.GetGenericData("AssetDrag") is Type genType)
            {
                return HandleAssetGenDrag(args, genType);
            }

            return DragAndDropVisualMode.Rejected;
        }

        private DragAndDropVisualMode HandleAssetGenDrag(DragAndDropArgs args, Type genType)
        {
            if (args.performDrop)
            {
                if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
                {
                    if (args.parentItem != null && m_uiMaps.TryGetValue(args.parentItem.id, out var newParent))
                    {
                        var newUI = Designer.AddChild(genType, newParent);
                        newUI.UIParent.MoveChild(newUI, args.insertAtIndex);
                        Designer.SaveToAml();

                        SelectItem(newUI);
                    }
                }
                else if (args.dragAndDropPosition == DragAndDropPosition.UponItem)
                {
                    if (args.parentItem != null && m_uiMaps.TryGetValue(args.parentItem.id, out var newParent))
                    {
                        var newUI = Designer.AddChild(genType, newParent);
                        Designer.SaveToAml();
                        SelectItem(newUI);
                    }
                }
            }

            return args.dragAndDropPosition switch
            {
                DragAndDropPosition.UponItem => DragAndDropVisualMode.Generic,
                DragAndDropPosition.OutsideItems => DragAndDropVisualMode.Rejected,
                DragAndDropPosition.BetweenItems => DragAndDropVisualMode.Generic,
                _ => DragAndDropVisualMode.None,
            };
        }

        private DragAndDropVisualMode HandleLogicTreeMove(DragAndDropArgs args, UIElement dragElement)
        {
            if (args.performDrop)
            {
                if (args.dragAndDropPosition == DragAndDropPosition.BetweenItems)
                {
                    if (args.parentItem != null && m_uiMaps.TryGetValue(args.parentItem.id, out var newParent))
                    {
                        if (dragElement.UIParent == newParent)
                        {
                            dragElement.UIParent.MoveChild(dragElement, args.insertAtIndex);
                        }
                        else
                        {
                            dragElement.UIParent.RemoveChild(dragElement);
                            newParent.AddChild(dragElement);
                            dragElement.UIParent.MoveChild(dragElement, args.insertAtIndex);
                        }

                        Designer.SaveToAml();

                        SelectItem(dragElement);
                    }
                }
                else if (args.dragAndDropPosition == DragAndDropPosition.UponItem)
                {
                    if (args.parentItem != null && m_uiMaps.TryGetValue(args.parentItem.id, out var newParent))
                    {
                        if (dragElement.UIParent == newParent)
                        {
                            dragElement.UIParent.MoveChild(dragElement, int.MaxValue);//move to last
                        }
                        else
                        {
                            dragElement.UIParent.RemoveChild(dragElement);
                            newParent.AddChild(dragElement);
                        }

                        Designer.SaveToAml();

                        SelectItem(dragElement);
                    }
                }
            }

            return args.dragAndDropPosition switch
            {
                DragAndDropPosition.UponItem => DragAndDropVisualMode.Move,
                DragAndDropPosition.OutsideItems => DragAndDropVisualMode.Rejected,
                DragAndDropPosition.BetweenItems => DragAndDropVisualMode.Move,
                _ => DragAndDropVisualMode.None,
            };
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

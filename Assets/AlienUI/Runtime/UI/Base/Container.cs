using AlienUI.Models;
using AlienUI.Models.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AlienUI.UIElements.Containers
{
    [Description(Icon = "Container")]
    public abstract class Container : UIElement
    {
        public static bool GetIgnoreChild(DependencyObject obj)
        {
            return (bool)obj.GetValue(IgnoreChildProperty);
        }

        public static void SetIgnoreChild(DependencyObject obj, bool value)
        {
            obj.SetValue(IgnoreChildProperty, value);
        }

        public static readonly DependencyProperty IgnoreChildProperty =
            DependencyProperty.RegisterAttached("IgnoreChild", typeof(bool), typeof(Container), new PropertyMetadata(false), OnLayoutParamDirty);


        public ItemTemplate ItemTemplate
        {
            get { return (ItemTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(ItemTemplate), typeof(Container), new PropertyMetadata(new ItemTemplate("Builtin.SimpleItem")));

        public IList ItemData
        {
            get { return (IList)GetValue(ItemDataProperty); }
            set { SetValue(ItemDataProperty, value); }
        }

        public static readonly DependencyProperty ItemDataProperty =
            DependencyProperty.Register("ItemData", typeof(IList), typeof(Container), new PropertyMetadata(null), OnItemDataChanged);

        private static void OnItemDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Container;
            self.RefreshChildrenUI();
        }

        private List<UIElement> m_childrenFromItemData = new List<UIElement>();
        private void RefreshChildrenUI()
        {
            foreach (var child in m_childrenFromItemData)
            {
                RemoveChild(child);
                child.Close();
            }
            m_childrenFromItemData.Clear();

            if (ItemData != null)
            {
                foreach (var data in ItemData)
                {
                    var itemUI = ItemTemplate.Instantiate(Engine, m_childRoot, data, null);
                    m_childrenFromItemData.Add(itemUI);
                    AddChild(itemUI);
                }
            }
        }

        protected override void OnInitialized() { }

        protected abstract Vector2 OnCalcDesireSize(IReadOnlyList<UIElement> children);

        protected sealed override Vector2 CalcDesireSize()
        {
            return OnCalcDesireSize(UIChildren.Where(child => !GetIgnoreChild(child) && child.Active).ToList());
        }

        public sealed override void CalcChildrenLayout()
        {
            OnCalcChildrenLayout(UIChildren.Where(child => !GetIgnoreChild(child) && child.Active).ToList());
            foreach (var child in UIChildren)
            {
                if (!GetIgnoreChild(child)) continue;
                PerformUIElement(child);
            }
        }

        protected abstract void OnCalcChildrenLayout(IReadOnlyList<UIElement> children);
    }
}

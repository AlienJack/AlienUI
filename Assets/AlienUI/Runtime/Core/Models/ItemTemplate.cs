using AlienUI.Models;
using AlienUI.UIElements;
using System;
using UnityEngine;

namespace AlienUI
{
    public readonly struct ItemTemplate : IEquatable<ItemTemplate>
    {
        private readonly string m_name;
        public readonly bool Valid => m_name != null;
        public string Name => m_name;

        public ItemTemplate(string name)
        {
            m_name = name;
        }

        public readonly UIElement Instantiate(Engine engine, RectTransform parent, object dataContext, AmlNodeElement templateHost)
        {
            var templateAsset = Settings.Get().GetTemplateAsset(m_name);
            if (templateAsset == null) return null;

            var uiInstance = engine.CreateTemplate(templateAsset, parent, dataContext, templateHost);

            return uiInstance;
        }

        public bool Equals(ItemTemplate other)
        {
            return m_name == other.m_name;
        }

        public override bool Equals(object obj)
        {
            if (obj is ItemTemplate contrl) return Equals(contrl);
            else return false;
        }

        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        public static bool operator ==(ItemTemplate a, object b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ItemTemplate a, object b)
        {
            return !a.Equals(b);
        }
    }
}

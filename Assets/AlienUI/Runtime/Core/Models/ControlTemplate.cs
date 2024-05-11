using AlienUI.Models;
using AlienUI.UIElements;
using System;
using UnityEngine;

namespace AlienUI
{
    public readonly struct ControlTemplate : IEquatable<ControlTemplate>
    {
        private readonly string m_name;
        public readonly bool Valid => m_name != null;
        public string Name => m_name;

        public ControlTemplate(string name)
        {
            m_name = name;
        }

        public readonly UIElement Instantiate(Engine engine, RectTransform parent, DependencyObject dataContext, XmlNodeElement templateHost)
        {
            var templateAsset = Settings.Get().GetTemplateAsset(m_name);
            if (templateAsset == null) return null;

            var uiInstance = engine.CreateTemplate(templateAsset, parent, dataContext, templateHost);

            return uiInstance;
        }

        public bool Equals(ControlTemplate other)
        {
            return m_name == other.m_name;
        }

        public override bool Equals(object obj)
        {
            if (obj is ControlTemplate contrl) return Equals(contrl);
            else return false;
        }

        public override int GetHashCode()
        {
            return m_name.GetHashCode();
        }

        public static bool operator ==(ControlTemplate a, object b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ControlTemplate a, object b)
        {
            return !a.Equals(b);
        }
    }
}

using AlienUI.Models;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public class Storyboard : Resource
    {
        public List<Animation> m_animations;

        public override void ParseFromXml(XmlNode xnode)
        {
            
        }
    }

    public class Animation
    {
        public string PropertyName;

        public object From;
        public object To;
        public float Duration;
        public float Offset;

        public bool Evalution(float time, Engine engine, DependencyObject target, out object value)
        {
            value = null;
            if (time < Offset) return false;

            var type = target.GetDependencyPropertyType(PropertyName);
            if (type == null) return false;

            var resolver = engine.GetAttributeResolver(type);
            if (resolver == null) return false;

            var progress = (time - Offset) / Duration;

            value = resolver.Lerp(From, To, progress);

            return true;
        }
    }
}

using System.Xml;
using UnityEngine;

namespace AlienUI.Core.Resources
{
    public abstract class Resource
    {
        public abstract void ParseFromXml(XmlNode xnode);
    }
}

using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class ColorResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(Color);
        }

        public override object Resolve(string originStr)
        {
            if (!ColorUtility.TryParseHtmlString(originStr, out var color))
                Debug.LogError($"����Html��ɫ�������:{originStr}");
            return color;
        }
    }
}
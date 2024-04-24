using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class TextAlignHorizontalResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(TextAlignHorizontal);
        }

        public override object Resolve(string originStr)
        {
            Enum.TryParse<TextAlignHorizontal>(originStr, true, out TextAlignHorizontal result);
            return result;
        }
    }

    public class TextAlignVerticalResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(TextAlignVertical);
        }

        public override object Resolve(string originStr)
        {
            Enum.TryParse<TextAlignVertical>(originStr, true, out TextAlignVertical result);
            return result;
        }
    }
}

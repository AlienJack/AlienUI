using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class eHorizontalAlignResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(eHorizontalAlign);
        }

        public override object Resolve(string originStr)
        {
            Enum.TryParse<eHorizontalAlign>(originStr, true, out eHorizontalAlign result);
            return result;
        }
    }

    public class eVerticalAlignResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(eVerticalAlign);
        }

        public override object Resolve(string originStr)
        {
            Enum.TryParse<eVerticalAlign>(originStr, true, out eVerticalAlign result);
            return result;
        }
    }
}

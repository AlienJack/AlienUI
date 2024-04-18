using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class BorderResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(Border);
        }

        public override object Resolve(string originStr)
        {
            var result = new Border();

            originStr = originStr.Trim('(').Trim(')');
            var paramList = originStr.Split(',');
            for (int i = 0; i < paramList.Length; i++)
            {
                float.TryParse(paramList[i], out float value);
                if (i == 0) result.top = value;
                else if (i == 1) result.bottom = value;
                else if (i == 2) result.left = value;
                else if (i == 3) result.right = value;
            }

            return result;
        }
    }

}

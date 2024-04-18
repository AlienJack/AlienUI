using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class NumberResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(Number);
        }

        public override object Resolve(string originStr)
        {
            if (originStr == "*") return Number.Identity;
            else
            {
                float.TryParse(originStr, out float value);
                return new Number { Value = value };
            }
        }
    }
}
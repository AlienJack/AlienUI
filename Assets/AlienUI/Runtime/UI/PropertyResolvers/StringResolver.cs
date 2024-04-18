using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class StringResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(string);
        }

        public override object Resolve(string originStr)
        {
            return originStr;
        }
    }
}
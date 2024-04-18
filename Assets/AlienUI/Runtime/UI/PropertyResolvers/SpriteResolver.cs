using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class SpriteResolver : PropertyResolver
    {
        public override Type GetResolveType()
        {
            return typeof(Sprite);
        }

        public override object Resolve(string originStr)
        {
            return Resources.Load<Sprite>(originStr);
        }
    }
}
using AlienUI.Models;
using System.Collections;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class DependencyObjectResolver : PropertyTypeResolver<DependencyObjectRef>
    {
        protected override DependencyObjectRef OnResolve(string originStr)
        {
            return new DependencyObjectRef(originStr);
        }

        protected override DependencyObjectRef OnLerp(DependencyObjectRef from, DependencyObjectRef to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }
    }
}
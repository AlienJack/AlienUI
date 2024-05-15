﻿using AlienUI.Models;

namespace AlienUI.PropertyResolvers
{
    public class DependencyObjectResolver : PropertyResolver<DependencyObjectRef>
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

        protected override string Reverse(DependencyObjectRef value)
        {
            return value.GetUniqueTag();
        }
    }
}
using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class StringResolver : PropertyTypeResolver<string>
    {
        protected override string OnResolve(string originStr)
        {
            return originStr;
        }

        protected override string OnLerp(string from, string to, float progress)
        {
            if (Mathf.Approximately(progress, 1f))
                return to;
            else
                return from;
        }
    }
}
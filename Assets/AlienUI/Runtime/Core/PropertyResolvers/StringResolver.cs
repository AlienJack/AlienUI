using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class StringResolver : PropertyResolver<string>
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

        protected override string Reverse(string value)
        {
            return value;
        }
    }
}
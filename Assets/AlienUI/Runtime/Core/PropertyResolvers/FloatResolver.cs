using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class FloatResolver : PropertyTypeResolver<float>
    {
        protected override float OnResolve(string originStr)
        {
            float.TryParse(originStr, out float value);
            return value;
        }

        protected override float OnLerp(float from, float to, float progress)
        {
            return Mathf.Lerp(from, to, progress);
        }
    }
}
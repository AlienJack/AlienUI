using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class ColorResolver : PropertyTypeResolver<Color>
    {
        protected override Color OnResolve(string originStr)
        {
            if (!ColorUtility.TryParseHtmlString(originStr, out var color))
                Debug.LogError($"����Html��ɫ�������:{originStr}");
            return color;
        }

        protected override Color OnLerp(Color from, Color to, float progress)
        {
            return Color.Lerp(from, to, progress);
        }
    }
}
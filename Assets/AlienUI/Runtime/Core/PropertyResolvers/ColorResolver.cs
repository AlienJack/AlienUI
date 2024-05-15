using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class ColorResolver : PropertyResolver<Color>
    {
        protected override Color OnResolve(string originStr)
        {
            if (!ColorUtility.TryParseHtmlString(originStr, out var color))
                Engine.LogError($"解析Html颜色代码出错:{originStr}");
            return color;
        }

        protected override Color OnLerp(Color from, Color to, float progress)
        {
            return Color.Lerp(from, to, progress);
        }

        protected override string Reverse(Color value)
        {
            return ColorUtility.ToHtmlStringRGBA(value);
        }
    }
}
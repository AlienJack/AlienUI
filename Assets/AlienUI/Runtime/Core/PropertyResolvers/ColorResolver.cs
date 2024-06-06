using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class ColorResolver : PropertyResolver<Color>
    {
        protected override Color OnResolve(string originStr)
        {
            if (!ColorUtility.TryParseHtmlString(originStr, out var color))
                Engine.LogError($"����Html��ɫ�������:{originStr}");
            return color;
        }

        protected override Color OnLerp(Color from, Color to, float progress)
        {
            return Color.Lerp(from, to, progress);
        }

        protected override string Reverse(Color value)
        {
            return $"#{ColorUtility.ToHtmlStringRGBA(value)}";
        }
    }
}
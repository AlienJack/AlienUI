using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class FloatResolver : PropertyResolver<float>
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

        protected override string Reverse(float value)
        {
            return value.ToString();
        }
    }
}
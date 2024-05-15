using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class IntResolver : PropertyResolver<int>
    {
        protected override int OnResolve(string originStr)
        {
            int.TryParse(originStr, out var value);
            return value;
        }

        protected override int OnLerp(int from, int to, float progress)
        {
            var lerpValue = Mathf.Lerp(from, to, progress);
            return Mathf.RoundToInt(lerpValue);
        }

        protected override string Reverse(int value)
        {
            return value.ToString();
        }
    }
}

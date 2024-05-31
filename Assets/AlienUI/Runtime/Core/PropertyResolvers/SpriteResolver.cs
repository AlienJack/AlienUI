using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class UnityObjectResolver : PropertyResolver<UnityEngine.Object>
    {
        protected override Object OnResolve(string originStr)
        {
            var temp = originStr.Split('/');
            if (temp.Length != 2) return null;

            return Settings.Get().GetUnityAsset<Object>(temp[0], temp[1]);
        }

        protected override Object OnLerp(Object from, Object to, float progress)
        {
            if (Mathf.Approximately(progress, 1f))
                return to;
            else
                return from;
        }

        protected override string Reverse(Object value)
        {
            if (value == null) return string.Empty;

            Settings.Get().GetUnityAssetPath(value, out var group, out var assetName);
            return $"{group}/{assetName}";
        }
    }
}
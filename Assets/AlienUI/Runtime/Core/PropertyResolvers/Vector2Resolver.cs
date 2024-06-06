using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class Vector2Resolver : PropertyResolver<Vector2>
    {
        protected override Vector2 OnResolve(string originStr)
        {
            Vector2 result = default;
            var temp = originStr.Trim('(').Trim(')').Split(',');
            float.TryParse(temp[0], out result.x);
            float.TryParse(temp[1], out result.y);

            return result;
        }

        protected override Vector2 OnLerp(Vector2 from, Vector2 to, float progress)
        {
            return Vector2.Lerp(from, to, progress);
        }

        protected override string Reverse(Vector2 value)
        {
            return $"{value.x},{value.y}";
        }
    }
}

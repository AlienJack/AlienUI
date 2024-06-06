using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class Vector2IntResolver : PropertyResolver<Vector2Int>
    {
        protected override Vector2Int OnResolve(string originStr)
        {
            Vector2Int result = default;
            var temp = originStr.Trim('(').Trim(')').Split(',');
            int.TryParse(temp[0], out var x);
            int.TryParse(temp[1], out var y);

            result.x = x;
            result.y = y;

            return result;
        }

        protected override Vector2Int OnLerp(Vector2Int from, Vector2Int to, float progress)
        {
            var vector2 = Vector2.Lerp(from, to, progress);
            return new Vector2Int((int)vector2.x, (int)vector2.y);
        }

        protected override string Reverse(Vector2Int value)
        {
            return $"{value.x},{value.y}";
        }
    }
}

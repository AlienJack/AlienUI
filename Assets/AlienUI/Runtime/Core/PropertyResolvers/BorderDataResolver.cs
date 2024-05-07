using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class BorderResolver : PropertyResolver<BorderData>
    {
        protected override BorderData OnResolve(string originStr)
        {
            var result = new BorderData();

            originStr = originStr.Trim('(').Trim(')');
            var paramList = originStr.Split(',');
            for (int i = 0; i < paramList.Length; i++)
            {
                float.TryParse(paramList[i], out float value);
                if (i == 0) result.top = value;
                else if (i == 1) result.bottom = value;
                else if (i == 2) result.left = value;
                else if (i == 3) result.right = value;
            }

            return result;
        }

        protected override BorderData OnLerp(BorderData from, BorderData to, float progress)
        {
            var border = from;

            border.top = Mathf.Lerp(from.top, to.top, progress);
            border.bottom = Mathf.Lerp(from.bottom, to.bottom, progress);
            border.left = Mathf.Lerp(from.left, to.left, progress);
            border.right = Mathf.Lerp(from.right, to.right, progress);

            return border;
        }
    }

}

using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class TextAlignHorizontalResolver : PropertyTypeResolver<TextAlignHorizontal>
    {
        protected override TextAlignHorizontal OnResolve(string originStr)
        {
            Enum.TryParse<TextAlignHorizontal>(originStr, true, out TextAlignHorizontal result);
            return result;
        }

        protected override TextAlignHorizontal OnLerp(TextAlignHorizontal from, TextAlignHorizontal to, float progress)
        {
            if (progress >= 1)
                return to;
            else
                return from;
        }
    }

    public class TextAlignVerticalResolver : PropertyTypeResolver<TextAlignVertical>
    {
        protected override TextAlignVertical OnResolve(string originStr)
        {
            Enum.TryParse<TextAlignVertical>(originStr, true, out TextAlignVertical result);
            return result;
        }

        protected override TextAlignVertical OnLerp(TextAlignVertical from, TextAlignVertical to, float progress)
        {
            if (progress >= 1)
                return to;
            else
                return from;
        }
    }
}

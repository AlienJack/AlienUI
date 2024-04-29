using AlienUI.Models;
using System;

namespace AlienUI.PropertyResolvers
{
    public class TextAlignHorizontalResolver : PropertyResolver<TextAlignHorizontal>
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

    public class TextAlignVerticalResolver : PropertyResolver<TextAlignVertical>
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

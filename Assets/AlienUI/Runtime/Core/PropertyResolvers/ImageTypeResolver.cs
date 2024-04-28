using AlienUI.Models;
using System;
using UGUIImg = UnityEngine.UI.Image;

namespace AlienUI
{
    public class ImageTypeResolver : PropertyResolver<UGUIImg.Type>
    {
        protected override UGUIImg.Type OnResolve(string originStr)
        {
            Enum.TryParse(originStr, out UGUIImg.Type result);
            return result;
        }

        protected override UGUIImg.Type OnLerp(UGUIImg.Type from, UGUIImg.Type to, float progress)
        {
            if (progress >= 1)
                return to;
            else
                return from;
        }
    }
}

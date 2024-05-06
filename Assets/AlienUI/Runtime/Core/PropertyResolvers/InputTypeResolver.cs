using AlienUI.Models;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace AlienUI
{
    public class InputTypeResolver : PropertyResolver<InputField.InputType>
    {
        protected override InputField.InputType OnResolve(string originStr)
        {
            Enum.TryParse(originStr, out InputField.InputType type);
            return type;
        }

        protected override InputField.InputType OnLerp(InputField.InputType from, InputField.InputType to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }
    }
}

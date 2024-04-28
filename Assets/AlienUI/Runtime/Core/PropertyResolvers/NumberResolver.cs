using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class NumberResolver : PropertyResolver<Number>
    {

        protected override Number OnResolve(string originStr)
        {
            if (originStr == "*") return Number.Identity;
            else
            {
                float.TryParse(originStr, out float value);
                return new Number { Value = value };
            }
        }

        protected override Number OnLerp(Number from, Number to, float progress)
        {
            Number result = default;
            if (Mathf.Approximately(progress, 1f))
                result.Auto = to.Auto;
            else
                result.Auto = from.Auto;

            result.Value = Mathf.Lerp(from.Value, to.Value, progress);

            return result;
        }
    }
}
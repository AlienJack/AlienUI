using AlienUI.Models;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class ControlTemplateResolver : PropertyResolver<ControlTemplate>
    {
        protected override ControlTemplate OnResolve(string originStr)
        {
            return new ControlTemplate(originStr);
        }

        protected override ControlTemplate OnLerp(ControlTemplate from, ControlTemplate to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }
    }
}

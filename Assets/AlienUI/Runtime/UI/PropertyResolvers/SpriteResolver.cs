using AlienUI.Models;
using System;
using UnityEngine;

namespace AlienUI.PropertyResolvers
{
    public class SpriteResolver : PropertyTypeResolver<Sprite>
    {

        protected override Sprite OnResolve(string originStr)
        {
            return Resources.Load<Sprite>(originStr);
        }

        protected override Sprite OnLerp(Sprite from, Sprite to, float progress)
        {
            if (Mathf.Approximately(progress, 1f))
                return to;
            else
                return from;
        }
    }
}
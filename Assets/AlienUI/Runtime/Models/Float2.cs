using UnityEngine;

namespace AlienUI.Models
{
    public struct Float2
    {
        public float x;
        public float y;

        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static implicit operator Vector2(Float2 v)
        {
            return new Vector2(v.x, v.y);
        }
    }
}

using UnityEngine;

namespace AlienUI.Models
{
    public struct Border
    {
        public float top;
        public float bottom;
        public float left;
        public float right;

        public Border(float value)
        {
            top = value;
            bottom = value;
            left = value;
            right = value;
        }
    }
}

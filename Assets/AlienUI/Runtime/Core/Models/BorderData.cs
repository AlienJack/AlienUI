using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AlienUI.Models
{
    public struct BorderData : IEquatable<BorderData>
    {
        public float top;
        public float bottom;
        public float left;
        public float right;

        public BorderData(float value)
        {
            top = value;
            bottom = value;
            left = value;
            right = value;
        }

        public override readonly int GetHashCode()
        {
            return top.GetHashCode() ^ (bottom.GetHashCode() << 2) ^ (left.GetHashCode() >> 2) ^ (right.GetHashCode() >> 1);
        }

        public override bool Equals(object other)
        {
            if (other is not BorderData)
            {
                return false;
            }

            return Equals((BorderData)other);
        }

        public bool Equals(BorderData other)
        {
            return top == other.top && bottom == other.bottom && left == other.left && right == other.right;
        }
    }
}

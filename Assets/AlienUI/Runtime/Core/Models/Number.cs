using System;

namespace AlienUI.Models
{
    public struct Number : IComparable<Number>
    {
        public bool Auto;
        public float Value;
        public static Number Identity => new() { Auto = true, Value = 100f };

        public static Number operator +(Number a, Number b)
        {
            a.Value += b.Value;
            return a;
        }

        public static Number operator -(Number a, Number b)
        {
            a.Value -= b.Value;
            return a;
        }

        public static implicit operator Number(float a)
        {
            return new() { Auto = false, Value = a };
        }

        public int CompareTo(Number other)
        {
            if (Auto == other.Auto && Value == other.Value) return 0;
            else return Value.CompareTo(other.Value);
        }
    }
}

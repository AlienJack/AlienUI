namespace AlienUI.Models
{
    public struct Number
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
    }
}

namespace AlienUI.Models
{
    public struct Number
    {
        public bool Auto;
        public float Value;
        public static Number Identity => new() { Auto = true, Value = 100f };
    }
}

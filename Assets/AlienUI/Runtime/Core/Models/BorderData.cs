namespace AlienUI.Models
{
    public struct BorderData
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
    }
}

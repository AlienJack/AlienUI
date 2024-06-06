namespace AlienUI.PropertyResolvers
{
    public class BoolResolver : PropertyResolver<bool>
    {
        protected override bool OnResolve(string originStr)
        {
            bool.TryParse(originStr, out var value);
            return value;
        }

        protected override bool OnLerp(bool from, bool to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }

        protected override string Reverse(bool value)
        {
            return value.ToString();
        }
    }
}

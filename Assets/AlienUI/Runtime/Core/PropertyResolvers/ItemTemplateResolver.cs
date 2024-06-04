using AlienUI.Models;

namespace AlienUI.PropertyResolvers
{
    public class ItemTemplateResolver : PropertyResolver<ItemTemplate>
    {
        protected override ItemTemplate OnResolve(string originStr)
        {
            return new ItemTemplate(originStr);
        }

        protected override ItemTemplate OnLerp(ItemTemplate from, ItemTemplate to, float progress)
        {
            if (progress >= 1) return to;
            else return from;
        }

        protected override string Reverse(ItemTemplate value)
        {
            return value.Name;
        }
    }
}

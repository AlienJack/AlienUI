using AlienUI.Models;

namespace AlienUI.Core.Converters
{
    public class DependencyObject2DependencyObjectRef : ConverterBase<DependencyObject, DependencyObjectRef>
    {
        protected override DependencyObjectRef OnConvert(DependencyObject src)
        {
            return new DependencyObjectRef(src);
        }

        protected override DependencyObject OnConvert(DependencyObjectRef src)
        {
            return src.Get(null);
        }
    }
}

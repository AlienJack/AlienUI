using AlienUI.Models;

namespace AlienUI.Core.Converters
{
    public class FloatToBorder : ConverterBase<float, BorderData>
    {
        protected override BorderData OnConvert(float src)
        {
            return new BorderData(src);
        }

        protected override float OnConvert(BorderData src)
        {
            return src.top;
        }
    }
}

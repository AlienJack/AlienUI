using AlienUI.Models;
using UnityEngine;

namespace AlienUI.Core.Converters
{
    public class FloatToBorder : ConverterBase<float, Border>
    {
        protected override Border OnConvert(float src)
        {
            return new Border(src);
        }

        protected override float OnConvert(Border src)
        {
            return src.top;
        }
    }
}

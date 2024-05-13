using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.Core.Converters
{
    public class DependencyObjectConverter : ConverterBase<DependencyObject, DependencyObjectRef>
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

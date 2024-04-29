using System;

namespace AlienUI.Core.Converters
{
    public abstract class ConverterBase
    {
        public abstract Type RawValueType { get; }
        public abstract Type TargetValueType { get; }
        public abstract object SrcToDst(object value);
        public abstract object DstToSrc(object value);
    }

    public abstract class ConverterBase<Src, Dst> : ConverterBase
    {
        public sealed override Type RawValueType => typeof(Src);
        public sealed override Type TargetValueType => typeof(Dst);

        public override object DstToSrc(object value)
        {
            return OnConvert((Dst)value);
        }

        public override object SrcToDst(object value)
        {
            return OnConvert((Src)value);
        }


        protected abstract Dst OnConvert(Src src);
        protected abstract Src OnConvert(Dst src);
    }
}

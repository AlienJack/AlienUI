namespace AlienUI.Core.Converters
{
    public class Object2String : ConverterBase<object, string>
    {
        protected override string OnConvert(object src)
        {
            return src.ToString();
        }

        protected override object OnConvert(string src)
        {
            throw new System.NotImplementedException();
        }
    }
}

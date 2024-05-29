namespace AlienUI.Models
{
    public class PropertyMetadata
    {
        public object DefaultValue { get; private set; }
        public string Group { get; internal set; }
        public bool IsReadOnly { get; private set; }
        public bool IsAmlDisable { get; private set; }
        public bool IsAmlOnly { get; private set; }
        public PropertyMetadata(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public PropertyMetadata(object defaultValue, string group) : this(defaultValue)
        {
            Group = group;
        }

        public PropertyMetadata ReadOnly()
        {
            IsReadOnly = true;
            return this;
        }

        public PropertyMetadata AmlDisable()
        {
            IsAmlDisable = true;
            return this;
        }

        public PropertyMetadata AmlOnly()
        {
            IsAmlOnly = true;
            return this;
        }
    }
}

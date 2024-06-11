using AlienUI.Core.Converters;
using AlienUI.Models;
using AlienUI.UIElements;
using AlienUI.UIElements.ToolsScript;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace AlienUI.Core
{
    public class Binding
    {
        public object Source { get; private set; }
        public string SourcePropertyName { get; private set; }
        public AmlNodeElement Target { get; private set; }
        public string TargetPropertyName { get; private set; }
        public DependencyProperty TargetProperty { get; private set; }
        public string SourceCode { get; private set; }

        private ConverterBase m_converter;

        private enum EnumBindMode
        {
            OneWay,
            OneTime,
            TwoWay,
        }

        public void Apply(string convertName, string mode, AmlNodeElement parent)
        {
            Enum.TryParse<EnumBindMode>(mode, true, out EnumBindMode bindMode);

            if (Source != null)
            {
                Type dstPropType = null;
                Type srcPropType = null;
                m_converter = Target.Engine.GetConverter(convertName);
                if (m_converter == null)
                {
                    TargetProperty = Target.GetDependencyProperty(TargetPropertyName);
                    if (TargetProperty != null)
                        dstPropType = TargetProperty.PropType;
                    else if (parent != null)//检查是否是附加属性
                    {
                        var parentDP = parent.GetDependencyProperty(TargetPropertyName);
                        if (parentDP != null && parentDP.IsAttachedProerty)
                        {
                            TargetProperty = parentDP;
                            dstPropType = parentDP.PropType;
                        }
                    }
                    srcPropType = AlienUtility.GetPropertyType(Source, SourcePropertyName);
                    if (dstPropType == null) Engine.LogError($"Binding {Target.GetType()} has no such property named {TargetPropertyName}");
                    if (srcPropType == null) Engine.LogError($"Binding {Source.GetType()} has no such property named {SourcePropertyName}");
                    if (dstPropType != srcPropType && !dstPropType.IsAssignableFrom(srcPropType))
                    {
                        m_converter = Target.Engine.GetConverter(srcPropType, dstPropType);
                        if (m_converter == null)
                        {
                            Engine.LogError($"<color=blue>{Source.GetType()}</color>.<color=white>{srcPropType}</color>与<color=blue>{Target.GetType()}</color>.<color=white>{dstPropType}</color>类型不一致,并且没有找到可用的Converter");
                            return;
                        }
                    }
                }

                var srcValue = AlienUtility.GetPropertyValue(Source, SourcePropertyName);
                if (m_converter != null) srcValue = m_converter.SrcToDst(srcValue);

                //首先同步一次数值
                Target.FillDependencyValue(TargetProperty, srcValue);

                switch (bindMode)
                {
                    case EnumBindMode.OneWay:
                        {
                            if (Source is DependencyObject dpSource) DependencyProperty.Subscribe(dpSource, SourcePropertyName, OnSourceDataChanged);
                            else if (Source is INotifyPropertyChanged notifyObj)
                            {
                                notifyObj.PropertyChanged += SourcePropertyChanged;
                            }
                        }
                        break;
                    case EnumBindMode.TwoWay:
                        {
                            if (Source is DependencyObject dpSource) DependencyProperty.Subscribe(dpSource, SourcePropertyName, OnSourceDataChanged);
                            else if (Source is INotifyPropertyChanged notifyObj)
                            {
                                notifyObj.PropertyChanged += SourcePropertyChanged;
                            }
                            DependencyProperty.Subscribe(Target, TargetPropertyName, OnTargetDataChanged);
                        }
                        break;
                }

                if (TargetProperty == null) Engine.LogError($"Binding Target Property {TargetPropertyName} not exist");
                else TargetProperty.SetBinding(this);
            }
        }

        private bool m_dataSync = false;

        public Binding(string sourceCode)
        {
            SourceCode = sourceCode;
        }

        public void Disconnect()
        {
            if (Source is DependencyObject dpObj) DependencyProperty.Unsubscribe(dpObj, SourcePropertyName, OnSourceDataChanged);
            else if (Source is INotifyPropertyChanged inotify) inotify.PropertyChanged -= SourcePropertyChanged;
            DependencyProperty.Unsubscribe(Target, TargetPropertyName, OnTargetDataChanged);

            if (TargetProperty != null) TargetProperty.RemoveBinding(this);
        }

        private void OnTargetDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (m_dataSync) return;
            if (Source == null) return;

            m_dataSync = true;

            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;
            try { Source.SetPropertyValue(SourcePropertyName, newValue); }
            finally { m_dataSync = false; }
        }

        private void OnSourceDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (m_dataSync) return;

            m_dataSync = true;

            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;

            try { Target.SetValue(TargetProperty, newValue); }
            finally { m_dataSync = false; }
        }


        private void SourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SourcePropertyName != e.PropertyName) return;

            if (m_dataSync) return;

            m_dataSync = true;

            var newValue = AlienUtility.GetPropertyValue(sender, e.PropertyName);
            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;

            try { Target.SetValue(TargetProperty, newValue); }
            finally { m_dataSync = false; }
        }

        public class BindSourceProperty
        {
            private Binding bind;

            internal BindSourceProperty(Binding bind, object source)
            {
                this.bind = bind;
                this.bind.Source = source;
            }

            public BindTarget SetSourceProperty(string property)
            {
                var target = new BindTarget(bind);
                bind.SourcePropertyName = property;
                return target;
            }
        }

        public class BindTarget
        {
            private Binding bind;

            internal BindTarget(Binding bind)
            {
                this.bind = bind;
            }

            public BindTargetProperty SetTarget(AmlNodeElement target)
            {
                bind.Target = target;
                return new BindTargetProperty(bind);
            }
        }

        public class BindTargetProperty
        {
            private Binding bind;

            public BindTargetProperty(Binding bind)
            {
                this.bind = bind;
            }

            public Binding SetTargetProperty(string property)
            {
                bind.TargetPropertyName = property;
                return bind;
            }
        }

        public static BindSourceProperty Create(object source, string sourceCode)
        {
            var bind = new Binding(sourceCode);
            return new BindSourceProperty(bind, source);
        }
    }
    public enum EnumBindingType
    {
        InValid,
        Binding,
        TemplateBinding
    }
    internal static class BindUtility
    {
        public static Binding.BindSourceProperty BeginBind(this object source, string sourceCode)
        {
            return Binding.Create(source, sourceCode);
        }

        public static bool IsBindingString(string input, out Match match)
        {
            match = Regex.Match(input, @"{(.*?) (.*?)}", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public static EnumBindingType ParseBindParam(Match match, out string propName, out string converterName, out string modeName)
        {
            propName = string.Empty;
            converterName = string.Empty;
            modeName = string.Empty;

            if (!Enum.TryParse(match.Groups[1].Value, out EnumBindingType bindType))
                return EnumBindingType.InValid;

            var parameters = match.Groups[2].Value.Split(',');
            propName = parameters[0]; // 第一个参数是属性名
            for (int i = 1; i < parameters.Length; i++)
            {
                var pair = parameters[i].Split('=');
                if (pair.Length == 2)
                {
                    if (pair[0].Equals("Converter", System.StringComparison.OrdinalIgnoreCase))
                    {
                        converterName = pair[1];
                    }
                    else if (pair[0].Equals("Mode", System.StringComparison.OrdinalIgnoreCase))
                    {
                        modeName = pair[1];
                    }
                }
            }

            return bindType;
        }
    }
}

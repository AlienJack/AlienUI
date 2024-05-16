using AlienUI.Core.Converters;
using AlienUI.Models;
using AlienUI.UIElements;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlienUI.Core
{
    public class Binding
    {
        public DependencyObject Source { get; private set; }
        public string SourceProperty { get; private set; }
        public AmlNodeElement Target { get; private set; }
        public string TargetProperty { get; private set; }
        public string SourceCode { get; private set; }

        private ConverterBase m_converter;

        private enum EnumBindMode
        {
            OneWay,
            OneTime,
            TwoWay,
        }

        public void Apply(string convertName, string mode)
        {
            Enum.TryParse<EnumBindMode>(mode, true, out EnumBindMode bindMode);

            if (Source != null)
            {

                m_converter = Target.Engine.GetConverter(convertName);
                if (m_converter == null)
                {
                    var dstPropType = Target.GetDependencyPropertyType(TargetProperty);
                    var srcPropType = Source.GetDependencyPropertyType(SourceProperty);
                    if (dstPropType == null) Engine.LogError($"Binding {Target.GetType()} has no such property named {TargetProperty}");
                    if (srcPropType == null) Engine.LogError($"Binding {Source.GetType()} has no such property named {SourceProperty}");
                    if (dstPropType != srcPropType && !srcPropType.IsSubclassOf(dstPropType))
                    {
                        m_converter = Target.Engine.GetConverter(srcPropType, dstPropType);
                        if (m_converter == null)
                        {
                            Engine.LogError($"<color=blue>{Source.GetType()}</color>.<color=white>{srcPropType}</color>与<color=blue>{Target.GetType()}</color>.<color=white>{dstPropType}</color>类型不一致,并且没有找到可用的Converter");
                            return;
                        }
                    }
                }

                var srcValue = Source.GetValue(SourceProperty);
                if (m_converter != null) srcValue = m_converter.SrcToDst(srcValue);

                //首先同步一次数值
                Target.FillDependencyValue(TargetProperty, srcValue);

                switch (bindMode)
                {
                    case EnumBindMode.OneWay:
                        DependencyProperty.Subscribe(Source, SourceProperty, OnSourceDataChanged);
                        break;
                    case EnumBindMode.TwoWay:
                        DependencyProperty.Subscribe(Source, SourceProperty, OnSourceDataChanged);
                        DependencyProperty.Subscribe(Target, TargetProperty, OnTargetDataChanged);
                        break;
                }
            }

            var targetproperty = Target.GetDependencyProperty(TargetProperty);

            if (targetproperty != null)
            {
                targetproperty.SetBinding(this);
            }
        }

        private bool m_dataSync = false;

        public Binding(string sourceCode)
        {
            SourceCode = sourceCode;
        }

        public void Disconnect()
        {
            DependencyProperty.Unsubscribe(Source, SourceProperty, OnSourceDataChanged);
            DependencyProperty.Unsubscribe(Target, TargetProperty, OnTargetDataChanged);

            var p = Target.GetDependencyProperty(TargetProperty);
            if (p != null) p.RemoveBinding(this);
        }

        private void OnTargetDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (m_dataSync) return;

            m_dataSync = true;

            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;
            try { Source.SetValue(SourceProperty, newValue); }
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

        public class BindSourceProperty
        {
            private Binding bind;

            internal BindSourceProperty(Binding bind, DependencyObject source)
            {
                this.bind = bind;
                this.bind.Source = source;
            }

            public BindTarget SetSourceProperty(string property)
            {
                var target = new BindTarget(bind);
                bind.SourceProperty = property;
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
                bind.TargetProperty = property;
                return bind;
            }
        }

        public static BindSourceProperty Create(DependencyObject source, string sourceCode)
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
    public static class BindUtility
    {
        public static Binding.BindSourceProperty BeginBind(this DependencyObject source, string sourceCode)
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

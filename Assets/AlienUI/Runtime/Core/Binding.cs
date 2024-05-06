using AlienUI.Core.Converters;
using AlienUI.Models;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlienUI.Core
{
    public class Binding
    {
        public DependencyObject Source { get; private set; }
        public string SourceProperty { get; private set; }
        public DependencyObject Target { get; private set; }
        public string TargetProperty { get; private set; }

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

            m_converter = Target.Engine.GetConverter(convertName);
            if (m_converter == null)
            {
                var dstPropType = Target.GetDependencyPropertyType(TargetProperty);
                var srcPropType = Source.GetDependencyPropertyType(SourceProperty);
                if (dstPropType != srcPropType && !srcPropType.IsSubclassOf(dstPropType))
                {
                    m_converter = Target.Engine.GetConverter(srcPropType, dstPropType);
                    if (m_converter == null)
                    {
                        Debug.LogError($"<color=blue>{Source.GetType()}</color>.<color=white>{srcPropType}</color>与<color=blue>{Target.GetType()}</color>.<color=white>{dstPropType}</color>类型不一致,并且没有找到可用的Converter");
                        return;
                    }
                }
            }

            var srcValue = Source.GetValue(SourceProperty);
            if (m_converter != null) srcValue = m_converter.SrcToDst(srcValue);

            //首先同步一次数值
            Target.SetValue(TargetProperty, srcValue, false);

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

        private bool m_dataSync = false;

        public void Disconnect()
        {
            DependencyProperty.Unsubscribe(Source, SourceProperty, OnSourceDataChanged);
        }

        private void OnTargetDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (m_dataSync) return;

            m_dataSync = true;

            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;
            try { Source.SetValue(SourceProperty, newValue, true); }
            finally { m_dataSync = false; }
        }

        private void OnSourceDataChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (m_dataSync) return;

            m_dataSync = true;

            newValue = m_converter != null ? m_converter.SrcToDst(newValue) : newValue;

            try { Target.SetValue(TargetProperty, newValue, true); }
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

            public BindTargetProperty SetTarget(DependencyObject target)
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

        public static BindSourceProperty Create(DependencyObject source)
        {
            var bind = new Binding();
            return new BindSourceProperty(bind, source);
        }
    }

    public static class BindUtility
    {
        public static Binding.BindSourceProperty BeginBind(this DependencyObject source)
        {
            return Binding.Create(source);
        }

        public static bool IsBindingString(string input)
        {
            return Regex.Match(input, @"{Binding (.+)}", RegexOptions.IgnoreCase).Success;
        }

        public static void ParseBindParam(string input, out string propName, out string converterName, out string modeName)
        {
            propName = string.Empty;
            converterName = string.Empty;
            modeName = string.Empty;

            var matches = Regex.Matches(input, @"{Binding (.*?)}", RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                var parameters = matches[0].Groups[1].Value.Split(',');
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
            }
        }
    }
}

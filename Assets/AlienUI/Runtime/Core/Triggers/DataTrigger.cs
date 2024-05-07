using AlienUI.Core.Triggers;
using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI.Core.Triggers
{
    public class DataTrigger : Trigger
    {
        public string PropertyName
        {
            get { return (string)GetValue(PropertyNameProperty); }
            set { SetValue(PropertyNameProperty, value); }
        }
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.Register("PropertyName", typeof(string), typeof(DataTrigger), string.Empty);

        public ConditionSets Condition
        {
            get { return (ConditionSets)GetValue(ConditionProperty); }
            set { SetValue(ConditionProperty, value); }
        }
        public static readonly DependencyProperty ConditionProperty =
            DependencyProperty.Register("Condition", typeof(ConditionSets), typeof(DataTrigger), default(ConditionSets));

        private DependencyProperty dp;

        protected override void OnInit(UIElement host)
        {
            dp = host.GetDependencyProperty(PropertyName);
            dp.OnValueChanged += Dp_OnValueChanged;

            for (int i = 0; i < Condition.Conditions.Count; i++)
            {
                var resolveCondition = Condition.Conditions[i].ResolveCompareValue(Engine, dp.PropType);
                Condition.Conditions[i] = resolveCondition;
            }
        }

        private void Dp_OnValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            if (sender != m_host) return;

            if (Condition.Test(newValue))
            {
                Execute();
            }
        }
    }
}

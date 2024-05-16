using AlienUI.Core.Resources;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using UnityEngine;
using static AlienUI.UIElements.Button;

namespace AlienUI.Core.Triggers
{
    public class DataTrigger : Trigger
    {
        private List<Condition> m_conditions = new List<Condition>();

        protected override void OnInit()
        {
            m_targetObj.OnDependencyPropertyChanged += M_targetObj_OnDependencyPropertyChanged;
        }

        private void M_targetObj_OnDependencyPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            if (Test(dp.PropName))
            {
                if ((EnumButtonState)oldValue == EnumButtonState.Pressing)
                {
                    Debug.Log("!");
                }
                Execute();
            }
        }

        HashSet<string> m_focusProperties = new HashSet<string>();
        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case Condition cond:
                    m_conditions.Add(cond);
                    m_focusProperties.Add(cond.PropertyName);
                    cond.ResolveCompareValue(m_targetObj);
                    break;
            }

            base.OnAddChild(childObj);
        }

        protected override void OnRemoveChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case Condition cond:
                    m_conditions.Remove(cond);
                    m_focusProperties.Remove(cond.PropertyName);
                    cond.ResolveCompareValue(m_targetObj);
                    break;
            }

            base.OnRemoveChild(childObj);
        }

        private bool Test(string propName)
        {
            if (!m_focusProperties.Contains(propName)) return false;

            if (m_conditions.Count == 0) return false;

            foreach (var con in m_conditions)
            {
                if (!con.Test())
                    return false;
            }

            return true;
        }

    }
}

using AlienUI.Core.Resources;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;

namespace AlienUI.Core.Triggers
{
    public class DataTrigger : Trigger
    {
        private List<Condition> m_conditions = new List<Condition>();

        protected override void OnInit()
        {
            m_targetObj.OnDependencyPropertyChanged += M_targetObj_OnDependencyPropertyChanged;
        }

        private void M_targetObj_OnDependencyPropertyChanged(string propName, object oldValue, object newValue)
        {
            if (Test(propName))
                Execute();
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

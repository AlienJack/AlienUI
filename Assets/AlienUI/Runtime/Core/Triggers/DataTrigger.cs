using AlienUI.Core.Resources;
using AlienUI.Models;
using AlienUI.Models.Attributes;
using AlienUI.UIElements;
using System.Collections.Generic;

namespace AlienUI.Core.Triggers
{
    [AllowChild(typeof(Condition))]
    public class DataTrigger : Trigger
    {
        protected override void OnInit()
        {
            m_targetObj.OnDependencyPropertyChanged += M_targetObj_OnDependencyPropertyChanged;
        }

        protected override void OnDispose()
        {
            m_targetObj.OnDependencyPropertyChanged -= M_targetObj_OnDependencyPropertyChanged;
        }

        protected override void OnDocumentPerformed()
        {
            foreach (var propName in m_focusProperties)
            {
                if (Test(propName)) Execute();
            }
        }

        private void M_targetObj_OnDependencyPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            if (Test(dp.PropName))
            {
                Execute();
            }
        }

        HashSet<string> m_focusProperties = new HashSet<string>();
        protected override void OnAddChild(AmlNodeElement childObj)
        {
            switch (childObj)
            {
                case Condition cond:
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
                    m_focusProperties.Remove(cond.PropertyName);
                    cond.ResolveCompareValue(m_targetObj);
                    break;
            }

            base.OnRemoveChild(childObj);
        }

        private bool Test(string propName)
        {
            if (!m_focusProperties.Contains(propName)) return false;

            var conditions = GetChildren<Condition>();
            if (conditions.Count == 0) return false;

            foreach (var con in conditions)
            {
                if (!con.Test())
                    return false;
            }

            return true;
        }

    }
}

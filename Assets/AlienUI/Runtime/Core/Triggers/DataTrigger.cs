using AlienUI.Models;

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
            DependencyProperty.Register("PropertyName", typeof(string), typeof(DataTrigger), new PropertyMetadata(null));

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
            Execute();
        }

        private void M_targetObj_OnDependencyPropertyChanged(DependencyProperty dp, object oldValue, object newValue)
        {
            if (dp.PropName == PropertyName)
                Execute();
        }

    }
}

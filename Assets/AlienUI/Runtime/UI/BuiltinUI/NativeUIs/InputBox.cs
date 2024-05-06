
using AlienUI.Models;
using UnityEngine.UI;

namespace AlienUI.UIElements
{
    public class InputBox : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InputBox), string.Empty, OnTextChanged);

        private static void OnTextChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.SetTextWithoutNotify((string)newValue);
        }

        private InputField m_inputField;
        protected override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.InputBox");

        protected override void OnInitialized()
        {
            base.OnInitialized();
            m_inputField = m_rectTransform.gameObject.AddComponent<InputField>();
            m_inputField.onValueChanged.AddListener(OnInputFieldTextChanged);
            if (m_templateInstance != null)
            {
                var phLabel = m_templateInstance.Document.Query<Label>("PlaceHolder");
                if (phLabel != null && phLabel.UGUIText != null)
                    m_inputField.placeholder = phLabel.UGUIText;

                var textComLabel = m_templateInstance.Document.Query<Label>("TextCom");
                if (textComLabel != null && textComLabel.UGUIText != null)
                {
                    m_inputField.textComponent = textComLabel.UGUIText;
                    textComLabel.UGUIText.supportRichText = false;
                }
            }

            OnUpdateSelected += InputBox_OnUpdateSelected;
            OnBeginDrag += InputBox_OnBeginDrag;
            OnDrag += InputBox_OnDrag;
            OnEndDrag += InputBox_OnEndDrag;
            OnPointerClick += InputBox_OnPointerClick;
            OnSubmit += InputBox_OnSubmit;
        }

        private void OnInputFieldTextChanged(string arg0)
        {
            Text = arg0;
        }

        #region HandleUGUIEvents
        private void InputBox_OnSubmit(object sender, Events.OnSubmitEvent e)
        {
            m_inputField.OnSubmit(e.EvtData);
        }

        private void InputBox_OnPointerClick(object sender, Events.OnPointerClickEvent e)
        {
            m_inputField.OnPointerClick(e.EvtData);
        }

        private void InputBox_OnEndDrag(object sender, Events.OnEndDragEvent e)
        {
            m_inputField.OnEndDrag(e.EvtData);
        }

        private void InputBox_OnDrag(object sender, Events.OnDragEvent e)
        {
            m_inputField.OnDrag(e.EvtData);
        }

        private void InputBox_OnBeginDrag(object sender, Events.OnBeginDragEvent e)
        {
            m_inputField.OnBeginDrag(e.EvtData);
        }

        private void InputBox_OnUpdateSelected(object sender, Events.OnUpdateSelectedEvent e)
        {
            m_inputField.OnUpdateSelected(e.EvtData);
        }
        #endregion
    }
}

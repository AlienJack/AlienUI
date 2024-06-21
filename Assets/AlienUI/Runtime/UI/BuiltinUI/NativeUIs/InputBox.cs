
using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace AlienUI.UIElements
{
    [Description(Icon = "inputbox")]
    public class InputBox : UserControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(InputBox), new PropertyMetadata(string.Empty), OnTextChanged);
        private static void OnTextChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.SetTextWithoutNotify((string)newValue);
        }

        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(InputBox), new PropertyMetadata("Write here"));

        public InputField.InputType InputType
        {
            get { return (InputField.InputType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }
        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(InputField.InputType), typeof(InputBox), new PropertyMetadata(InputField.InputType.Standard), OnInputTypeChanged);
        private static void OnInputTypeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.inputType = (InputField.InputType)newValue;
        }

        public Color CaretColor
        {
            get { return (Color)GetValue(CaretColorProperty); }
            set { SetValue(CaretColorProperty, value); }
        }

        public static readonly DependencyProperty CaretColorProperty =
            DependencyProperty.Register("CaretColor", typeof(Color), typeof(InputBox), new PropertyMetadata(Color.white), OnCaretColorChanged);

        private static void OnCaretColorChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.caretColor = self.CaretColor;
        }

        private InputField m_inputField;
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.InputBox");

        protected override void OnInitialized()
        {
            m_inputField = m_rectTransform.gameObject.AddComponent<InputField>();
            m_inputField.onValueChanged.AddListener(OnInputFieldTextChanged);

            HandleUGUIInputFieldEvent();
        }

        protected override void OnInteractableValueChanged()
        {
            m_inputField.interactable = Interactable;
        }

        protected override void OnTemplateApply()
        {
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
        }

        private void TemplateProperty_OnValueChanged(DependencyObject sender, object oldValue, object newValue)
        {
            throw new System.NotImplementedException();
        }

        private void OnInputFieldTextChanged(string arg0)
        {
            Text = arg0;
        }



        #region HandleUGUIEvents
        private void HandleUGUIInputFieldEvent()
        {
            OnBeginDrag += InputBox_OnBeginDrag;
            OnDrag += InputBox_OnDrag;
            OnEndDrag += InputBox_OnEndDrag;
            OnPointerClick += InputBox_OnPointerClick;
            OnPointerDown += InputBox_OnPointerDown;
            OnPointerUp += InputBox_OnPointerUp;
            OnPointerEnter += InputBox_OnPointerEnter;
            OnPointerExit += InputBox_OnPointerExit;
        }

        private void InputBox_OnPointerExit(object sender, Events.OnPointerExitEvent e)
        {
            m_inputField.OnPointerExit(e.EvtData);
        }

        private void InputBox_OnPointerEnter(object sender, Events.OnPointerEnterEvent e)
        {
            m_inputField.OnPointerEnter(e.EvtData);
        }

        private void InputBox_OnPointerUp(object sender, Events.OnPointerUpEvent e)
        {
            m_inputField.OnPointerUp(e.EvtData);
        }

        private void InputBox_OnPointerDown(object sender, Events.OnPointerDownEvent e)
        {
            m_inputField.OnPointerDown(e.EvtData);
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
        #endregion
    }
}


using AlienUI.Models;
using System;
using TMPro;
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

        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(InputBox), "Write here");

        private static void OnTextChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.SetTextWithoutNotify((string)newValue);
        }



        public InputField.InputType InputType
        {
            get { return (InputField.InputType)GetValue(InputTypeProperty); }
            set { SetValue(InputTypeProperty, value); }
        }

        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register("InputType", typeof(InputField.InputType), typeof(InputBox), InputField.InputType.Standard,OnInputTypeChanged);

        private static void OnInputTypeChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as InputBox;
            self.m_inputField.inputType = (InputField.InputType)newValue;
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

            HandleUGUIInputFieldEvent();

        }

        private void OnInputFieldTextChanged(string arg0)
        {
            Text = arg0;
        }



        #region HandleUGUIEvents
        private void HandleUGUIInputFieldEvent()
        {
            OnUpdateSelected += InputBox_OnUpdateSelected;
            OnBeginDrag += InputBox_OnBeginDrag;
            OnDrag += InputBox_OnDrag;
            OnEndDrag += InputBox_OnEndDrag;
            OnPointerClick += InputBox_OnPointerClick;
            OnSubmit += InputBox_OnSubmit;
            OnMove += InputBox_OnMove;
            OnPointerDown += InputBox_OnPointerDown;
            OnPointerUp += InputBox_OnPointerUp;
            OnPointerEnter += InputBox_OnPointerEnter;
            OnPointerExit += InputBox_OnPointerExit;
            OnSelect += InputBox_OnSelect;
            OnDeselect += InputBox_OnDeselect;
        }


        private void InputBox_OnDeselect(object sender, Events.OnDeselectEvent e)
        {
            m_inputField.OnDeselect(e.EvtData);
        }

        private void InputBox_OnSelect(object sender, Events.OnSelectEvent e)
        {
            m_inputField.OnSelect(e.EvtData);
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

        private void InputBox_OnMove(object sender, Events.OnMoveEvent e)
        {
            m_inputField.OnMove(e.EvtData);
        }


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

using AlienUI.Core.Commnands;
using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    [Description(Icon = "window")]
    public class Window : UserControl
    {
        public override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Window");

        public Command SwitchMaximizStateCmd
        {
            get { return (Command)GetValue(SwitchMaximizStateProperty); }
            set { SetValue(SwitchMaximizStateProperty, value); }
        }

        public static readonly DependencyProperty SwitchMaximizStateProperty =
            DependencyProperty.Register("SwitchMaximizStateCmd", typeof(Command), typeof(Window), new PropertyMetadata(null).AmlDisable());

        public Command CloseCmd
        {
            get { return (Command)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("CloseCmd", typeof(Command), typeof(Window), new PropertyMetadata(null).AmlDisable());

        public bool IsMaximized
        {
            get { return (bool)GetValue(IsMaximizedProperty); }
            set { SetValue(IsMaximizedProperty, value); }
        }
        public static readonly DependencyProperty IsMaximizedProperty =
            DependencyProperty.Register("IsMaximized", typeof(bool), typeof(Window), new PropertyMetadata(false), OnIsMaximizedChanged);



        public bool CanDragMove
        {
            get { return (bool)GetValue(CanDragMoveProperty); }
            set { SetValue(CanDragMoveProperty, value); }
        }

        public static readonly DependencyProperty CanDragMoveProperty =
            DependencyProperty.Register("CanDragMove", typeof(bool), typeof(Window), new PropertyMetadata(true));

        private Vector2 m_orOffset;
        private eHorizontalAlign m_orH_Align;
        private eVerticalAlign m_orV_Align;
        private static void OnIsMaximizedChanged(DependencyObject sender, object oldValue, object newValue)
        {
            var self = sender as Window;
            if (self.IsMaximized)
            {
                self.m_orOffset = self.Offset;
                self.m_orH_Align = self.Horizontal;
                self.m_orV_Align = self.Vertical;

                self.Offset = default;
                self.Horizontal = eHorizontalAlign.Stretch;
                self.Vertical = eVerticalAlign.Stretch;
            }
            else if ((bool)oldValue)
            {
                self.Offset = self.m_orOffset;
                self.Horizontal = self.m_orH_Align;
                self.Vertical = self.m_orV_Align;
            }
        }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Window), new PropertyMetadata(nameof(Window)));


        protected override void OnInitialized()
        {
            CloseCmd = new Command();
            CloseCmd.OnExecute += Close_OnExecute;
            SwitchMaximizStateCmd = new Command();
            SwitchMaximizStateCmd.OnExecute += SwitchMaximizStateCmd_OnExecute;
            OnDrag += Window_OnDrag;
        }

        private void SwitchMaximizStateCmd_OnExecute()
        {
            IsMaximized = !IsMaximized;
        }

        private void Window_OnDrag(object sender, Events.OnDragEvent e)
        {
            if (!CanDragMove) return;

            if (IsMaximized)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    Rect, e.EvtData.position, NodeProxy.Canvas.worldCamera, out Vector2 localPos);

                m_orOffset = localPos;

                IsMaximized = false;
            }

            Offset += e.EvtData.delta / NodeProxy.Canvas.scaleFactor;
        }

        private void Close_OnExecute()
        {
            Close();
        }
    }
}

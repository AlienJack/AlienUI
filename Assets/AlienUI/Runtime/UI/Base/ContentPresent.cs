using AlienUI.Models;
using AlienUI.Models.Attributes;
using System;
using UnityEngine;

namespace AlienUI.UIElements
{
    public abstract class ContentPresent<T> : ContentPresent
    {
        public T Content
        {
            get { return (T)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(T), typeof(ContentPresent<T>), default(T), OnContentChangedHandle);

        private static void OnContentChangedHandle(DependencyObject sender, object oldValue, object newValue)
        {
            var host = (sender as ContentPresent);
            host.RaiseContentChanged(oldValue, newValue);
        }

        public sealed override void RaiseContentChanged(object oldValue, object newValue)
        {
            OnContentChanged((T)oldValue, (T)newValue);
        }

        protected virtual void OnContentChanged(T oldValue, T newValue) { }
    }

    public abstract class ContentPresent : UIElement
    {
        public abstract void RaiseContentChanged(object oldValue, object newValue);
    }
}

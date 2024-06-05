using AlienUI.Models;

namespace AlienUI.UIElements
{
    public abstract class ContentPresent<T> : ContentPresent
    {
        public T Content
        {
            get { return (T)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(T), typeof(ContentPresent<T>), new PropertyMetadata(default(T)), OnContentChangedHandle);

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

    public abstract class ContentPresent : VisualElement
    {
        public abstract void RaiseContentChanged(object oldValue, object newValue);
    }
}

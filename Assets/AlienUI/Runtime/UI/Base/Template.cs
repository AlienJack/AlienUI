using AlienUI.Models;
using AlienUI.Models.Attributes;

namespace AlienUI.UIElements
{
    [Description(Icon = "template")]
    public class Template : VisualElement
    {
        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("Type", typeof(string), typeof(Template), new PropertyMetadata(string.Empty, "Data"));



        public DependencyObjectRef TemplateRoot
        {
            get { return (DependencyObjectRef)GetValue(TemplateRootProperty); }
            set { SetValue(TemplateRootProperty, value); }
        }

        public static readonly DependencyProperty TemplateRootProperty =
            DependencyProperty.Register("TemplateRoot", typeof(DependencyObjectRef), typeof(Template), new PropertyMetadata(default(DependencyObjectRef)));

    }
}

using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Window : UserControl
    {
        protected override ControlTemplate DefaultTemplate => new ControlTemplate("Builtin.Window");
    }
}

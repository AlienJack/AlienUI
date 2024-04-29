using AlienUI.Models;
using UnityEngine;

namespace AlienUI.UIElements
{
    public class Window : UserControl
    {
        protected override TextAsset DefaultTemplate => Engine.Settings.GetTemplate("Builtin.Window");
    }
}

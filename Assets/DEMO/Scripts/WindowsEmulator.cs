using AlienUI;
using AlienUI.UIElements;
using AlienUI.UIManager;
using UnityEngine;

namespace DEMO
{
    public class WindowsEmulator : MonoBehaviour
    {
        public UIManager UIManager;

        private void Start()
        {
            var uiAsset = Settings.Get().GetWindowAsset("LoginScreen");
            UIManager.OpenWiondow<Window>(uiAsset);            
        }
    }
}

using AlienUI;
using AlienUI.UIElements;
using AlienUI.UIManager;
using System.Collections;
using UnityEngine;

namespace DEMO
{
    public class WindowsEmulator : MonoBehaviour
    {
        public UIManager UIManager;

        private IEnumerator Start()
        {
            var setting = Settings.Get();

            yield return null;

            var uiAsset = setting.GetWindowAsset("LoginScreen");
            yield return null;
            UIManager.OpenWiondow<Window>(uiAsset);
        }
    }
}

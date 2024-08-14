using AlienUI;
using AlienUI.UIElements;
using AlienUI.UIManager;
using System.Collections;
using UnityEngine;

namespace DEMO
{
    public class WindowsEmulator : MonoBehaviour
    {
        public static WindowsEmulator Instance { get; private set; }

        public UIManager UIManager;
        public Engine Engine => UIManager.Engine;

        private void Awake()
        {
            Instance = this;
        }

        private IEnumerator Start()
        {
            var setting = Settings.Get();

            yield return null;

            var uiAsset = setting.GetWindowAsset("LoginScreen");
            yield return null;
            UIManager.OpenWindow<Window>(uiAsset, new LoginScreenVM());
        }
    }
}

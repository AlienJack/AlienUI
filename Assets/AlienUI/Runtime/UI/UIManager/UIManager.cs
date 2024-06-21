using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections.Generic;
using UnityEngine;

namespace AlienUI.UIManager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_uiRoot;
        [SerializeField]
        private Engine m_engine;

        public Engine Engine => m_engine;

        private RectTransform m_hudRoot;
        private RectTransform m_windowRoot;
        private RectTransform m_floatRoot;

        Dictionary<Settings.AmlResouces, HashSet<Window>> m_openedWindowsMap = new Dictionary<Settings.AmlResouces, HashSet<Window>>();
        Dictionary<Window, Settings.AmlResouces> m_openedWindows = new Dictionary<Window, Settings.AmlResouces>();

        private void Awake()
        {
            if (gameObject.transform.parent == null) DontDestroyOnLoad(gameObject);
            if (m_engine.transform.parent == null) DontDestroyOnLoad(m_engine.gameObject);
            if (m_uiRoot.transform.parent == null) DontDestroyOnLoad(m_uiRoot.gameObject);

            m_hudRoot = createUINode("[HUD]");
            m_windowRoot = createUINode("[Window]");
            m_floatRoot = createUINode("[Float]");
        }

        public T OpenWiondow<T>(Settings.AmlResouces amlRes, ViewModel viewModel = null) where T : Window
        {
            if (amlRes == null) return null;
            if (amlRes.Aml == null) return null;

            if (!typeof(T).IsAssignableFrom(amlRes.AssetType)) return null;

            var newUI = m_engine.CreateUI(amlRes.Aml, m_windowRoot, viewModel);

            if (!m_openedWindowsMap.ContainsKey(amlRes))
                m_openedWindowsMap[amlRes] = new HashSet<Window>();

            m_openedWindowsMap[amlRes].Add(newUI as Window);
            m_openedWindows[newUI as Window] = amlRes;

            newUI.OnFocusChanged += NewUI_OnFocusChanged;
            newUI.OnClose += NewUI_OnClose;

            m_engine.Focus(newUI);

            return newUI as T;
        }

        private void NewUI_OnClose(object sender, Events.OnCloseEvent e)
        {
            if (sender is Window wnd)
            {
                RemoveWindow(wnd);
            }
        }

        private void NewUI_OnFocusChanged(UIElement ui)
        {
            if (ui is Window wnd && wnd.Focused && HasWindow(wnd))
            {
                wnd.Rect.SetAsLastSibling();
            }
        }

        private void RemoveWindow(Window wnd)
        {
            var amlRes = GetWindowResource(wnd);
            if (amlRes == null) return;

            if (!m_openedWindowsMap.TryGetValue(amlRes, out var windowList)) return;

            windowList.Remove(wnd);
            m_openedWindows.Remove(wnd);

            wnd.OnFocusChanged -= NewUI_OnFocusChanged;
            wnd.OnClose -= NewUI_OnClose;
        }

        private Settings.AmlResouces GetWindowResource(Window wnd)
        {
            m_openedWindows.TryGetValue(wnd, out Settings.AmlResouces resouces);
            return resouces;
        }

        private bool HasWindow(Window wnd)
        {
            return GetWindowResource(wnd) != null;
        }

        private RectTransform createUINode(string name)
        {
            var nodeGo = new GameObject(name);
            var nodeRect = nodeGo.AddComponent<RectTransform>();
            nodeRect.SetParent(m_uiRoot);

            nodeRect.anchorMin = new Vector2(0, 0);
            nodeRect.anchorMax = new Vector2(1, 1);
            nodeRect.pivot = new Vector2(0.5f, 0.5f);
            nodeRect.sizeDelta = Vector2.zero;
            nodeRect.anchoredPosition3D = Vector3.zero;
            nodeRect.localScale = Vector2.one;

            return nodeRect;
        }


    }
}

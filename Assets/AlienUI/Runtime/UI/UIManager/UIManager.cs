using AlienUI.Models;
using AlienUI.UIElements;
using UnityEngine;

namespace AlienUI.UIManager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private RectTransform m_uiRoot;
        [SerializeField]
        private Engine m_engine;

        private RectTransform m_hudRoot;
        private RectTransform m_windowRoot;
        private RectTransform m_floatRoot;

        private void Awake()
        {
            if (gameObject.transform.parent == null) DontDestroyOnLoad(gameObject);
            if (m_engine.transform.parent == null) DontDestroyOnLoad(m_engine.gameObject);
            if (m_uiRoot.transform.parent == null) DontDestroyOnLoad(m_uiRoot.gameObject);

            m_hudRoot = createUINode("[HUD]");
            m_windowRoot = createUINode("[Window]");
            m_floatRoot = createUINode("[Float]");
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

        public T OpenWiondow<T>(Settings.AmlResouces amlRes, ViewModel viewModel = null) where T : Window
        {
            if (amlRes == null) return null;
            if (amlRes.Aml == null) return null;

            if (!typeof(T).IsAssignableFrom(amlRes.AssetType)) return null;

            var newUI = m_engine.CreateUI(amlRes.Aml, m_windowRoot, viewModel);

            return newUI as T;
        }
    }
}

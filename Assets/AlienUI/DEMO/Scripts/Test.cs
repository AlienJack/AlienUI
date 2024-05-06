using AlienUI;
using AlienUI.UIElements;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset ta;
    public Engine e;
    public RectTransform uiRoot;
    private Window m_window;

    private void Start()
    {
        m_window = e.CreateUI(ta.text, uiRoot, null) as Window;
    }

    private void Update()
    {
        //m_window.Width += 10 * Time.deltaTime;
        //m_window.Height += 10 * Time.deltaTime;
    }
}

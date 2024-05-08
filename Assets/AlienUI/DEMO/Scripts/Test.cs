using AlienUI;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AmlAsset ta;
    public Engine e;
    public RectTransform uiRoot;
    private Window m_window;

    private void Start()
    {
        m_window = e.CreateUI(ta.Text, uiRoot, new LoginVM()) as Window;
    }

    private void Update()
    {
        //m_window.Width += 10 * Time.deltaTime;
        //m_window.Height += 10 * Time.deltaTime;
    }
}

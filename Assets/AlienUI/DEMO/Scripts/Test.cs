using AlienUI;
using AlienUI.Models;
using AlienUI.UIElements;
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AmlAsset ta;
    public Settings settings;
    public Engine e;
    public RectTransform uiRoot;

    private void Start()
    {
        //resolve not in Editor Environment
        Settings.SettingGetter = () => settings;
        e.CreateUI(ta.Text, uiRoot, new LoginVM());
    }

    private void Update()
    {
        //m_window.Width += 10 * Time.deltaTime;
        //m_window.Height += 10 * Time.deltaTime;
    }
}

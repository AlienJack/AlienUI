using AlienUI;
using AlienUI.Models;
using UnityEngine;

public class Test : MonoBehaviour
{
    public AmlAsset ta;
    public Engine e;
    public RectTransform uiRoot;

    private void Start()
    {
        e.CreateUI(ta.Text, uiRoot, new LoginVM());
    }

}

using AlienUI;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset ta;
    public Engine e;
    public RectTransform uiRoot;

    private void Start()
    {
        e.CreateUI(ta.text, uiRoot, null);
    }

}

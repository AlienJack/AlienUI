using AlienUI;
using UnityEngine;

public class Test : MonoBehaviour
{
    public TextAsset ta;
    public Settings settings;
    private Engine e;

    private void Awake()
    {
        e = new Engine(settings);
        e.CreateUI(ta.text, transform, null);
    }

    private void LateUpdate()
    {
        e.Lateupdate();
    }
}

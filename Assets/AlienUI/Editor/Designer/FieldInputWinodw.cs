using System;
using UnityEditor;
using UnityEngine;

public class FieldInputWinodw : EditorWindow
{
    private Func<float> OnGUIAction;
    public static void ShowWindow(string title, Vector2 popPos, Func<float> onGui)
    {
        var wnd = GetWindow(typeof(FieldInputWinodw), true, title) as FieldInputWinodw;
        wnd.minSize = new Vector2(500, 45);
        wnd.maxSize = wnd.minSize;

        wnd.OnGUIAction = onGui;

        popPos = GUIUtility.GUIToScreenPoint(popPos);

        wnd.Show();

        var pos = wnd.position;
        pos.position = popPos;
        wnd.position = pos;

        wnd.ShowModal();
    }

    void OnGUI()
    {
        var height = OnGUIAction.Invoke();
        var temp = minSize;

        temp.y = height;

        minSize = temp;
        maxSize = temp;
    }
}
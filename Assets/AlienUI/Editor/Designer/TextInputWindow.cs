using System;
using UnityEditor;
using UnityEngine;

public class TextInputWindow : EditorWindow
{
    string userInput = "";
    private Action<string> userInputCallback;

    public static void ShowWindow(string title, string defaultInput, Vector2 popPos, Action<string> userInput)
    {
        var wnd = GetWindow(typeof(TextInputWindow), true, title) as TextInputWindow;
        wnd.userInputCallback = userInput;
        wnd.userInput = defaultInput;
        wnd.minSize = new Vector2(500, 45);
        wnd.maxSize = wnd.minSize;


        var pos = wnd.position;
        pos.position = popPos;
        wnd.position = pos;

        wnd.ShowModalUtility();

    }

    void OnGUI()
    {
        userInput = EditorGUILayout.TextField(userInput);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK", GUILayout.Width(50)))
        {
            try
            {
                userInputCallback?.Invoke(userInput);
            }
            finally
            {
                Close();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void OnDestroy()
    {
        userInputCallback = null;
    }
}
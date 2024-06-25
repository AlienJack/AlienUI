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

        popPos = GUIUtility.GUIToScreenPoint(popPos);

        wnd.firstShow = true;
        wnd.Show();

        var pos = wnd.position;
        pos.position = popPos;
        wnd.position = pos;

        wnd.ShowModal();
    }

    bool firstShow;
    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                Event.current.Use();
                return;
            }
            else if (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)
            {
                HandleInput();
                Event.current.Use();
                return;
            }
        }

        if (firstShow)
            GUI.SetNextControlName("InputFieldNeedFocus");

        userInput = EditorGUILayout.TextField(userInput);

        if (firstShow)
            EditorGUI.FocusTextInControl("InputFieldNeedFocus");
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK", GUILayout.Width(50)))
        {
            HandleInput();
        }
        EditorGUILayout.EndHorizontal();



        firstShow = false;
    }

    private void HandleInput()
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

    private void OnDestroy()
    {
        userInputCallback = null;
    }
}
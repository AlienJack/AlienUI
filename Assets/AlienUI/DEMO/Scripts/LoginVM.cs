using AlienUI;
using AlienUI.Core.Commnands;
using AlienUI.Models;
using System.Collections.Generic;
using UnityEngine;

public class LoginVM : ViewModel
{
    private string m_userName;
    public string Username
    {
        get => m_userName;
        set
        {
            if (m_userName == value) return;

            m_userName = value;
            RaisePropertyChanged(nameof(Username));
        }
    }

    private string m_password;

    public string Password
    {
        get { return m_password; }
        set
        {
            if (m_password == value) return;
            m_password = value;
            RaisePropertyChanged(nameof(Password));
        }
    }

    private Command m_loginCmd = new Command();

    public Command LoginCmd
    {
        get { return m_loginCmd; }
        set
        {
            if (m_loginCmd == value) return;
            m_loginCmd = value;
            RaisePropertyChanged(nameof(LoginCmd));
        }
    }

    private List<Vector2> m_list = new List<Vector2>() {
        new Vector2(20, 20),
        new Vector2(2.2f, 42f),
        new Vector2(22.1f, 34.3f),
        new Vector2(76.1f, 99.9f),
        new Vector2(-20f, -21.1f),
    };

    public List<Vector2> List
    {
        get { return m_list; }
        set
        {
            if (m_list == value) return;
            m_list = value;
            RaisePropertyChanged(nameof(List));
        }
    }


    public LoginVM()
    {
        LoginCmd.OnExecute += LoginCmd_OnExecute;
    }

    private void LoginCmd_OnExecute()
    {
        Debug.Log($"u:{Username},p:{Password}");
    }
}

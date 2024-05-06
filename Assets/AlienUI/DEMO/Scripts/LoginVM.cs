using AlienUI;
using AlienUI.Core.Commnands;
using AlienUI.Models;
using UnityEngine;

public class LoginVM : ViewModel
{
    public string Username
    {
        get { return (string)GetValue(UsernameProperty); }
        set { SetValue(UsernameProperty, value); }
    }
    public static readonly DependencyProperty UsernameProperty =
        DependencyProperty.Register("Username", typeof(string), typeof(LoginVM), string.Empty);

    public string Password
    {
        get { return (string)GetValue(PasswordProperty); }
        set { SetValue(PasswordProperty, value); }
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register("Password", typeof(string), typeof(LoginVM), string.Empty);



    public Command LoginCmd
    {
        get { return (Command)GetValue(LoginCmdProperty); }
        set { SetValue(LoginCmdProperty, value); }
    }
    public static readonly DependencyProperty LoginCmdProperty =
        DependencyProperty.Register("LoginCmd", typeof(Command), typeof(LoginVM), new Command());

    public LoginVM() 
    {
        LoginCmd.OnExecute += LoginCmd_OnExecute;
    }

    private void LoginCmd_OnExecute()
    {
        Debug.Log($"u:{Username},p:{Password}");
    }
}

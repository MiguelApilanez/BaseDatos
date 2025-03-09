using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UserManager userManager;

    [Header("Login Panel")]
    public TMP_InputField usernameInputLogin;
    public TMP_InputField passwordInputLogin;
    public GameObject loginPanel;
    public GameObject mainMenuPanel;

    [Header("Register Panel")]
    public TMP_InputField usernameInputRegister;
    public TMP_InputField passwordInputRegister;
    public GameObject registerPanel;

    //public TextMeshPro errorText;

    private void Start()
    {
        ShowLoginPanel();
    }

    public void TryLogin()
    {
        string username = usernameInputLogin.text;
        string password = passwordInputLogin.text;

        if (userManager.LoginUser(username, password))
        {
            ShowMainMenu(username);
        }
        /*
        else
        {
            errorText.text = "Usuario o contraseña incorrectos.";
        }
        */
    }

    public void TryRegister()
    {
        string username = usernameInputRegister.text;
        string password = passwordInputRegister.text;

        if (userManager.RegisterUser(username, password))
        {
            ShowLoginPanel();
        }
        /*
        else
        {
            errorText.text = "El usuario ya existe.";
        }
        */
    }
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
    }
    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void ShowMainMenu(string username)
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        Debug.Log("Bienvenido " + username);
    }
    public void ShowLogros(string username)
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        //logrosPanel.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UserManager userManager;
    public TMP_Text usernameText;

    [Header("Login Panel")]
    public TMP_InputField emailInputLogin;
    public TMP_InputField passwordInputLogin;
    public GameObject loginPanel;
    public GameObject mainMenuPanel;

    [Header("Register Panel")]
    public TMP_InputField emailInputRegister;
    public TMP_InputField passwordInputRegister;
    public GameObject registerPanel;

    //public TextMeshPro errorText;

    private void Start()
    {
        ShowLoginPanel();
    }

    public void TryLogin()
    {
        string input = emailInputLogin.text; // Puede ser correo o nombre de usuario
        string password = passwordInputLogin.text;

        if (userManager.LoginUser(input, password))
        {
            // Obtener el usuario autenticado
            User user = userManager.GetUserByEmailOrUsername(input);

            if (user != null)
            {
                ShowMainMenu(user.Email); // Pasamos el correo correctamente
            }
            else
            {
                Debug.LogError("El usuario no se encontró después de iniciar sesión.");
            }
        }
        /*
        else
        {
            errorText.text = "Correo/Usuario o contraseña incorrectos.";
        }
        */
    }

    public void TryRegister()
    {
        string email = emailInputRegister.text;
        string password = passwordInputRegister.text;

        if (userManager.RegisterUser(email, password))
        {
            ShowLoginPanel();
        }
        /*
        else
        {
            errorText.text = "Error al registrar usuario.";
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

    public void ShowMainMenu(string email)
    {
        if (string.IsNullOrEmpty(userManager.currentUserEmail))
        {
            Debug.LogError("El correo del usuario no está asignado correctamente.");
            return;
        }

        usernameText.text = "Usuario: " + userManager.currentUserEmail; // Muestra el correo en el menú principal
        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
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

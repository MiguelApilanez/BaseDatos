using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;


[Serializable]
public class User
{
    public string Username;
    public string PasswordHash;
}

[Serializable]
public class UserDatabase
{
    public List<User> Users = new List<User>();
}

public class UserManager : MonoBehaviour
{
    [Header("Login Panel")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goToRegisterButton;

    [Header("Register Panel")]
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private TMP_InputField newUsernameInput;
    [SerializeField] private TMP_InputField newPasswordInput;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button backToLoginButton;

    [Header("Main Menu Panel")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private TextMeshProUGUI usernameDisplay;

    private string filePath;
    private UserDatabase userDatabase = new UserDatabase();
    private string loggedInUser = "";

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "users.yaml");
        LoadUsers();

        loginButton.onClick.AddListener(() => Login(usernameInput.text, passwordInput.text));
        goToRegisterButton.onClick.AddListener(() => ShowPanel(registerPanel));

        signUpButton.onClick.AddListener(() => RegisterUser(newUsernameInput.text, newPasswordInput.text));
        backToLoginButton.onClick.AddListener(() => ShowPanel(loginPanel));

        ShowPanel(loginPanel);
    }
    private void ShowPanel(GameObject panel)
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(false);
        mainMenuPanel.SetActive(false);

        panel.SetActive(true);
    }

    //Guardar usuarios en YAML
    private void SaveUsers()
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        string yaml = serializer.Serialize(userDatabase);
        File.WriteAllText(filePath, yaml);
    }

    //Cargar usuarios desde YAML
    private void LoadUsers()
    {
        if (!File.Exists(filePath)) return;

        string yaml = File.ReadAllText(filePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        userDatabase = deserializer.Deserialize<UserDatabase>(yaml);
    }

    //Registrar un nuevo usuario
    private void RegisterUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Debug.LogError("Usuario o contraseña no pueden estar vacíos.");
            return;
        }

        if (userDatabase.Users.Exists(u => u.Username == username))
        {
            Debug.LogError("El usuario ya existe.");
            return;
        }

        string hashedPassword = HashPassword(password);
        userDatabase.Users.Add(new User { Username = username, PasswordHash = hashedPassword });

        SaveUsers();
        Debug.Log("Cuenta creada con éxito.");

        // Volver al panel de Login
        ShowPanel(loginPanel);
    }

    //Iniciar sesión
    private void Login(string username, string password)
    {
        User user = userDatabase.Users.Find(u => u.Username == username);

        if (user == null || user.PasswordHash != HashPassword(password))
        {
            Debug.LogError("Usuario o contraseña incorrectos.");
            return;
        }

        loggedInUser = username;
        usernameDisplay.text = $"Bienvenido, {loggedInUser}";

        Debug.Log("Inicio de sesión exitoso.");
        ShowPanel(mainMenuPanel);
    }

    //Método para hashear la contraseña
    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create()
            .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
    }
}

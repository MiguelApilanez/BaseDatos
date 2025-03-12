using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class UserManager : MonoBehaviour
{
    private string filePath;
    private UserDatabase userDatabase;

    private PointsManager pointsManager;

    public string currentUserEmail;

    private void Start()
    {
        pointsManager = FindObjectOfType<PointsManager>();

        Debug.Log("Ruta del archivo YAML: " + filePath);

        DontDestroyOnLoad(this.gameObject);
    }
    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "users.yaml");

        if (!File.Exists(filePath))
        {
            userDatabase = new UserDatabase { Users = new List<User>() };
            SaveUsers();
        }
        else
        {
            LoadUsers();
            if (userDatabase.Users == null)
            {
                userDatabase.Users = new List<User>();
            }
        }
    }
    public User GetUserByEmailOrUsername(string input)
    {
        return userDatabase.Users.Find(u => u.Email == input || u.Username == input);
    }
    public bool RegisterUser(string email, string password)
    {
        if (userDatabase == null || userDatabase.Users == null)
        {
            userDatabase = new UserDatabase { Users = new List<User>() };
        }

        // Validar formato del email
        if (!IsValidEmail(email))
        {
            Debug.LogError("Correo inválido.");
            return false;
        }

        // Extraer el nombre de usuario antes del @
        string username = email.Split('@')[0];

        if (userDatabase.Users.Exists(u => u.Email == email))
        {
            Debug.LogError("Este correo ya está registrado.");
            return false;
        }

        string hashedPassword = HashPassword(password);
        userDatabase.Users.Add(new User { Email = email, Username = username, PasswordHash = hashedPassword });
        SaveUsers();

        Debug.Log($"Usuario registrado correctamente. Nombre de usuario: {username}");
        return true;
    }

    public bool LoginUser(string input, string password)
    {
        User user = userDatabase.Users.Find(u => u.Email == input || u.Username == input);

        if (user == null || user.PasswordHash != HashPassword(password))
        {
            Debug.LogError("Correo/Usuario o contraseña incorrectos.");
            return false;
        }

        //Asigna el correo del usuario
        currentUserEmail = user.Email;
        Debug.Log("Inicio de sesión exitoso: " + currentUserEmail);

        return true;
    }

    private void SaveUsers()
    {
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        string yaml = serializer.Serialize(userDatabase);
        File.WriteAllText(filePath, yaml);
    }
    private void LoadUsers()
    {
        string yaml = File.ReadAllText(filePath);
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        userDatabase = deserializer.Deserialize<UserDatabase>(yaml);
    }
    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }
    private bool IsValidEmail(string email)
    {
        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern);
    }
}

    [System.Serializable]
    public class User
    {
    public string Email;
    public string Username;
    public string PasswordHash;
    }

    [System.Serializable]
    public class UserDatabase
    {
    public List<User> Users = new List<User>();
    }
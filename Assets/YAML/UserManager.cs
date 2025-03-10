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

public class UserManager : MonoBehaviour
{
    private string filePath;
    private string pointsFilePath;
    private UserDatabase userDatabase;
    public PlayerPointsDatabase playerPointsDatabase;

    private string currentUserEmail;

    private void Start()
    {
        Debug.Log("Ruta del archivo YAML: " + filePath);
    }
    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "users.yaml");
        pointsFilePath = Path.Combine(Application.persistentDataPath, "playerPoints.json");

        if (!File.Exists(filePath))
        {
            userDatabase = new UserDatabase { Users = new List<User>() };
            SaveUsers();
        }
        else
        {
            LoadUsers();
        }

        if (!File.Exists(pointsFilePath))
        {
            playerPointsDatabase = new PlayerPointsDatabase { playerPoints = new List<puntosJSON>() };
            SavePlayerPoints();
        }
        else
        {
            LoadPlayerPoints();
        }
    }

    public bool RegisterUser(string username, string password)
    {
        if (userDatabase.Users.Exists(u => u.Username == username))
        {
            Debug.LogError("El usuario ya existe.");
            return false;
        }

        string hashedPassword = HashPassword(password);
        userDatabase.Users.Add(new User { Username = username, PasswordHash = hashedPassword });
        SaveUsers();

        Debug.Log("Usuario registrado correctamente.");
        return true;
    }

    public bool LoginUser(string username, string password)
    {
        User user = userDatabase.Users.Find(u => u.Username == username);
        if (user == null || user.PasswordHash != HashPassword(password))
        {
            Debug.LogError("Usuario o contraseña incorrectos.");
            return false;
        }

        Debug.Log("Inicio de sesión exitoso: " + username);
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

    public void SetCurrentUserEmail(string email)
    {
        currentUserEmail = email;
    }

    public string GetCurrentUserEmail()
    {
        return currentUserEmail;
    }

    public void SavePlayerPoints()
    {
        string json = JsonUtility.ToJson(playerPointsDatabase, true);
        File.WriteAllText(pointsFilePath, json);
    }

    public void LoadPlayerPoints()
    {
        string json = File.ReadAllText(pointsFilePath);
        playerPointsDatabase = JsonUtility.FromJson<PlayerPointsDatabase>(json);
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
}

    [System.Serializable]
    public class User
    {
    public string Username;
    public string PasswordHash;
    }

    [System.Serializable]
    public class UserDatabase
    {
    public List<User> Users = new List<User>();
    }
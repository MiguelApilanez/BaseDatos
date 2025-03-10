using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance { get; private set; }
    public PlayerPointsDatabase playerPointsDatabase;
    private string pointsFilePath;

    private string currentUserEmail;

    private UserManager userManager;

    private void Awake()
    {
        userManager = FindObjectOfType<UserManager>();

        if (Instance != null)
        {
            Destroy(gameObject);  // Si ya existe una instancia, destrúyela.
            return;
        }

        Instance = this;  // Asignamos la instancia
        pointsFilePath = Path.Combine(Application.streamingAssetsPath, "playerPoints.json");

        // Cargamos los puntos si el archivo existe
        if (File.Exists(pointsFilePath))
        {
            LoadPlayerPoints();
        }
        else
        {
            playerPointsDatabase = new PlayerPointsDatabase();  // Si no existe, creamos una nueva base de datos.
            SavePlayerPoints();
        }
    }

    public void SetCurrentUserEmail(string email)
    {
        currentUserEmail = email;
    }

    public string GetCurrentUserEmail()
    {
        return currentUserEmail;
    }



    // Método para cargar los puntos del archivo JSON
    public void LoadPlayerPoints()
    {
        string json = File.ReadAllText(pointsFilePath);
        playerPointsDatabase = JsonUtility.FromJson<PlayerPointsDatabase>(json);
    }

    // Método para guardar los puntos en el archivo JSON
    public void SavePlayerPoints()
    {
        string json = JsonUtility.ToJson(playerPointsDatabase, true);
        File.WriteAllText(pointsFilePath, json);
    }

    // Método para obtener puntos por correo
    public puntosJSON GetPlayerPoints(string email)
    {
        return playerPointsDatabase.playerPoints.Find(p => p.email == email);
    }

    // Método para agregar o actualizar puntos de un jugador
    public void UpdatePoints(string email, int newMaxPoints)
    {
        puntosJSON playerPoints = GetPlayerPoints(email);
        if (playerPoints != null)
        {
            playerPoints.maxPoints = newMaxPoints;
        }
        else
        {
            playerPointsDatabase.playerPoints.Add(new puntosJSON(email, newMaxPoints));
        }

        SavePlayerPoints();
    }
}

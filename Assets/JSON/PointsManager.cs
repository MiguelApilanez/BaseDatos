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

    public int puntosMax;

    private void Awake()
    {
        userManager = FindObjectOfType<UserManager>();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pointsFilePath = Path.Combine(Application.streamingAssetsPath, "playerPoints.json");

        if (File.Exists(pointsFilePath))
        {
            LoadPlayerPoints();
        }
        else
        {
            playerPointsDatabase = new PlayerPointsDatabase();
            SavePlayerPoints();
        }

        currentUserEmail = userManager.currentUserEmail;

        if (!string.IsNullOrEmpty(currentUserEmail))
        {
            SetPlayerMaxPoints(currentUserEmail);
        }
    }

    private void SetPlayerMaxPoints(string email)
    {
        puntosJSON playerPoints = GetPlayerPoints(email);

        if (playerPoints != null)
        {
            puntosMax = playerPoints.maxPoints;
            Debug.Log("Puntos máximos del jugador " + email + ": " + puntosMax);
        }
        else
        {
            Debug.LogWarning("El jugador " + email + " no tiene puntos registrados.");
            puntosMax = 0;
        }
    }

    public void LoadPlayerPoints()
    {
        string json = File.ReadAllText(pointsFilePath);
        playerPointsDatabase = JsonUtility.FromJson<PlayerPointsDatabase>(json);
    }

    public void SavePlayerPoints()
    {
        string json = JsonUtility.ToJson(playerPointsDatabase, true);
        File.WriteAllText(pointsFilePath, json);
    }

    public puntosJSON GetPlayerPoints(string email)
    {
        return playerPointsDatabase.playerPoints.Find(p => p.email == email);
    }

    public void UpdatePoints(int newMaxPoints)
    {
        if (Instance == null)
        {
            Debug.LogError("PointsManager no está inicializado correctamente.");
            return;
        }

        string email = userManager.currentUserEmail;
        if (string.IsNullOrEmpty(email))
        {
            Debug.LogError("El correo del usuario no está asignado correctamente.");
            return;
        }

        puntosJSON playerPoints = GetPlayerPoints(email);

        if (playerPoints != null)
        {
            if (newMaxPoints > playerPoints.maxPoints)
            {
                playerPoints.maxPoints = newMaxPoints;
                SavePlayerPoints();
            }
        }
        else
        {
            Debug.Log("Añadiendo nuevo jugador: " + email + " con puntuación: " + newMaxPoints);
            playerPointsDatabase.playerPoints.Add(new puntosJSON(email, newMaxPoints));
            SavePlayerPoints();
        }
    }
}

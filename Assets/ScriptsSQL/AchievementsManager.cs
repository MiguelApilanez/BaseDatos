using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance;

    public List<string> logros = new List<string>();
    public List<int> puntosRequeridos = new List<int>();
    public List<bool> logrosCompletados = new List<bool>();
    public List<string> logrosDescripcion = new List<string>();

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    [Header("UI Elements")]
    public TextMeshProUGUI achievementsText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadAchievements(string userEmail)
    {
        logros.Clear();
        puntosRequeridos.Clear();
        logrosCompletados.Clear();

        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = @"
            SELECT l.descripcion, l.puntos_requeridos, IFNULL(lc.completado, FALSE) AS completado
            FROM logros l
            LEFT JOIN logros_completados lc
            ON l.id = lc.logro_id AND lc.usuario_email = @Email";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", userEmail);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                logros.Add(reader.GetString("descripcion"));
                puntosRequeridos.Add(reader.GetInt32("puntos_requeridos"));
                logrosCompletados.Add(reader.GetBoolean("completado"));
            }

            if (logros.Count == 0)
            {
                Debug.LogError("No se han cargado logros para el usuario.");
            }

            if (logros.Count != logrosCompletados.Count)
            {
                Debug.LogError("Las listas logros y logrosCompletados no tienen el mismo tamaño.");
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al cargar los logros: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    public void DisplayAchievements()
    {
        if (logros == null || logrosCompletados == null || logros.Count == 0)
        {
            Debug.LogError("No se han cargado logros o las listas están vacías.");
            return;
        }

        if (logros.Count != logrosCompletados.Count)
        {
            Debug.LogError("Las listas logros y logrosCompletados no tienen el mismo tamaño.");
            return;
        }

        string logrosDisplay = "";

        for (int i = 0; i < logros.Count; i++)
        {
            if (i < logrosCompletados.Count)
            {
                string estadoLogro = logrosCompletados[i] ? "Completado" : "No Completado";
                logrosDisplay += logros[i] + " - " + estadoLogro + "\n";
            }
        }

        if (achievementsText != null)
        {
            achievementsText.text = logrosDisplay;
        }
        else
        {
            Debug.LogError("achievementsText no está asignado.");
        }
    }
    public void CheckAchievements(int puntos, string userEmail)
    {
        for (int i = 0; i < logros.Count; i++)
        {
            if (puntos >= puntosRequeridos[i] && !logrosCompletados[i])
            {
                logrosCompletados[i] = true;
                UpdateAchievementStatus(i, userEmail);
            }
        }
    }
    private void UpdateAchievementStatus(int index, string userEmail)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = @"
                INSERT INTO logros_completados (usuario_email, logro_id, completado)
                VALUES (@Email, @LogroId, @Completado)
                ON DUPLICATE KEY UPDATE completado = @Completado";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", userEmail);
            cmd.Parameters.AddWithValue("@LogroId", index + 1);
            cmd.Parameters.AddWithValue("@Completado", logrosCompletados[index]);

            cmd.ExecuteNonQuery();
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al actualizar el estado del logro: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }
}

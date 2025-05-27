using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsManager : MonoBehaviour
{
    public static AchievementsManager Instance;

    public List<string> logros = new List<string>();
    public List<int> puntosRequeridos = new List<int>();
    public List<bool> logrosCompletados = new List<bool>();

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

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
    public void LoadAchievements()
    {
        logros.Clear();
        puntosRequeridos.Clear();
        logrosCompletados.Clear();

        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "SELECT descripcion, puntos_requeridos, completado FROM logros";
            MySqlCommand cmd = new MySqlCommand(query, connection);

            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                logros.Add(reader.GetString("descripcion"));
                puntosRequeridos.Add(reader.GetInt32("puntos_requeridos"));
                logrosCompletados.Add(reader.GetBoolean("completado"));
            }
            //Debug.Log("Logros cargados: " + logros.Count);
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

    public void CheckAchievements(int puntos)
    {
        for (int i = 0; i < logros.Count; i++)
        {
            if (puntos >= puntosRequeridos[i] && !logrosCompletados[i])
            {
                logrosCompletados[i] = true;
                UpdateAchievementStatus(i);
            }
        }
    }

    private void UpdateAchievementStatus(int index)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "UPDATE logros SET completado = @Completado WHERE descripcion = @Descripcion";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Descripcion", logros[index]);
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

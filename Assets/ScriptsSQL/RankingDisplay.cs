using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;

public class RankingDisplay : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject rankingEntryPrefab;
    public Transform rankingContainer;
    public int topN = 5;

    public void ShowTopPlayers()
    {
        string connString = "Server=localhost; Database=basedatos; User ID=root;Password=;";

        using (MySqlConnection conn = new MySqlConnection(connString))
        {
            try
            {
                conn.Open();
                // Cambié SELECT para obtener username en vez de email
                string query = $"SELECT username, max_points FROM puntos_jugadores ORDER BY max_points DESC LIMIT {topN};";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int rank = 1;

                while (reader.Read())
                {
                    string username = reader.GetString("username");
                    int puntos = reader.GetInt32("max_points");

                    GameObject entry = Instantiate(rankingEntryPrefab, rankingContainer);
                    TextMeshProUGUI textComponent = entry.GetComponent<TextMeshProUGUI>();
                    // Mostrar username en lugar de email
                    textComponent.text = $"{rank}. {username} - {puntos} puntos";

                    rank++;
                }

                reader.Close();
            }
            catch (MySqlException ex)
            {
                Debug.LogError("Error al conectar a la base de datos: " + ex.Message);
            }
        }
    }

}

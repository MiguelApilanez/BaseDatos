using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System;

public class UsuarioBuscador : MonoBehaviour
{
    public TMP_InputField inputUsername;
    public TextMeshProUGUI resultText;

    private string connectionString = "Server=localhost;Database=basedatos;User ID=root;Password=;";

    public void BuscarUsuario()
    {
        string usernameInput = inputUsername.text;

        if (string.IsNullOrEmpty(usernameInput))
        {
            return;
        }

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT u.username, mp.max_points, u.last_login FROM usuarios u JOIN puntos_jugadores mp ON u.email = mp.email WHERE u.username LIKE @username";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", "%" + usernameInput + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string username = reader.GetString("username");
                            int puntos = reader.GetInt32("max_points");
                            DateTime lastLogin = reader.GetDateTime("last_login");

                            resultText.text = $"User: {username}\nMax Points: {puntos}\nLast Login: {lastLogin}";
                        }
                        else
                        {
                            resultText.text = "User not found.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error de conexión o consulta: " + ex.Message);
                resultText.text = "Error finding user.";
            }
        }
    }
}

using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointsManager
{
    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    // Método para obtener username desde email en tabla usuarios
    private string GetUsernameFromEmail(MySqlConnection connection, string email)
    {
        string query = "SELECT username FROM usuarios WHERE email = @Email";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Email", email);
        object result = cmd.ExecuteScalar();
        return result != null ? result.ToString() : null;
    }

    // Nuevo método que corrige usernames vacíos en puntos_jugadores al iniciar el script
    public void CheckAndFixUsernames()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                // 1. Obtener todos los emails con username vacío o NULL
                string selectQuery = "SELECT email FROM puntos_jugadores WHERE username IS NULL OR username = ''";
                MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);

                var emailsToFix = new List<string>();
                using (MySqlDataReader reader = selectCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        emailsToFix.Add(reader["email"].ToString());
                    }
                } // reader cerrado aquí

                // 2. Para cada email, obtener username y actualizar la tabla
                foreach (string email in emailsToFix)
                {
                    string username = GetUsernameFromEmail(connection, email);
                    if (!string.IsNullOrEmpty(username))
                    {
                        string updateQuery = "UPDATE puntos_jugadores SET username = @Username WHERE email = @Email";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                        updateCmd.Parameters.AddWithValue("@Username", username);
                        updateCmd.Parameters.AddWithValue("@Email", email);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Debug.LogWarning($"No se encontró username para el email {email}");
                    }
                }
            }
            catch (MySqlException e)
            {
                Debug.LogError("Error al corregir usernames: " + e.Message);
            }
        }
    }

    private string GetEmailFromUsername(MySqlConnection connection, string username)
    {
        string query = "SELECT email FROM usuarios WHERE username = @Username";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@Username", username);
        object result = cmd.ExecuteScalar();
        return result != null ? result.ToString() : null;
    }

    public bool SavePlayerPoints(string identificador, int maxPoints)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string email = identificador;
                if (!identificador.Contains("@"))
                {
                    // Si no contiene @, asumimos que es username
                    email = GetEmailFromUsername(connection, identificador);
                    if (string.IsNullOrEmpty(email))
                    {
                        Debug.LogError("No se encontró un email para el username: " + identificador);
                        return false;
                    }
                }

                string username = GetUsernameFromEmail(connection, email);
                if (string.IsNullOrEmpty(username))
                {
                    Debug.LogError("No se encontró un username para el email: " + email);
                    return false;
                }

                string checkQuery = "SELECT * FROM puntos_jugadores WHERE email = @Email";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Email", email);

                MySqlDataReader checkReader = checkCmd.ExecuteReader();

                if (checkReader.HasRows)
                {
                    checkReader.Close();
                    return true;
                }
                else
                {
                    checkReader.Close();

                    string insertQuery = "INSERT INTO puntos_jugadores (email, username, max_points) VALUES (@Email, @Username, @MaxPoints)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Username", username);
                    insertCmd.Parameters.AddWithValue("@MaxPoints", maxPoints);

                    int result = insertCmd.ExecuteNonQuery();
                    return result > 0;
                }
            }
            catch (MySqlException e)
            {
                Debug.LogError("Error al guardar los puntos: " + e.Message);
                return false;
            }
        }
    }

    public bool UpdatePlayerPoints(string identificador, int newMaxPoints)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string email = identificador;
                if (!identificador.Contains("@"))
                {
                    // Es username
                    email = GetEmailFromUsername(connection, identificador);
                    if (string.IsNullOrEmpty(email))
                    {
                        Debug.LogError("No se encontró un email para el username: " + identificador);
                        return false;
                    }
                }

                string checkQuery = "SELECT * FROM puntos_jugadores WHERE email = @Email";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                checkCmd.Parameters.AddWithValue("@Email", email);

                MySqlDataReader checkReader = checkCmd.ExecuteReader();

                if (checkReader.HasRows)
                {
                    checkReader.Close();

                    string updateQuery = "UPDATE puntos_jugadores SET max_points = @MaxPoints WHERE email = @Email";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                    updateCmd.Parameters.AddWithValue("@MaxPoints", newMaxPoints);
                    updateCmd.Parameters.AddWithValue("@Email", email);

                    int result = updateCmd.ExecuteNonQuery();
                    return result > 0;
                }
                else
                {
                    checkReader.Close();
                    return SavePlayerPoints(email, newMaxPoints);
                }
            }
            catch (MySqlException e)
            {
                Debug.LogError("Error al actualizar los puntos: " + e.Message);
                return false;
            }
        }
    }

}

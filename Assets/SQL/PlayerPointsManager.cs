using MySql.Data.MySqlClient;
using UnityEngine;

public class PlayerPointsManager
{
    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    public bool SavePlayerPoints(string email, int maxPoints)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

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
                string insertQuery = "INSERT INTO puntos_jugadores (email, max_points) VALUES (@Email, @MaxPoints)";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@Email", email);
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
        finally
        {
            connection.Close();
        }
    }

    public bool UpdatePlayerPoints(string email, int newMaxPoints)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string checkQuery = "SELECT * FROM puntos_jugadores WHERE email = @Email";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader checkReader = checkCmd.ExecuteReader();

            if (checkReader.HasRows)
            {
                checkReader.Close();
                string updateQuery = "UPDATE puntos_jugadores SET max_points = @MaxPoints WHERE email = @Email";
                MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@Email", email);
                updateCmd.Parameters.AddWithValue("@MaxPoints", newMaxPoints);

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
        finally
        {
            connection.Close();
        }
    }
}

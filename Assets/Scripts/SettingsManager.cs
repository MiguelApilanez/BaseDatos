using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    private void Start()
    {
        float savedVolume = GetUserVolume(LoginArreglado.currentUserEmail);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        SaveUserVolume(LoginArreglado.currentUserEmail, value);
    }

    private void SaveUserVolume(string email, float volume)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "INSERT INTO usuario_settings (email, volume) VALUES (@Email, @Volume) " +
                           "ON DUPLICATE KEY UPDATE volume = @Volume";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Volume", volume);

            cmd.ExecuteNonQuery();
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al guardar el volumen: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private float GetUserVolume(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        float volume = 0.5f;

        try
        {
            connection.Open();

            string query = "SELECT volume FROM usuario_settings WHERE email = @Email";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (!reader.IsDBNull(reader.GetOrdinal("volume")))
                {
                    volume = reader.GetFloat("volume");
                }
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al obtener el volumen: " + e.Message);
        }
        finally
        {
            connection.Close();
        }

        return volume;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Button spanishButton;
    public Button englishButton;

    [Header("Config")]
    public TMP_Text configText;
    public TMP_Text configVolumeText;
    public TMP_Text backConfigText;

    [Header("Logros")]
    public TMP_Text logrosText;
    public TMP_Text logrosTitleText;
    public TMP_Text backLogrosText;

    [Header("Buscar")]
    public TMP_Text buscarTitleText;
    public TMP_Text buscarText;
    public TMP_Text seguirText;
    public TMP_Text buscarBotonText;
    public TMP_Text backBuscarText;

    [Header("Menu")]
    public TMP_Text menuTitleText;
    public TMP_Text startMenuText;
    public Button backButton;
    public TMP_Text backMenuText;
    public TMP_Text buscarMenuText;
    public TMP_Text configMenuText;
    public TMP_Text logrosMenuText;


    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    private void Start()
    {
        float savedVolume = GetUserVolume(LoginArreglado.currentUserEmail);
        AudioListener.volume = savedVolume;
        volumeSlider.value = savedVolume;

        string language = GetUserLanguage(LoginArreglado.currentUserEmail);
        SetLanguage(language);

        spanishButton.onClick.AddListener(() => ChangeLanguage("es"));
        englishButton.onClick.AddListener(() => ChangeLanguage("en"));
        backButton.onClick.AddListener(ResetSettingsOnBackButton);

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }
    private void ResetSettingsOnBackButton()
    {
        volumeSlider.value = 0.5f;
        AudioListener.volume = 0.5f;

        ChangeLanguage("es");

        SaveUserLanguage(LoginArreglado.currentUserEmail, "es");
        SaveUserVolume(LoginArreglado.currentUserEmail, 0.5f);
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
    private string GetUserLanguage(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        string language = "es";

        try
        {
            connection.Open();

            string query = "SELECT language FROM usuarios WHERE email = @Email";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                language = reader.GetString("language");
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al obtener el idioma: " + e.Message);
        }
        finally
        {
            connection.Close();
        }

        return language;
    }
    private void SaveUserLanguage(string email, string language)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "UPDATE usuarios SET language = @Language WHERE email = @Email";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Language", language);
            cmd.Parameters.AddWithValue("@Email", email);

            cmd.ExecuteNonQuery();
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al guardar el idioma: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void ChangeLanguage(string language)
    {
        SaveUserLanguage(LoginArreglado.currentUserEmail, language);
        SetLanguage(language);
    }

    private void SetLanguage(string language)
    {
        if (language == "es")
        {
            configText.text = "Configuración";
            configVolumeText.text = "Volumen";
            backConfigText.text = "Volver";
            logrosText.text = "";
            logrosTitleText.text = "Logros";
            backLogrosText.text = "Volver";
            buscarTitleText.text = "Buscar";
            buscarText.text = "";
            seguirText.text = "Seguir";
            buscarBotonText.text = "Buscar";
            backBuscarText.text = "Volver";
            menuTitleText.text = "Menú";
            startMenuText.text = "Jugar";
            backMenuText.text = "Volver";
            buscarMenuText.text = "Buscar";
            configMenuText.text = "Configuración";
            logrosMenuText.text = "Logros";
        }
        else if (language == "en")
        {
            configText.text = "Configuration";
            configVolumeText.text = "Volume";
            backConfigText.text = "Back";
            logrosText.text = "";
            logrosTitleText.text = "Achievements";
            backLogrosText.text = "Back";
            buscarTitleText.text = "Search";
            buscarText.text = "";
            seguirText.text = "Follow";
            buscarBotonText.text = "Search";
            backBuscarText.text = "Back";
            menuTitleText.text = "Menu";
            startMenuText.text = "Play";
            backMenuText.text = "Back";
            buscarMenuText.text = "Search";
            configMenuText.text = "Configuration";
            logrosMenuText.text = "Achievements";

        }
    }
}
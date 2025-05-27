using System.Collections;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System.Threading.Tasks; // Asegúrate de tener esta importación
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class LoginArreglado : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject signUpPanel;
    public GameObject mainMenuPanel;

    [Header("Login Fields")]
    public TMP_InputField loginUserField;
    public TMP_InputField loginPasswordField;

    [Header("SignUp Fields")]
    public TMP_InputField signUpUserField;
    public TMP_InputField signUpPasswordField;

    [Header("MainMenu UI")]
    public TMP_Text usernameText;
    public TMP_Text lastLoginText;
    public Button playButton;
    public Button logoutButton;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    [Header("Logros UI")]
    public TMP_Text achievementsText;
    public GameObject achievementsPanel;
    public Button openAchievementsButton;

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    // Variable estática para almacenar el correo del jugador
    public static string currentUserEmail;
    public static string currentUsername;

    public void TogglePanels(bool showSignUp)
    {
        signUpPanel.SetActive(showSignUp);
        loginPanel.SetActive(!showSignUp);
        feedbackText.text = "";
    }
    void Start()
    {
        AchievementsManager.Instance.LoadAchievements();

        openAchievementsButton.onClick.AddListener(OpenAchievementsPanel);
    }
    public void OpenAchievementsPanel()
    {
        mainMenuPanel.SetActive(false);
        achievementsPanel.SetActive(true);

        AchievementsManager.Instance.LoadAchievements();

        string logrosDisplay = "";

        Debug.Log("Total logros: " + AchievementsManager.Instance.logros.Count);

        for (int i = 0; i < AchievementsManager.Instance.logros.Count; i++)
        {
            string estadoLogro = AchievementsManager.Instance.logrosCompletados[i] ? "Completado" : "No Completado";
            logrosDisplay += AchievementsManager.Instance.logros[i] + " - " + estadoLogro + "\n";
        }

        Debug.Log("Logros a mostrar: " + logrosDisplay);

        achievementsText.text = logrosDisplay;
    }
    public void Login()
    {
        StartCoroutine(LoginRequest());
    }

    public void Register()
    {
        string pass = signUpPasswordField.text;

        if (pass.Length < 6 || !pass.Any(char.IsDigit))
        {
            feedbackText.text = "La contraseña debe tener al menos 6 caracteres y un número.";
            return;
        }

        StartCoroutine(RegisterRequest());
    }

    IEnumerator LoginRequest()
    {
        string input = loginUserField.text;
        string password = loginPasswordField.text;

        string email = "";
        string username = "";

        if (input.Contains('@'))
        {
            email = input;
        }
        else
        {
            username = input;
        }

        var task = Task.Run(() => CheckLogin(email, username, password));

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result)
        {
            if (!string.IsNullOrEmpty(email))
            {
                currentUserEmail = email;
                currentUsername = email.Split('@')[0];
            }
            else if (!string.IsNullOrEmpty(username))
            {
                currentUsername = username;
                currentUserEmail = GetEmailByUsername(username);
            }

            feedbackText.text = "Bienvenido, " + currentUsername;

            CheckAndAssignPlayerPoints(currentUserEmail);

            ShowMainMenu(currentUsername);
        }
        else
        {
            feedbackText.text = "Usuario o contraseña incorrectos.";
        }
    }

    private bool CheckLogin(string email, string username, string password)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "";
            if (!string.IsNullOrEmpty(email))
            {
                query = "SELECT * FROM usuarios WHERE email = @Email AND password = @Password";
            }
            else if (!string.IsNullOrEmpty(username))
            {
                query = "SELECT * FROM usuarios WHERE username = @Username AND password = @Password";
            }

            MySqlCommand cmd = new MySqlCommand(query, connection);

            if (!string.IsNullOrEmpty(email))
            {
                cmd.Parameters.AddWithValue("@Email", email);
            }
            else if (!string.IsNullOrEmpty(username))
            {
                cmd.Parameters.AddWithValue("@Username", username);
            }

            cmd.Parameters.AddWithValue("@Password", password);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                reader.Close();
                string updateQuery = "UPDATE usuarios SET last_login = @LastLogin WHERE email = @Email";
                MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                updateCmd.Parameters.AddWithValue("@Email", email);

                updateCmd.ExecuteNonQuery();

                return true;
            }
            else
            {
                return false;
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error de conexión: " + e.Message);
            return false;
        }
        finally
        {
            connection.Close();
        }
    }
    private string GetEmailByUsername(string username)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "SELECT email FROM usuarios WHERE username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return reader.GetString("email");
            }
            else
            {
                return "";
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al obtener el correo por nombre de usuario: " + e.Message);
            return "";
        }
        finally
        {
            connection.Close();
        }
    }
    public void ShowMainMenu(string username)
    {
        int maxPoints = LoadPlayerMaxPoints(username);
        string lastLogin = GetLastLogin(currentUserEmail);

        usernameText.text = "Usuario: " + username;
        lastLoginText.text = "Último inicio de sesión: " + lastLogin;

        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        playButton.onClick.AddListener(() => StartGame());
        logoutButton.onClick.AddListener(() => Logout());
    }
    private void CheckAndAssignPlayerPoints(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string checkQuery = "SELECT * FROM puntos_jugadores WHERE email = @Email";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader checkReader = checkCmd.ExecuteReader();

            if (!checkReader.HasRows)
            {
                Debug.Log("Jugador sin puntos. Asignando 0 puntos...");
                PlayerPointsManager pointsManager = new PlayerPointsManager();
                pointsManager.SavePlayerPoints(email, 0);
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al comprobar los puntos del jugador: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }
    private int LoadPlayerMaxPoints(string username)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        int maxPoints = 0;
        try
        {
            connection.Open();

            string query = "SELECT max_points FROM puntos_jugadores WHERE username = @Username";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Username", username);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                maxPoints = reader.GetInt32("max_points");
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al cargar los puntos máximos: " + e.Message);
        }
        finally
        {
            connection.Close();
        }

        return maxPoints;
    }
    private string GetLastLogin(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        string lastLogin = "Nunca";
        try
        {
            connection.Open();

            string query = "SELECT last_login FROM usuarios WHERE email = @Email";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                if (!reader.IsDBNull(reader.GetOrdinal("last_login")))
                {
                    lastLogin = reader.GetDateTime("last_login").ToString("dd/MM/yyyy HH:mm:ss");
                }
                else
                {
                    lastLogin = "Nunca";
                }
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al obtener la fecha del último inicio de sesión: " + e.Message);
        }
        finally
        {
            connection.Close();
        }

        return lastLogin;
    }
    private void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private void Logout()
    {
        mainMenuPanel.SetActive(false);
        loginPanel.SetActive(true);
        loginUserField.text = "";
        loginPasswordField.text = "";
        feedbackText.text = "";
    }


    IEnumerator RegisterRequest()
    {
        string email = signUpUserField.text;
        string password = signUpPasswordField.text;

        var task = Task.Run(() => RegisterUser(email, password));

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result)
        {
            feedbackText.text = "Cuenta creada correctamente.";
            yield return new WaitForSeconds(1.5f);
            TogglePanels(false);
        }
        else
        {
            feedbackText.text = "Error al crear la cuenta o el correo ya está registrado.";
        }
    }

    private bool RegisterUser(string email, string password)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string checkQuery = "SELECT * FROM usuarios WHERE email = @Email";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader checkReader = checkCmd.ExecuteReader();

            if (checkReader.HasRows)
            {
                return false;
            }
            else
            {
                checkReader.Close();

                string insertQuery = "INSERT INTO usuarios (email, username, password) VALUES (@Email, @Name, @Password)";
                MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
                insertCmd.Parameters.AddWithValue("@Email", email);
                insertCmd.Parameters.AddWithValue("@Name", email.Split('@')[0]);
                insertCmd.Parameters.AddWithValue("@Password", password);

                int result = insertCmd.ExecuteNonQuery();

                PlayerPointsManager pointsManager = new PlayerPointsManager();
                if (result > 0)
                {
                    pointsManager.SavePlayerPoints(email, 0);
                }

                return result > 0;
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error al registrar al usuario: " + e.Message);
            return false;
        }
        finally
        {
            connection.Close();
        }
    }
    public void CloseAchievementsPanel()
    {
        achievementsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}

using System.Collections;
using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System.Threading.Tasks; // Aseg�rate de tener esta importaci�n
using UnityEngine.SceneManagement;
using System.Linq;

public class LoginArreglado : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject signUpPanel;

    [Header("Login Fields")]
    public TMP_InputField loginUserField;
    public TMP_InputField loginPasswordField;

    [Header("SignUp Fields")]
    public TMP_InputField signUpUserField;
    public TMP_InputField signUpPasswordField;

    [Header("Feedback")]
    public TMP_Text feedbackText;

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    // Variable est�tica para almacenar el correo del jugador
    public static string currentUserEmail;

    public void TogglePanels(bool showSignUp)
    {
        signUpPanel.SetActive(showSignUp);
        loginPanel.SetActive(!showSignUp);
        feedbackText.text = "";
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
            feedbackText.text = "La contrase�a debe tener al menos 6 caracteres y un n�mero.";
            return;
        }

        StartCoroutine(RegisterRequest());
    }

    IEnumerator LoginRequest()
    {
        string email = loginUserField.text;
        string password = loginPasswordField.text;

        var task = Task.Run(() => CheckLogin(email, password));  // Este es el uso de Task

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result)
        {
            // Almacenar el correo del jugador
            currentUserEmail = loginUserField.text;

            string username = task.Result ? loginUserField.text : "";
            feedbackText.text = "Bienvenido, " + username;

            // Verificar y asignar puntos si el jugador no tiene
            CheckAndAssignPlayerPoints(currentUserEmail);

            yield return new WaitForSeconds(1.5f);
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            feedbackText.text = "Usuario o contrase�a incorrectos.";
        }
    }

    private bool CheckLogin(string email, string password)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            string query = "SELECT * FROM usuarios WHERE email = @Email AND password = @Password";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error de conexi�n: " + e.Message);
            return false;
        }
        finally
        {
            connection.Close();
        }
    }

    // Verificar si el jugador tiene puntos en la base de datos, si no, asignarle 0 puntos
    private void CheckAndAssignPlayerPoints(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();

            // Comprobar si el jugador tiene puntos asignados
            string checkQuery = "SELECT * FROM puntos_jugadores WHERE email = @Email";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@Email", email);

            MySqlDataReader checkReader = checkCmd.ExecuteReader();

            if (!checkReader.HasRows)
            {
                // Si no tiene puntos asignados, asignar 0 puntos
                Debug.Log("Jugador sin puntos. Asignando 0 puntos...");
                PlayerPointsManager pointsManager = new PlayerPointsManager();
                pointsManager.SavePlayerPoints(email, 0); // Asigna 0 puntos al usuario
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
            feedbackText.text = "Error al crear la cuenta o el correo ya est� registrado.";
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

                // Guardar los puntos iniciales despu�s de registrar al usuario
                PlayerPointsManager pointsManager = new PlayerPointsManager();
                if (result > 0)
                {
                    pointsManager.SavePlayerPoints(email, 0);  // Guardamos los puntos iniciales
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using MySql.Data.MySqlClient;

public class playerController : MonoBehaviour
{
    [Header("Jugador")]
    public float xLimit;
    public float speed = 15f;
    Rigidbody2D rbPlayer;
    Animator animatorPlayer;

    [Header("Puntos")]
    public int puntosIniciales;
    public int puntos;
    public int puntosMax;
    public TextMeshProUGUI textPoints;
    public TextMeshProUGUI maxText;
    public EnemySpawner enemySpawner;

    [Header("UI")]
    public GameObject menuButton;
    public GameObject restartButton;

    private PlayerPointsManager pointsManagerDB;

    private string connectionString = "Server=localhost; database=basedatos; user=root; password=;";

    void Start()
    {
        pointsManagerDB = new PlayerPointsManager();

        if (pointsManagerDB == null)
        {
            Debug.LogError("PlayerPointsManager no ha sido inicializado correctamente.");
            return;
        }

        puntosIniciales = 0;
        puntos = puntosIniciales;

        rbPlayer = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();

        rbPlayer.bodyType = RigidbodyType2D.Kinematic;

        string username = LoginArreglado.currentUserEmail;
        puntosMax = LoadPlayerMaxPoints(username);

        textPoints.text = puntos.ToString();
        maxText.text = puntosMax.ToString();

        InvokeRepeating("Puntos", .5f, .5f);

        if (menuButton != null)
            menuButton.SetActive(false);

        if (restartButton != null)
            restartButton.SetActive(false);
    }

    private void Update()
    {
        Movement();
    }

    private int LoadPlayerMaxPoints(string email)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        int maxPoints = 0;
        try
        {
            connection.Open();

            string query = "SELECT max_points FROM puntos_jugadores WHERE email = @Email";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Email", email);

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

    void Puntos()
    {
        puntos++;

        CheckAchievements(puntos);

        if (puntos > puntosMax)
        {
            puntosMax = puntos;
            string username = LoginArreglado.currentUsername;
            UpdatePlayerPoints(puntosMax);
        }

        textPoints.text = puntos.ToString();
        maxText.text = puntosMax.ToString();
    }
    public void CheckAchievements(int puntos)
    {
        string userEmail = LoginArreglado.currentUserEmail;

        if (string.IsNullOrEmpty(userEmail))
        {
            Debug.LogError("No se ha encontrado el email del usuario.");
            return;
        }

        AchievementsManager.Instance.CheckAchievements(puntos, userEmail);
    }

    void UpdatePlayerPoints(int newMaxPoints)
    {
        string username = LoginArreglado.currentUsername;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("El nombre de usuario no es válido.");
            return;
        }

        bool success = pointsManagerDB.UpdatePlayerPoints(username, newMaxPoints);

        if (success)
        {
            Debug.Log("Puntos en la base de datos actualizados correctamente.");
        }
        else
        {
            Debug.LogError("Error al actualizar los puntos en la base de datos.");
        }
    }

    void Movement()
    {
        if (transform.position.x < -xLimit)
        {
            transform.position = new Vector3(-xLimit, transform.position.y, 0);
        }

        if (transform.position.x > xLimit)
        {
            transform.position = new Vector3(xLimit, transform.position.y, 0);
        }

        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        rbPlayer.velocity = new Vector2(speed * moveHorizontal, rbPlayer.velocity.y);

        if (moveHorizontal > 0)
        {
            animatorPlayer.SetFloat("Speed", 1f);
        }
        else if (moveHorizontal < 0)
        {
            animatorPlayer.SetFloat("Speed", -1f);
        }
        else if (moveHorizontal == 0)
        {
            animatorPlayer.SetFloat("Speed", 0f);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rbPlayer.bodyType = RigidbodyType2D.Dynamic;

            CancelInvoke();
            enemySpawner.jugando = false;

            if (menuButton != null)
                menuButton.SetActive(true);

            if (restartButton != null)
                restartButton.SetActive(true);
        }
    }
    public void BackButton()
    {
        SceneManager.LoadScene("IndexScene");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
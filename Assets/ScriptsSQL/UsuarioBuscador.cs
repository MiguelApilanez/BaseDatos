using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System;
using UnityEngine.UI;

public class UsuarioBuscador : MonoBehaviour
{
    public TMP_InputField inputUsername;
    public TextMeshProUGUI resultText;
    public Button followButton;

    private string connectionString = "Server=localhost;Database=basedatos;User ID=root;Password=;";
    private string buscadoUsername = "";

    void Start()
    {
        followButton.onClick.AddListener(ToggleFollow);
        followButton.gameObject.SetActive(false); // Ocultar al inicio
    }

    public void BuscarUsuario()
    {
        string usernameInput = inputUsername.text.Trim();

        if (string.IsNullOrEmpty(usernameInput))
        {
            resultText.text = "Introduce un nombre de usuario.";
            return;
        }

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string query = @"SELECT u.username, mp.max_points, u.last_login, COALESCE(su.numero_seguidores, 0) AS seguidores FROM usuarios u JOIN puntos_jugadores mp ON u.email = mp.email
                    LEFT JOIN seguidores_usuarios su ON su.seguido_username = u.username
                    WHERE u.username LIKE @username LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", "%" + usernameInput + "%");

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            buscadoUsername = reader.GetString("username");
                            int puntos = reader.GetInt32("max_points");
                            DateTime lastLogin = reader.GetDateTime("last_login");
                            int seguidores = reader.GetInt32("seguidores");

                            resultText.text = $"Usuario: {buscadoUsername}\nPuntos máximos: {puntos}\nÚltimo login: {lastLogin}\nSeguidores: {seguidores}";

                            CheckIfFollowing(LoginArreglado.currentUserEmail, buscadoUsername);
                        }
                        else
                        {
                            resultText.text = "Usuario no encontrado.";
                            followButton.gameObject.SetActive(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error en búsqueda: " + ex.Message);
                resultText.text = "Error al buscar usuario.";
            }
        }
    }

    private void CheckIfFollowing(string followerEmail, string targetUsername)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string checkQuery = "SELECT 1 FROM seguimientos WHERE seguidor_email = @Email AND seguido_username = @Username";

            MySqlCommand cmd = new MySqlCommand(checkQuery, conn);
            cmd.Parameters.AddWithValue("@Email", followerEmail);
            cmd.Parameters.AddWithValue("@Username", targetUsername);

            var result = cmd.ExecuteScalar();

            if (result != null)
            {
                SetFollowButtonState(true);
            }
            else
            {
                SetFollowButtonState(false);
            }

            followButton.gameObject.SetActive(true);
        }
    }

    private void SetFollowButtonState(bool isFollowing)
    {
        if (isFollowing)
        {
            followButton.GetComponentInChildren<TextMeshProUGUI>().text = "Following";
            followButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            followButton.image.color = new Color(0f, 0f, 128f);
        }
        else
        {
            followButton.GetComponentInChildren<TextMeshProUGUI>().text = "Follow";
            followButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            followButton.image.color = new Color(255f, 255f, 255f);
        }
    }

    private void ToggleFollow()
    {
        bool isCurrentlyFollowing = followButton.GetComponentInChildren<TextMeshProUGUI>().text == "Following";
        if (isCurrentlyFollowing)
        {
            DejarDeSeguir(LoginArreglado.currentUserEmail, buscadoUsername);
        }
        else
        {
            SeguirUsuario(LoginArreglado.currentUserEmail, buscadoUsername);
        }

        BuscarUsuario(); // Recargar info y botón
    }

    private void SeguirUsuario(string followerEmail, string targetUsername)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string insertFollow = "INSERT IGNORE INTO seguimientos (seguidor_email, seguido_username) VALUES (@Email, @Username)";
            MySqlCommand cmd = new MySqlCommand(insertFollow, conn);
            cmd.Parameters.AddWithValue("@Email", followerEmail);
            cmd.Parameters.AddWithValue("@Username", targetUsername);
            cmd.ExecuteNonQuery();

            // Aumentar número de seguidores
            string updateFollowers = @"
                INSERT INTO seguidores_usuarios (seguido_username, numero_seguidores) VALUES (@Username, 1)
                ON DUPLICATE KEY UPDATE numero_seguidores = numero_seguidores + 1";
            MySqlCommand updateCmd = new MySqlCommand(updateFollowers, conn);
            updateCmd.Parameters.AddWithValue("@Username", targetUsername);
            updateCmd.ExecuteNonQuery();
        }
    }

    private void DejarDeSeguir(string followerEmail, string targetUsername)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string deleteFollow = "DELETE FROM seguimientos WHERE seguidor_email = @Email AND seguido_username = @Username";
            MySqlCommand cmd = new MySqlCommand(deleteFollow, conn);
            cmd.Parameters.AddWithValue("@Email", followerEmail);
            cmd.Parameters.AddWithValue("@Username", targetUsername);
            cmd.ExecuteNonQuery();

            // Disminuir número de seguidores (sin bajar de 0)
            string updateFollowers = "UPDATE seguidores_usuarios SET numero_seguidores = GREATEST(0, numero_seguidores - 1) WHERE seguido_username = @Username";
            MySqlCommand updateCmd = new MySqlCommand(updateFollowers, conn);
            updateCmd.Parameters.AddWithValue("@Username", targetUsername);
            updateCmd.ExecuteNonQuery();
        }
    }
}

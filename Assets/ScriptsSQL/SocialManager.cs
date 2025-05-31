using UnityEngine;
using TMPro;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class SocialManager : MonoBehaviour
{
    public TextMeshProUGUI seguidoresText;
    public TextMeshProUGUI seguidosText;
    public TextMeshProUGUI comunesText;
    public TMP_InputField inputOtroUsuario;

    private string connectionString = "Server=localhost;Database=basedatos;User ID=root;Password=;";

    public void MostrarSeguidoresPropios()
    {
        string currentEmail = LoginArreglado.currentUserEmail;
        List<string> seguidores = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
                SELECT u.username FROM seguimientos s JOIN usuarios u ON s.seguidor_email = u.email WHERE s.seguido_username = (SELECT username FROM usuarios WHERE email = @Email)";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", currentEmail);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    seguidores.Add(reader.GetString("username"));
                }
            }
        }

        seguidoresText.text = "Tus seguidores:\n" + (seguidores.Count > 0 ? string.Join("\n", seguidores) : "Ninguno.");
    }

    public void MostrarSeguidosPropios()
    {
        string currentEmail = LoginArreglado.currentUserEmail;
        List<string> seguidos = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
                SELECT s.seguido_username FROM seguimientos s WHERE s.seguidor_email = @Email";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", currentEmail);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    seguidos.Add(reader.GetString("seguido_username"));
                }
            }
        }

        seguidosText.text = "Usuarios que sigues:\n" + (seguidos.Count > 0 ? string.Join("\n", seguidos) : "Ninguno.");
    }

    public void MostrarSeguidoresEnComun()
    {
        string currentEmail = LoginArreglado.currentUserEmail;
        string otroUsuario = inputOtroUsuario.text.Trim();

        if (string.IsNullOrEmpty(otroUsuario))
        {
            comunesText.text = "Introduce el nombre de otro usuario.";
            return;
        }

        List<string> comunes = new List<string>();

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();

            string query = @"
                SELECT u.username FROM usuarios u
                WHERE u.email IN (
                    SELECT s1.seguidor_email
                    FROM seguimientos s1
                    WHERE s1.seguido_username = (
                        SELECT username FROM usuarios WHERE email = @Email
                    )
                    INTERSECT
                    SELECT s2.seguidor_email
                    FROM seguimientos s2
                    WHERE s2.seguido_username = @OtroUsername
                )";

            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Email", currentEmail);
            cmd.Parameters.AddWithValue("@OtroUsername", otroUsuario);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    comunes.Add(reader.GetString("username"));
                }
            }
        }

        comunesText.text = $"Seguidores en común con {otroUsuario}:\n" + (comunes.Count > 0 ? string.Join("\n", comunes) : "Ninguno.");
    }
}

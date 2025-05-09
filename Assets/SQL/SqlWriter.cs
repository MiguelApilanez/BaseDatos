using UnityEngine;
using MySql.Data.MySqlClient;

public class SqlWriter : MonoBehaviour
{
    private MySqlConnection MS_Connection;
    private MySqlCommand MS_Command;


    private void Awake()
    {
        var connectionString = "Server = localhost ; Database = basedatos ; user = root ; password = ; Charset = utf8;";

        MS_Connection = new MySqlConnection(connectionString);
        MS_Connection.Open();

        string campo_descripcion = "Consigue 800 puntos";
        string query = "INSERT INTO Logros VALUES(id, '" + campo_descripcion + "', false)";

        MS_Command = new MySqlCommand(query, MS_Connection);

        MS_Command.ExecuteNonQuery();

        MS_Connection.Close();
    }
}

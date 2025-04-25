using UnityEngine;
using MySql.Data.MySqlClient;

public class patato : MonoBehaviour
{
    private MySqlConnection msConnection;
    private MySqlCommand msCommand;
    private MySqlDataReader msDataReader;

    void Start()
    {
        var query = "SELECT * FROM logros";
        var connectionString = "Server = localhost ; Database = basedatos ; user = root ; password = ; Charset = utf8;";

        msConnection = new MySqlConnection(connectionString);
        msConnection.Open();

        msCommand = new MySqlCommand(query, msConnection);
        
        msDataReader = msCommand.ExecuteReader();

        while (msDataReader.Read())
        {
            Debug.Log(msDataReader[0] + " " + msDataReader[1] + " " + msDataReader[2]);
        }

        msDataReader.Close();
    }
}

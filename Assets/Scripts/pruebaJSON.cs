using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using TMPro;

public class pruebaJSON : MonoBehaviour
{
    public string password;
    public string correo;

    public int puntos;
    public int maxPuntos = 0;
    void Start()
    {
        puntos = 0;
    }

    void SaveData()
    {
        SaveData model = new SaveData();
        model.playerName = correo;
        model.email = correo;
        model.password = password;
        model.highScore = maxPuntos;

        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
        Debug.Log("Writing File to: " + Application.persistentDataPath);
    }

    void LoadData()
    {

    }
}

[Serializable]
public class SaveData
{
    public string email;
    public string playerName;
    public string password;
    public int highScore;
    public DateTime date;
}

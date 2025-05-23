using System.Collections;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Translator : TextMeshProUGUI
{
    [SerializeField] private Language _language;

    protected override void Awake()
    {
        base.Awake();

        _language = FindObjectOfType<Language>();
        //SqlSelect sqlLoader = new SqlSelect("SELECT " + _language.GetLanguage() + " FROM idiomos WHERE id='" + gameObject.name + "'");
        //var lines = sqlLoader.Execute();

        //text = lines[0];
    }
}
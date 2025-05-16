using System.Collections;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Translator : TextMeshProUGUI
{
    [SerializeField] private Language language;


    protected override void Awake()
    {
        base.Awake();


        //SqlSelect sqlloader = new SqlSelect ("SELECT" + language + "From Idiomos WHERE Id='"+ gameObject.name + "'")

    }
}

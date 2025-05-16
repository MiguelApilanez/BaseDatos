using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Language : MonoBehaviour
{
    [SerializeField] private string _language;

    public string GetLanguage()
    {
        return _language;
    }

}

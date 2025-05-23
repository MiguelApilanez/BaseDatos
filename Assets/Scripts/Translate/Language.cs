using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Language : MonoBehaviour
{
    public enum Languages
    {
        Spanish,
        English
    }

    [SerializeField] private Languages _language;

    public Languages GetLanguage()
    {
        return _language;
    }

}

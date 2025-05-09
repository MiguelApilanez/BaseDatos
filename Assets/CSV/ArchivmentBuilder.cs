using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArchivmentBuilder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _value;

    public void SetUp(Archivments archivment)
    {
        if (_name != null) _name.text = archivment.descripción;
        if (_value != null) _value.text = archivment.completado;
    }
}

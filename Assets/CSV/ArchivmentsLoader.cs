using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchivmentsLoader : MonoBehaviour
{
    [SerializeField] private GameObject _archivmentPrefab;

    private void Awake()
    {
        CSVLoader csvloader = new CSVLoader();
        string[] lines = csvloader.LoadCSV("Logros.csv");

        foreach (string line in lines)
        {
            var info = line.Split(",");
            Archivments archivment = new Archivments(info[0], info[1]);

            GameObject archivmentGO = Instantiate(_archivmentPrefab, this.transform);
            archivmentGO.GetComponent<ArchivmentBuilder>().SetUp(archivment);
        }
    }
}

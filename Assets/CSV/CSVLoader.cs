using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSVLoader
{
    public string[] LoadCSV(string filepath)
    {
        filepath = Path.Combine(Application.streamingAssetsPath, filepath);

        if(!File.Exists(filepath))
            return null;

        string[] lines = File.ReadAllLines(filepath);
        lines = lines.Skip(1).ToArray();

        return lines;
    }
}

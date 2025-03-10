using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class puntosJSON
{
    public playerController playerController;

    public string email;
    public int maxPoints;

    public puntosJSON(string email, int maxPoints)
    {
        this.email = email;
        this.maxPoints = maxPoints;
    }
}

[System.Serializable]
public class PlayerPointsDatabase
{
    public List<puntosJSON> playerPoints = new List<puntosJSON>();
}

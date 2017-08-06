using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStats
{
    public int shotsFired;
    public int hits;
    public int kills;
    public int deaths;

    public void Reset()
    {
        shotsFired = 0;
        hits = 0;
        kills = 0;
        deaths = 0;
    }
}

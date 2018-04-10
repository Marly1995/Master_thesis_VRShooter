using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldState
{
    private static WorldState instance = null;

    private WorldState() { }

    public static WorldState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WorldState();
            }
            return instance;
        }
    }

    private static int teleportIndex;
    public static int TeleportIndex
    {
        get { return teleportIndex; }
        set { teleportIndex = value; }
    }

    private static int score;
    public static int Score
    {
        get { return score; }
        set { score = value; }
    }

    private static int waves;
    public static int Waves
    {
        get { return waves; }
        set { waves = value; }
    }
}

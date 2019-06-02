using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public delegate void GameUpdateEvent();

    public static event GameUpdateEvent onResourceGained;

    public static void resourceGained()
    {
        if (onResourceGained != null)
        {
            onResourceGained();
        }
    }
}

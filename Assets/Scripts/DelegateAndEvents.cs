using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateAndEvents : MonoBehaviour
{
    // Start is called before the first frame update
    public delegate void UnitEventHandler(GameObject unit);

    public static event UnitEventHandler onUnitSpawn;
    public static event UnitEventHandler onUnitDestroy;

    public static void newUnitCreated(GameObject unitCreated)
    {
        if (onUnitSpawn != null) {
            onUnitSpawn(unitCreated);
        }
    }

    public static void unitDead(GameObject unitDead)
    {
        if (onUnitDestroy != null) {
            onUnitDestroy(unitDead);
        }
    }


}

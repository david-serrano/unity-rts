using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameUpdateCycle : MonoBehaviour
{
    private float counter = 0;
    private const int tenSeconds = 10;

    void Start()
    {
        //FloatingTextController.initialise();
    }

    void Update()
    {
       // Debug.Log("Update event");

        counter += Time.deltaTime;
      //  Debug.Log("counter update: " + counter);

        if (counter > tenSeconds)
        {
            counter = 0;
            GameEvents.resourceGained();
        }
    }
}

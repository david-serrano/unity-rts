using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolController : MonoBehaviour
{
    public static int schoolPoints = 0;
    private float counter = 0;
    private float showingTime = 2;
    private bool isTextShowing = false;
    private TextMeshPro textMesh;
    public int necessaryTeachersForPurchase = 0;
    public bool isPurchased = false;
    public int numberOfTeachersAvailable = 0;
    public int maxNumberOfTeachersInSchool = 0;

    void Awake()
    {
        textMesh = FindObjectOfType<TextMeshPro>();
    }

    public void purchaseSchool(GameObject teacher)
    {
        GameEvents.onResourceGained += this.onResourceGained;
        numberOfTeachersAvailable++;

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponents<Unit>() != null)
        {
            EventController.addEvent("Unit in range");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponents<Unit>() != null)
        {
            EventController.addEvent("Unit exiting range");
        }
    }

    private void Update()
    {
        if (isTextShowing)
        {
            if (counter > showingTime)
            {
                Debug.Log("counter reached, clearing");
                textMesh.text = "";
                counter = 0;
                isTextShowing = false;
            }
            else
            {
                counter += Time.deltaTime;
            }
        }
    }

    void onResourceGained()
    {
        EventController.addEvent("School point gained");
        Debug.Log("adding school point");

        textMesh.text = "+1";
        schoolPoints++;
        isTextShowing = true;
    }
}

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
    private TextMeshPro[] textMeshes;
    private TextMeshPro resourceCounter;
    private TextMeshPro teacherCount;
    public bool isPurchased = false;
    public int numberOfTeachersAvailable = 0;
    public int maxNumberOfTeachersInSchool = 0;

    private bool purchaseButtonVisible = false;
    private GameObject canvas;
    private Button purchaseButton;
    private Button instance;
    private GameObject purchasingUnit;

    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        textMeshes = FindObjectsOfType<TextMeshPro>();
        if(textMeshes.Length > 0)
        {
            for(int i = 0;i<textMeshes.Length;i++)
            {
                TextMeshPro currentIteration = textMeshes[i];
                if(currentIteration.name == "ResourceCounter")
                {
                    resourceCounter = currentIteration;
                } else if(currentIteration.name == "TeacherCount")
                {
                    teacherCount = currentIteration;
                    teacherCount.text = "";
                }
            }
        }
        if(!purchaseButton)
        {
            purchaseButton = Resources.Load<Button>("LoadablePrefabs/PurchaseButton");
        }
    }

    private void purchaseSchool()
    {
        if (!isPurchased)
        {
            GameEvents.onResourceGained += this.onResourceGained;
            numberOfTeachersAvailable++;
            isPurchased = true;
            EventController.addEvent("School purchased");
            Material activeSchoolMaterial = Resources.Load<Material>("LoadableMaterials/ActiveYellow");
            Renderer currentMaterial = gameObject.GetComponent<Renderer>();
            currentMaterial.material = activeSchoolMaterial;
            teacherCount.text = "Teachers: " + numberOfTeachersAvailable + "/" + maxNumberOfTeachersInSchool;
            if (purchasingUnit != null)
            {
                Mouse.removeUnitFromCurrentlySelectedUnits(purchasingUnit);
                Destroy(purchasingUnit);
                purchasingUnit = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponents<Unit>() != null)
        {
            EventController.addEvent("Unit in range");
            
            if(!isPurchased)
            {
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                try
                {
                    instance = Instantiate(purchaseButton);
                    instance.transform.SetParent(canvas.transform, false);
                    instance.transform.position = screenPosition;
                    instance.onClick.AddListener(purchaseButtonClicked);
                    purchaseButtonVisible = true;
                    purchasingUnit = other.gameObject;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
           // GameObject purchaseButton = GameObject.FindGameObjectWithTag("PurchaseButton");
           
           
        }
    }

    public bool getIsPurchased()
    {
        return isPurchased;
    }

    public bool getPurchaseButtonVisible()
    {
        return purchaseButtonVisible;
    }

    public string getSummaryTextForSchool()
    {
        return "Summary for school 1";
    }

    private void purchaseButtonClicked()
    {
        purchaseSchool();
        if (instance != null)
        {
            Destroy(instance.gameObject);
            purchaseButtonVisible = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponents<Unit>() != null)
        {
            EventController.addEvent("Unit exiting range");
            if(!isPurchased && instance != null)
            {
                Destroy(instance.gameObject);
                purchaseButtonVisible = false;
            }
        }
    }

    private void Update()
    {
        if (isTextShowing)
        {
            if (counter > showingTime)
            {
                //Debug.Log("counter reached, clearing");
                resourceCounter.text = "";
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
        //Debug.Log("adding school point");

        resourceCounter.text = "+1";
        gameObject.GetComponentInChildren<Animation>().Play();
        //resourceCounter.GetComponent<Animation>().Play("slide_up_and_fade");
        schoolPoints++;
        isTextShowing = true;
    }
}

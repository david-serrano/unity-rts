using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SchoolController : MonoBehaviour
{
    private float counter = 0;
    private float showingTime = 2;
    private bool isTextShowing = false;
    private TextMeshPro[] textMeshes;
    private TextMeshPro resourceCounter;
    private TextMeshPro teacherCount;
    private GameObject canvas;

    private bool purchaseButtonVisible = false;
    private Button purchaseButton;
    private Button purchaseButtonInstance;
    private GameObject purchasingUnit;
    private Transform purchasingUnitTransform;

    private bool houseUnitButtonVisible = false;
    private Button houseUnitButton;
    private Button houseUnitButtonInstance;
    private GameObject housedUnit;

    public static int schoolPoints = 0;
    public string schoolSummaryText = "Default school summary text";
    public bool isPurchased = false;
    public int numberOfTeachersAvailable = 0;
    public int maxNumberOfTeachersInSchool = 0;

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
        if (!houseUnitButton)
        {
            houseUnitButton = Resources.Load<Button>("LoadablePrefabs/HouseUnitButton");
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
            updateTeacherCount();
            if (purchasingUnit != null)
            {
                //FIXME copy by value not working
                //initialise
                if(!purchasingUnitTransform)
                {
                    purchasingUnitTransform = purchasingUnit.transform;
                }
                //copy by value
                CopyTransform(purchasingUnitTransform, purchasingUnit.transform);
                Mouse.removeUnitFromCurrentlySelectedUnits(purchasingUnit);
                Destroy(purchasingUnit);
                purchasingUnit = null;
            }
        }
    }

    void CopyTransform(Transform dst, Transform src)
    {
        dst.localPosition = src.localPosition;
        dst.localRotation = src.localRotation;
        dst.localScale = src.localScale;
    }

    private void updateTeacherCount()
    {
        teacherCount.text = "Teachers: " + numberOfTeachersAvailable + "/" + maxNumberOfTeachersInSchool;
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
                    purchaseButtonInstance = Instantiate(purchaseButton);
                    purchaseButtonInstance.transform.SetParent(canvas.transform, false);
                    purchaseButtonInstance.transform.position = screenPosition;

                    purchasingUnit = other.gameObject;
                    purchaseButtonInstance.onClick.AddListener(purchaseButtonClicked);
                    purchaseButtonVisible = true;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            } else if(numberOfTeachersAvailable < maxNumberOfTeachersInSchool)
            {
                Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                try
                {
                    houseUnitButtonInstance = Instantiate(houseUnitButton);
                    houseUnitButtonInstance.transform.SetParent(canvas.transform, false);
                    houseUnitButtonInstance.transform.position = screenPosition;

                    housedUnit = other.gameObject;
                    houseUnitButtonInstance.onClick.AddListener(houseUnit);
                    houseUnitButtonVisible = true;
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }           
        }
    }

    //TODO units should be held in a FIFO queue
    private void houseUnit()
    {
        Destroy(houseUnitButtonInstance.gameObject);
        houseUnitButtonVisible = false;

        Destroy(housedUnit);
        numberOfTeachersAvailable++;
        updateTeacherCount();
    }

    public bool getIsPurchased()
    {
        return isPurchased;
    }

    public bool getPurchaseButtonVisible()
    {
        return purchaseButtonVisible;
    }

    public bool getHouseUnitButtonVisible()
    {
        return houseUnitButtonVisible;
    }

    public string getSummaryTextForSchool()
    {
        return schoolSummaryText;
    }

    public void createUnit()
    {
        if(numberOfTeachersAvailable > 0)
        {
            GameObject unit = Resources.Load<GameObject>("LoadablePrefabs/Character");
            //x, y, z
            unit.transform.position = new Vector3(gameObject.transform.position.x + 4  , 1.0f, gameObject.transform.position.z);// gameObject.transform.position + (gameObject.transform.right * 3);
            // TODO this is broken because unit gets destroyed - 
            // need to copy by value: purchasingUnitTransform.position;
            numberOfTeachersAvailable--;
            updateTeacherCount();
            Instantiate(unit);
        }
    }

    private void purchaseButtonClicked()
    {
        purchaseSchool();
        if (purchaseButtonInstance != null)
        {
            Destroy(purchaseButtonInstance.gameObject);
            purchaseButtonVisible = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponents<Unit>() != null)
        {
            EventController.addEvent("Unit exiting range");
            if(!isPurchased && purchaseButtonInstance != null)
            {
                Destroy(purchaseButtonInstance.gameObject);
                purchaseButtonVisible = false;
            } else if (houseUnitButtonInstance != null)
            {
                Destroy(houseUnitButtonInstance.gameObject);
                houseUnitButtonVisible = false;
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
        if(numberOfTeachersAvailable > 0)
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
}

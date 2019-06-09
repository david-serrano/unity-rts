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

    private bool purchaseButtonVisible = false;
    private GameObject canvas;
    private Button purchaseButton;
    private Button instance;

    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        textMesh = FindObjectOfType<TextMeshPro>();
        if(!purchaseButton)
        {
            purchaseButton = Resources.Load<Button>("LoadablePrefabs/PurchaseButton");
        }
    }

    private void purchaseSchool()
    {
        GameEvents.onResourceGained += this.onResourceGained;
        numberOfTeachersAvailable++;
        isPurchased = true;
        EventController.addEvent("School purchased");
        Material activeSchoolMaterial = Resources.Load<Material>("LoadableMaterials/ActiveYellow");
        Renderer currentMaterial = gameObject.GetComponent<Renderer>();
        currentMaterial.material = activeSchoolMaterial;
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
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
           // GameObject purchaseButton = GameObject.FindGameObjectWithTag("PurchaseButton");
           
           
        }
    }

    public bool getPurchaseButtonVisible()
    {
        return purchaseButtonVisible;
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

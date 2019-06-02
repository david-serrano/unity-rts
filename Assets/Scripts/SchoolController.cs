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
    private TextMeshPro textMesh;

    void Awake()
    {
        GameEvents.onResourceGained += this.onResourceGained;
        textMesh = FindObjectOfType<TextMeshPro>();
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

     //   TextMeshPro textMesh = FindObjectOfType<TextMeshPro>();// transform.gameObject.GetComponent<TextMeshPro>();
        textMesh.text = "+1";
        isTextShowing = true;

      /*  Task.Delay(2000).ContinueWith(x =>
        {
            textMesh.text = "Done";
        });*/
        /*
        GameObject UItextGO = new GameObject("Text");
        UItextGO.transform.SetParent(transform);

        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

        Text text = UItextGO.AddComponent<Text>();
        text.text = "+1";
        text.fontSize = 16;
        text.color = Color.blue;
        */
        //FloatingTextController.createFloatingText("+1", transform);
    }
}

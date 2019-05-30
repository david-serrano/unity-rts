using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    RaycastHit hit;
    public GameObject target;
    public static GameObject currentlySelectedUnit;
    private static Vector3 mouseDownPoint;

    private void Awake()
    {
        mouseDownPoint = new Vector3();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
          //  Debug.Log("Hit: " + hit.collider.name + "x: " + hit.point.x + " y: " + hit.point.y + " z: " + hit.point.z);

            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
            }

            if(hit.collider.name == "TerrainMain") 
            {
                //0 left
                //1 right
                //2 middle
                if (Input.GetMouseButtonDown(1))
                {
                    GameObject targetObject = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                    targetObject.name = "Target Instantiated";
                } else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(hit.point))
                {
                    DeselectGameObjectIfSelected();
                }
            } else
            {
                //Debug.Log("Hit: " + hit.collider.name);
                if(/*Input.GetMouseButtonUp(0) &&*/ DidUserClickLeftMouse(mouseDownPoint)) {
                    Debug.Log("button up on: " +hit.collider.name);
                    if(hit.collider.transform.Find("Selected"))
                    {
                        Debug.Log("selectable unit found");
                   
                        if(currentlySelectedUnit != hit.collider.gameObject)
                        {
                            GameObject selectedObj = hit.collider.transform.Find("Selected").gameObject;
                            selectedObj.SetActive(true);

                            if(currentlySelectedUnit != null)
                            {
                                currentlySelectedUnit.transform.Find("Selected").gameObject.SetActive(false);
                            }
                            currentlySelectedUnit = hit.collider.gameObject;
                        }
                    } else
                    {
                        DeselectGameObjectIfSelected();
                    }
                }
            }
        } else {
            if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
            {
                DeselectGameObjectIfSelected();
            }
        }
   
        Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.yellow);
    }

    //mouse clivk with range of selection
    public bool DidUserClickLeftMouse(Vector3 hitPoint)
    {
        float clickZone = 1.3f;
        if (
            (mouseDownPoint.x < hitPoint.x && mouseDownPoint.x > hitPoint.x - clickZone) &&
         //   (mouseDownPoint.y < hitPoint.y && mouseDownPoint.y > hitPoint.y - clickZone) &&
            (mouseDownPoint.z < hitPoint.z && mouseDownPoint.z > hitPoint.z - clickZone)
        ) {
            Debug.Log("mouse in range");
            return true;
        } else
        {
            Debug.Log("mouse NOT in range");
            return false;
        }
    }

    public static void DeselectGameObjectIfSelected() {
        if(currentlySelectedUnit != null) {
            currentlySelectedUnit.transform.Find("Selected").gameObject.SetActive(false);
            currentlySelectedUnit = null;
        }
    }

}

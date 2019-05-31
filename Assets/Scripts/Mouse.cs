using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    public GameObject target;
    public static GameObject currentlySelectedTarget;
    private Vector3 mouseDownPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        mouseDownPoint = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
            }

            Debug.Log("Hit: " + hit.collider.name + "x: " + hit.point.x + " y: " + hit.point.y + " z: " + hit.point.z);
            switch (hit.collider.name)
            {
                case "TerrainMain":
                    //0 left
                    //1 right
                    //2 middle
                    if (Input.GetMouseButtonDown(1))
                    {
                        GameObject targetObject = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                        targetObject.name = "Target Instantiated";
                    }
                    else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        DeselectGameObjectIfSelected();
                    }

                    break;
                default:
                    if(Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)) {
                        if(hit.collider.transform.Find("Selected"))
                        {
                            Debug.Log("Found a unit!");

                            if (currentlySelectedTarget != hit.collider.gameObject)
                            {
                                GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                selectedObject.SetActive(true);

                                if(currentlySelectedTarget != null)
                                {
                                    currentlySelectedTarget.transform.Find("Selected").gameObject.SetActive(false);
                                }

                                currentlySelectedTarget = hit.collider.gameObject;
                            }
                        } else
                        {
                            DeselectGameObjectIfSelected();
                        }

                    }
                   /* if (Input.GetMouseButtonDown(1))
                    {
                        Projector selected = hit.collider.gameObject.GetComponentInChildren<Projector>();
                        selected.enabled = true;
                    }*/
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint)) {
                DeselectGameObjectIfSelected();
            }
        
        }

        Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.yellow);

    }

    public bool DidUserClickLeftMouse(Vector3 hitPoint)
    {
        float clickZone = 1.3f;

        if (
            (mouseDownPoint.x < hitPoint.x + clickZone && mouseDownPoint.x > hitPoint.x - clickZone) &&
            (mouseDownPoint.y < hitPoint.y + clickZone && mouseDownPoint.y > hitPoint.y - clickZone) &&
            (mouseDownPoint.z < hitPoint.z + clickZone && mouseDownPoint.z > hitPoint.z - clickZone)
        ) {
            return true;
        } else {
            return false;
        }
    }

    public static void DeselectGameObjectIfSelected()
    {
        if(currentlySelectedTarget != null)
        {
            currentlySelectedTarget.transform.Find("Selected").gameObject.SetActive(false);
            currentlySelectedTarget = null;
        }
    }
}

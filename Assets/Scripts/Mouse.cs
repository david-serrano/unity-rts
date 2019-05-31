using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    RaycastHit hit;
    public GameObject target;
    public static GameObject currentlySelectedTarget;
    private Vector3 mouseDownPoint;
    public static ArrayList currentlySelectedUnits = new ArrayList();

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

            // Debug.Log("Hit: " + hit.collider.name + "x: " + hit.point.x + " y: " + hit.point.y + " z: " + hit.point.z);
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
                        if(!shiftKeysDown()) {
                            DeselectGameObjectsIfSelected();
                        }
                    }

                    break;
                default:
                    if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        if (hit.collider.transform.Find("Selected"))
                        {
                            Debug.Log("Found a unit!");

                            if (!unitAlreadyInCurrentlySelectedUnits(hit.collider.gameObject))
                            {
                                if (!shiftKeysDown())
                                {
                                    DeselectGameObjectsIfSelected();
                                }

                                GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                selectedObject.SetActive(true);

                                currentlySelectedUnits.Add(hit.collider.gameObject);

                            }
                            else
                            {
                                if (shiftKeysDown())
                                {
                                    removeUnitFromCurrentlySelectedUnits(hit.collider.gameObject);
                                }
                                else
                                {
                                    DeselectGameObjectsIfSelected();

                                    GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                    selectedObject.SetActive(true);

                                    currentlySelectedUnits.Add(hit.collider.gameObject);
                                }

                            }
                        }
                        else
                        {
                            if (!shiftKeysDown())
                            {
                                DeselectGameObjectsIfSelected();
                            }
                        }

                    }
                    break;
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
            {
                DeselectGameObjectsIfSelected();
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
        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void DeselectGameObjectsIfSelected()
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject unitAtIndex = currentlySelectedUnits[i] as GameObject;
                unitAtIndex.transform.Find("Selected").gameObject.SetActive(false);
            }
            currentlySelectedUnits.Clear();
        }
    }

    public static bool unitAlreadyInCurrentlySelectedUnits(GameObject unit)
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject unitAtIndex = currentlySelectedUnits[i] as GameObject;
                if (unitAtIndex == unit)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public void removeUnitFromCurrentlySelectedUnits(GameObject unit)
    {
        if (currentlySelectedUnits.Count > 0)
        {
            for (int i = 0; i < currentlySelectedUnits.Count; i++)
            {
                GameObject unitAtIndex = currentlySelectedUnits[i] as GameObject;
                if (unitAtIndex == unit)
                {
                    currentlySelectedUnits.RemoveAt(i);
                    unitAtIndex.transform.Find("Selected").gameObject.SetActive(false);
                }
            }
        }
    }

    public static bool shiftKeysDown()
    {
        if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            return true;
        } else
        {
            return false;
        }
    }
}

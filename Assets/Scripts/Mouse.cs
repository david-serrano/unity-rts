using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    RaycastHit hit;
    public GameObject target;
    public static ArrayList currentlySelectedUnits = new ArrayList();
    public static ArrayList unitsOnScreen = new ArrayList();
    public static ArrayList unitsInDrag = new ArrayList();
    private bool finishedDragOnThisFrame;

    public static bool userIsDragging;
    private static float timeLimitBeforeDeclareDrag = 1f;
    private static float timeLeftBeforeDeclareDrag;
    private static Vector2 mouseDragStart;
    private static float clickDragZone = 1.3f;

    public GUIStyle mouseDragSkin;
    private static Vector3 mouseDownPoint;
    public static Vector3 mouseUpPoint;
    public static Vector3 currentMousePoint; //in world space

    //GUI
    private float boxWidth;
    private float boxHeight;
    private float boxLeft;
    private float boxTop;

    private Vector2 boxStart;
    private Vector2 boxFinish;

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
            currentMousePoint = hit.point;
            //Mouse drag
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
                timeLeftBeforeDeclareDrag = timeLimitBeforeDeclareDrag;
                mouseDragStart = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!userIsDragging)
                {
                    timeLeftBeforeDeclareDrag -= Time.deltaTime;
                    if (timeLeftBeforeDeclareDrag <= 0 || userDraggingByPosition(mouseDragStart, Input.mousePosition))
                    {
                        userIsDragging = true;
                    }
                }

                //GUI goes here
                if (userIsDragging) {
                 //   Debug.Log("User is dragging!");
                }
            } else  if (Input.GetMouseButtonUp(0))
            {
                timeLeftBeforeDeclareDrag = 0;
                userIsDragging = false;
            }

            //mouse click
            if (!userIsDragging)
            {
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
                            if (!shiftKeysDown())
                            {
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
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
            {
                if(!shiftKeysDown()) { 
                    DeselectGameObjectsIfSelected();
                }
            }

        }

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

     
        if(userIsDragging)
        {
            //update GUI variables once per update
            boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

            boxLeft = Input.mousePosition.x;
            boxTop = Screen.height - Input.mousePosition.y - boxHeight;

            if (FloatToBool(boxWidth))
            {
                if(FloatToBool(boxHeight))
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
                } else
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            } else if (!FloatToBool(boxWidth))
            {
                if (FloatToBool(boxHeight))
                {   
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
                } else
                {
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
                }
            }

            boxFinish = new Vector2(
                      boxStart.x + unsigned(boxWidth),
                      boxStart.y - unsigned(boxHeight)
                  );
        }
     
        Debug.Log(boxStart + "," + boxFinish);
    }

    private void LateUpdate()
    {
        Debug.Log(unitsOnScreen.Count);
    }

    void OnGUI()
    {
        if (userIsDragging) {   
            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
        }
    }

    public bool userDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        if((newPoint.x > dragStartPoint.x + clickDragZone || newPoint.x < dragStartPoint.x - clickDragZone) ||
            (newPoint.y > dragStartPoint.y + clickDragZone || newPoint.y < dragStartPoint.y - clickDragZone))
        {
            return true;
        }
        return false;
    }

    public bool DidUserClickLeftMouse(Vector3 hitPoint)
    {
        if (
            (mouseDownPoint.x < hitPoint.x + clickDragZone && mouseDownPoint.x > hitPoint.x - clickDragZone) &&
            (mouseDownPoint.y < hitPoint.y + clickDragZone && mouseDownPoint.y > hitPoint.y - clickDragZone) &&
            (mouseDownPoint.z < hitPoint.z + clickDragZone && mouseDownPoint.z > hitPoint.z - clickDragZone)
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

    public static void removeUnitFromOnScreen(GameObject unit)
    {
        for(int i =0; i< unitsOnScreen.Count; i++)
        {
            GameObject unitAtIndex = unitsOnScreen[i] as GameObject;
            if(unit == unitAtIndex)
            {
                unitsOnScreen.RemoveAt(i);
                unitAtIndex.GetComponent<Unit>().onScreen = false;
                break;
            }
        }
    }

    public static bool unitWithinScreenSpace(Vector2 unitScreenPos)
    {
        if((unitScreenPos.x < Screen.width && unitScreenPos.y < Screen.height) &&
            (unitScreenPos.x > 0f && unitScreenPos.y > 0f))
        {
            return true;
        }
        return false;
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

    public static float unsigned(float val)
    {
        if(val < 0f)
        {
            val *= -1;
        }
        return val;
    }

    public static bool FloatToBool(float f)
    {
        if(f < 0f)
        {
            return false;
        }
        return true;
    }
}

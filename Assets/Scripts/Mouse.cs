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

    public static Vector3 rightClickPoint;
    private bool finishedDragOnThisFrame;
    private bool startedDrag;

    public static bool userIsDragging;
    private static float timeLimitBeforeDeclareDrag = 1f;
    private static float timeLeftBeforeDeclareDrag;
    private static Vector2 mouseDragStart;
    private static float clickDragZone = 1.3f;

    public GUIStyle mouseDragSkin;
    private static Vector3 mouseDownPoint;
    public static Vector3 currentMousePoint; //in world space

    //GUI
    private float boxWidth;
    private float boxHeight;
    private float boxLeft;
    private float boxTop;

    private static Vector2 boxFinish;
    private static Vector2 boxStart;

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
                startedDrag = true;
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
                
            } else  if (Input.GetMouseButtonUp(0))
            {
                if(userIsDragging)
                {
                    finishedDragOnThisFrame = true;
                }
            
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
                            rightClickPoint = hit.point;
                            GameObject targetObject = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                            targetObject.name = "Target Instantiated";
                        }
                        else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                        {
                            if (!Common.shiftKeysDown())
                            {
                                DeselectGameObjectsIfSelected();
                            }
                        }

                        break;
                    default:
                        if (Input.GetMouseButtonDown(0) && DidUserClickLeftMouse(mouseDownPoint))
                        {
                            GameObject clickedObject = hit.collider.gameObject;
                            if (clickedObject.GetComponent<Unit>())
                            {
                                if (clickedObject.GetComponent<SchoolController>() != null && clickedObject.GetComponent<SchoolController>().getPurchaseButtonVisible())
                                {
                                    break;
                                }
                           
                               // if (hit.collider is SphereCollider || hit.collider is CapsuleCollider)
                              //  {
                                    if (!unitAlreadyInCurrentlySelectedUnits(clickedObject))
                                    {
                                        if (!Common.shiftKeysDown())
                                        {
                                            DeselectGameObjectsIfSelected();
                                        }

                                        GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                        selectedObject.SetActive(true);

                                        currentlySelectedUnits.Add(clickedObject);
                                        hit.collider.gameObject.GetComponent<Unit>().selected = true;
                                        EventController.addEvent("Unit selected");
                                    }
                                    else
                                    {
                                        if (Common.shiftKeysDown())
                                        {
                                            removeUnitFromCurrentlySelectedUnits(clickedObject);
                                        }
                                        else
                                        {
                                            DeselectGameObjectsIfSelected();

                                            GameObject selectedObject = hit.collider.transform.Find("Selected").gameObject;
                                            selectedObject.SetActive(true);

                                            currentlySelectedUnits.Add(clickedObject);
                                            hit.collider.gameObject.GetComponent<Unit>().selected = true;
                                            EventController.addEvent("Unit selected");
                                        }
                                    }
                               // }
                            }
                            else
                            {
                                if (!Common.shiftKeysDown())
                                {
                                    DeselectGameObjectsIfSelected();
                                }
                            }
                            
                        } else if (Input.GetMouseButtonDown(1))
                        {
                            rightClickPoint = hit.point;
                            GameObject targetObject = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                            targetObject.name = "Target Instantiated";
                        }
                        
                        break;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
            {
                if(!Common.shiftKeysDown()) { 
                    DeselectGameObjectsIfSelected();
                }
            }

        }

        if(!Common.shiftKeysDown() && startedDrag && userIsDragging)
        {
            DeselectGameObjectsIfSelected();
            startedDrag = false;
        }

        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);

     
        if(userIsDragging)
        {
            //update GUI variables once per update
            boxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(currentMousePoint).x;
            boxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(currentMousePoint).y;

            boxLeft = Input.mousePosition.x;
            boxTop = Screen.height - Input.mousePosition.y - boxHeight;

            if (Common.FloatToBool(boxWidth))
            {
                if(Common.FloatToBool(boxHeight))
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y + boxHeight);
                } else
                {
                    boxStart = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                }
            } else if (!Common.FloatToBool(boxWidth))
            {
                if (Common.FloatToBool(boxHeight))
                {   
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y + boxHeight);
                } else
                {
                    boxStart = new Vector2(Input.mousePosition.x + boxWidth, Input.mousePosition.y);
                }
            }

            boxFinish = new Vector2(
                      boxStart.x + Common.unsigned(boxWidth),
                      boxStart.y - Common.unsigned(boxHeight)
                  );
        }
     
      // Debug.Log(boxStart + "," + boxFinish);
    }

    private void LateUpdate()
    {
        unitsInDrag.Clear();

        if((userIsDragging || finishedDragOnThisFrame) && unitsOnScreen.Count > 0)
        {
            for(int i = 0; i < unitsOnScreen.Count; i ++)
            {
                GameObject unitObject = unitsOnScreen[i] as GameObject;
                Unit unitScript = unitObject.GetComponent<Unit>();
                GameObject selectedObject = unitObject.transform.Find("Selected").gameObject;

                if(!unitAlreadyInDraggedUnits(unitObject))
                {
                    if(unitInsideDrag(unitScript.screenPos))
                    {
                        selectedObject.SetActive(true);
                        unitsInDrag.Add(unitObject);
                    } else
                    {
                        if(!unitAlreadyInCurrentlySelectedUnits(unitObject))
                        {
                            selectedObject.SetActive(false);
                        }
                    }
                }
            }
        }

        if(finishedDragOnThisFrame)
        {
            finishedDragOnThisFrame = false;
            putDraggedUnitsInCurrentlySelectedUnits();
        }

    }

    void OnGUI()
    {
        if (userIsDragging) {   
            GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "", mouseDragSkin);
        }
    }


    //UTILITY METHODS

    /**
     * Click utility methods
     **/
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
                unitAtIndex.GetComponent<Unit>().selected = false;
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


    /**
    * Drag utility methods
    **/
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

    public static bool unitInsideDrag(Vector2 unitScreenPos)
    {
        if ((unitScreenPos.x > boxStart.x && unitScreenPos.y < boxStart.y) &&
           (unitScreenPos.x < boxFinish.x && unitScreenPos.y > boxFinish.y))
        {
            return true;
        }
        return false;
    }

    public static bool unitAlreadyInDraggedUnits(GameObject unit)
    {
        if (unitsInDrag.Count > 0)
        {
            for (int i = 0; i < unitsInDrag.Count; i++)
            {
                GameObject unitAtIndex = unitsInDrag[i] as GameObject;
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

    public static void putDraggedUnitsInCurrentlySelectedUnits()
    {
        if(!Common.shiftKeysDown())
        {
            DeselectGameObjectsIfSelected();
        } 
        
        if(unitsInDrag.Count > 0)
        {
            for(int i =0; i<unitsInDrag.Count; i++)
            {
                GameObject unitObject = unitsInDrag[i] as GameObject;
                if(!unitAlreadyInCurrentlySelectedUnits(unitObject))
                {
                    currentlySelectedUnits.Add(unitObject);
                    unitObject.GetComponent<Unit>().selected = true;
                }
            }

            unitsInDrag.Clear();
        }
    }


    public bool userDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        if ((newPoint.x > dragStartPoint.x + clickDragZone || newPoint.x < dragStartPoint.x - clickDragZone) ||
            (newPoint.y > dragStartPoint.y + clickDragZone || newPoint.y < dragStartPoint.y - clickDragZone))
        {
            return true;
        }
        return false;
    }
}

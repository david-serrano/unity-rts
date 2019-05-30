using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePoint : MonoBehaviour
{
    RaycastHit hit;
    int rayLength = 500;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, rayLength))
        {
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

                    break;
                case "Character":
                    if (Input.GetMouseButtonDown(1)) {
                        Projector selected = hit.collider.gameObject.GetComponentInChildren<Projector>();
                        selected.enabled = true;
                    }
                        break;
            }
         
        }

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow);

    }
}

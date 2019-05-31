using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UnitPath : MonoBehaviour
{
    private Seeker seeker;
    private CharacterController controller;
    private Unit unit;
    public Path path;

    public float speed;
    private int currentWaypoint = 0;

    //the max distance from the AI to a waypont for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    public void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
        unit = GetComponent<Unit>();
    }

    public void LateUpdate()
    {
        if(unit.selected && unit.isWalkable)
        {
            if(Input.GetMouseButtonDown(1))
            {
                seeker.StartPath(transform.position, Mouse.rightClickPoint, OnPathComplete);
            }
        }
    }

    public void FixedUpdate()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count || !unit.isWalkable)
        {
            return;
        }

        //calculate direction of unit
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        controller.SimpleMove(dir);

        //check if close enough to waypoint to proceed towards next
        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AStarPath : MonoBehaviour
{
    public Vector3 targetPosition;
    private Seeker seeker;
    private CharacterController controller;
    public Path path;

    public float speed;

    //the max distance from the AI to a waypont for it to continue to the next waypoint
    public float nextWaypointDistance = 3;

    private int currentWaypoint = 0;

    public void Start()
    {
        targetPosition = GameObject.Find("Target_Character").transform.position;
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }

    public void FixedUpdate()
    {
        if(path == null || currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //calculate direction of unit
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        controller.SimpleMove(dir);

        //check if close enough to waypoint to proceed towards next
        if(Vector3.Distance(transform.position, path.vectorPath[currentWaypoint])<nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
    public void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}

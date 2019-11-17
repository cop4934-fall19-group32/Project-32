using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    [SerializeField]
    Transform[] waypoints;

    [SerializeField]
    float moveSpeed = 2f;
    // the starting position is at the "rest" waypoint
    int waypointIndex = 2;

    void Start()
    {
        transform.position = waypoints [waypointIndex].transform.position;
    }

    void Update()
    {
        // testing movement
        Move();

        /* the following code will be used to interpret commands and move to the designated waypoints accordingly
         * 
         * if (instruction is INPUT)
         *      waypointIndex = 0;
         * if (instruction is OUTPUT)
         *      waypointIndex = 1;
         * if (instruction is {card related})
         *      waypointIndex = 3;
         * else
         *      waypointIndex = 2;
         *      
         * transform.position = Vector2.MoveTowards(transform.position,
         *                          waypoints[waypointIndex].transform.position,
         *                          moveSpeed * Time.deltaTime);     
         *      
         */
    }

    void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position,
                                waypoints[waypointIndex].transform.position,
                                moveSpeed * Time.deltaTime);

        if (transform.position == waypoints [waypointIndex].transform.position)
            waypointIndex++;

        if (waypointIndex == waypoints.Length)
            waypointIndex = 0;
    }
}

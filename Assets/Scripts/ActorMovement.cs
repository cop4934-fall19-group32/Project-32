using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    [SerializeField]
    Transform RestWaypoint;
    [SerializeField]
    Transform InputWaypoint;
    [SerializeField]
    Transform OutputWaypoint;
    [SerializeField]
    Transform RegisterWaypoint;

    [SerializeField]
    float moveSpeed = 2f;

    int x;


    void Start()
    {
        x = 0;
        transform.position = RestWaypoint.transform.position;
    }

    void Update()
    {
        // testing movement
        // this is a simple loop to illustrate the movement of the actor
        if (x == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            InputWaypoint.transform.position,
            moveSpeed * Time.deltaTime);
            if (transform.position == InputWaypoint.transform.position)
                x++;
        }
        if (x == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            OutputWaypoint.transform.position,
            moveSpeed * Time.deltaTime);
            if (transform.position == OutputWaypoint.transform.position)
                x++;
        }
        if (x == 2)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            RestWaypoint.transform.position,
            moveSpeed * Time.deltaTime);
            if (transform.position == RestWaypoint.transform.position)
                x++;
        }
        if (x == 3)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            RegisterWaypoint.transform.position,
            moveSpeed * Time.deltaTime);
            if (transform.position == RegisterWaypoint.transform.position)
                x++;
        }
        if (x == 4)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            RestWaypoint.transform.position,
            moveSpeed * Time.deltaTime);
            if (transform.position == RestWaypoint.transform.position)
                x++;
        }
        if (x == 5)
            x = 0;

        /* the following code will be used to interpret commands and move to the designated waypoints accordingly
         * 
         * if (instruction is INPUT)
         *      transform.position = Vector2.MoveTowards(transform.position,
         *                          InputWaypoint.transform.position,
         *                          moveSpeed * Time.deltaTime);
         * if (instruction is OUTPUT)
         *      transform.position = Vector2.MoveTowards(transform.position,
         *                          OutputWaypoint.transform.position,
         *                          moveSpeed * Time.deltaTime);
         * if (instruction is {register related})
         *      transform.position = Vector2.MoveTowards(transform.position,
         *                          RegisterWaypoint.transform.position,
         *                          moveSpeed * Time.deltaTime);
         * else
         *      transform.position = Vector2.MoveTowards(transform.position,
         *                          RestWaypoint.transform.position,
         *                          moveSpeed * Time.deltaTime);
         *      
         */
    }
}

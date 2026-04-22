using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;


public class EnemyMoveGun  : MonoBehaviour
{
    public float moveSpeed, distanceToStop;
    public Rigidbody theRigidbody;
    private Vector3 target;

    public NavMeshAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        target = PlayerControllerGun.instance.transform.position;
        //target.y = transform.position.y;

        agent.destination = target; 

        //transform.LookAt(target);

        //theRigidbody.linearVelocity = transform.forward * moveSpeed;

        if(Vector3.Distance(transform.position, target) > distanceToStop)
        {
            agent.destination = target;
        }
        else
        {
            agent.destination = transform.position;
        }
    }
}

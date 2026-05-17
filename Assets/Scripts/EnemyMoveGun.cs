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
    if (PlayerControllerGun.instance == null) return; // ADD THIS

    target = PlayerControllerGun.instance.transform.position;

    agent.destination = target;

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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public float moveSpeed, distanceToStop;
    public Rigidbody theRigidbody;
    private Vector3 target;
    public NavMeshAgent agent;

    void Start()
    {

    }

    void Update()
    {
        target = PlayerController.instance.transform.position;
        agent.destination = target;

        if (Vector3.Distance(transform.position, target) > distanceToStop)
        {
            agent.destination = target;
        }
        else
        {
            agent.destination = transform.position;
        }
    }
}
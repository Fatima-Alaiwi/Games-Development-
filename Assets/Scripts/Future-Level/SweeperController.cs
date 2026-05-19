using UnityEngine;
using System.Collections.Generic;

public class SweeperController : MonoBehaviour
{
    [Header("Path Settings")]
    public List<Transform> waypoints;
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float arrivalDistance = 0.5f;

    [Header("Mechanical Parts")]
    public Transform leftBrush;
    public Transform rightBrush;
    public List<Transform> wheels;
    public float brushRotateSpeed = 300f;
    public float wheelRotateSpeed = 500f;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints.Count == 0) return;

        MoveTowardsWaypoint();
        AnimateParts();
    }

    void MoveTowardsWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
    
        Vector3 targetDir = target.position - transform.position;
        targetDir.y = 0; 

        if (targetDir.sqrMagnitude < arrivalDistance * arrivalDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);


        float angle = Vector3.Angle(transform.forward, targetDir);
    
        if (angle < 30f) 
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    void AnimateParts()
    {
        float rotationThisFrame = Time.deltaTime;

        if (leftBrush) leftBrush.Rotate(0, brushRotateSpeed * rotationThisFrame, 0, Space.Self);
        if (rightBrush) rightBrush.Rotate(0, -brushRotateSpeed * rotationThisFrame, 0, Space.Self);

        foreach (Transform wheel in wheels)
        {
            if (wheel) wheel.Rotate(wheelRotateSpeed * rotationThisFrame, 0, 0, Space.Self);
        }
    }
}
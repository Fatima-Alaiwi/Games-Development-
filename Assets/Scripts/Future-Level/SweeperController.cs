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
    
        // 1. Get direction and flatten it (no tilting up or down)
        Vector3 targetDir = target.position - transform.position;
        targetDir.y = 0; 

        if (targetDir.sqrMagnitude < arrivalDistance * arrivalDistance)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            return;
        }

        // 2. Smooth Rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // 3. Movement - Only move if we are mostly facing the target
        // This prevents the car from moving sideways while still turning
        float angle = Vector3.Angle(transform.forward, targetDir);
    
        if (angle < 30f) // Only move forward if the angle is less than 30 degrees
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    void AnimateParts()
    {
        float rotationThisFrame = Time.deltaTime;

        // Rotate Brushes on Y Axis
        if (leftBrush) leftBrush.Rotate(0, brushRotateSpeed * rotationThisFrame, 0, Space.Self);
        if (rightBrush) rightBrush.Rotate(0, -brushRotateSpeed * rotationThisFrame, 0, Space.Self);

        // Rotate Wheels (usually on X or Z depending on model orientation)
        foreach (Transform wheel in wheels)
        {
            if (wheel) wheel.Rotate(wheelRotateSpeed * rotationThisFrame, 0, 0, Space.Self);
        }
    }
}
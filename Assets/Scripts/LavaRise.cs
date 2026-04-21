using UnityEngine;
using System.Collections;

public class LavaRise : MonoBehaviour
{
    public float initialSpeed = 0.2f;
    public float acceleration = 0.05f;   // Gets faster over time
    public float maxHeight = 10f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = initialSpeed;
    }

    void Update()
    {
        if (transform.position.y < maxHeight)
        {
            currentSpeed += acceleration * Time.deltaTime;
            transform.position += Vector3.up * currentSpeed * Time.deltaTime;
        }
    }
}
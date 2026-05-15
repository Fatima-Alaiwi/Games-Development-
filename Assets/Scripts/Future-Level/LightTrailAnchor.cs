using UnityEngine;

public class LightTrailAnchor : MonoBehaviour
{
    public TrailRenderer trail;
    public Color trailColor = Color.cyan;

    void Awake()
    {
        if (trail == null) trail = GetComponent<TrailRenderer>();
        
        // Apply the specific color to the trail
        if (trail != null)
        {
            trail.startColor = trailColor;
            // Fades to transparent at the end
            trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0); 
            trail.emitting = false; // Start off
        }
    }

    public void SetEmitting(bool state)
    {
        if (trail != null) trail.emitting = state;
    }
}
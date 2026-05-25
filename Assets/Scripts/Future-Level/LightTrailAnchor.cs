using UnityEngine;

public class LightTrailAnchor : MonoBehaviour
{
    public TrailRenderer trail;
    public Color trailColor = Color.cyan;

    void Awake()
    {
        if (trail == null) trail = GetComponent<TrailRenderer>();
        
        if (trail != null)
        {
            trail.startColor = trailColor;
            trail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0); 
            trail.emitting = false;
        }
    }

    public void SetEmitting(bool state)
    {
        if (trail != null) trail.emitting = state;
    }
}

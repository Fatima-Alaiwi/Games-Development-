using UnityEngine;

public class CollectableEffect : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateSpeed = 90f;

    [Header("Bobbing")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;

    [Header("Glow Pulse")]
    public bool enableGlow = true;
    public Color glowColor = new Color(1f, 0.8f, 0f); // golden glow
    public float minGlowIntensity = 0.5f;
    public float maxGlowIntensity = 2f;
    public float glowPulseSpeed = 2f;

    private Vector3 startPosition;
    private Renderer[] renderers;
    private MaterialPropertyBlock propBlock;

    void Start()
    {
        startPosition = transform.position;
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        // Rotate
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0, Space.World);

        // Bob up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Pulse glow
        if (enableGlow)
        {
            float intensity = Mathf.Lerp(minGlowIntensity, maxGlowIntensity, 
                              (Mathf.Sin(Time.time * glowPulseSpeed) + 1f) / 2f);

            foreach (Renderer r in renderers)
            {
                r.GetPropertyBlock(propBlock);
                propBlock.SetColor("_EmissionColor", glowColor * intensity);
                r.SetPropertyBlock(propBlock);
            }
        }
    }
}
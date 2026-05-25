using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the PlayerDot Image on the HUD map.
/// Pulses the dot's scale and alpha to create a breathing glow effect,
/// similar to LightPulser.cs already in your project but for UI.
/// </summary>
[RequireComponent(typeof(Image))]
public class PlayerDotPulse : MonoBehaviour
{
    [Header("Scale Pulse")]
    public float scaleMin    = 0.80f;
    public float scaleMax    = 1.20f;
    public float scalePeriod = 1.2f;

    [Header("Alpha Pulse")]
    public float alphaMin    = 0.70f;
    public float alphaMax    = 1.00f;

    [Header("Ring (optional second Image child)")]
    [Tooltip("A slightly larger ring Image that pulses outward and fades.")]
    public Image ringImage;
    public float ringScaleMax = 2.2f;
    public float ringAlphaMax = 0.45f;

    private Image     _dot;
    private Color     _baseColor;

    void Awake()
    {
        _dot       = GetComponent<Image>();
        _baseColor = _dot.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * (2f * Mathf.PI / scalePeriod)) + 1f) * 0.5f;

        // Core dot
        float scale = Mathf.Lerp(scaleMin, scaleMax, t);
        transform.localScale = Vector3.one * scale;

        float alpha = Mathf.Lerp(alphaMin, alphaMax, t);
        _dot.color  = new Color(_baseColor.r, _baseColor.g, _baseColor.b, alpha);

        // Expanding ring
        if (ringImage != null)
        {
            // Ring expands and fades on opposite phase
            float rt    = (Mathf.Sin(Time.time * (2f * Mathf.PI / scalePeriod) + Mathf.PI) + 1f) * 0.5f;
            float rs    = Mathf.Lerp(1f, ringScaleMax, rt);
            float ra    = Mathf.Lerp(ringAlphaMax, 0f, rt);
            ringImage.rectTransform.localScale = Vector3.one * rs;
            Color rc = ringImage.color;
            ringImage.color = new Color(rc.r, rc.g, rc.b, ra);
        }
    }
}

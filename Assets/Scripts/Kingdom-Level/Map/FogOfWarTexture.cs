using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a grayscale fog-of-war RenderTexture.
/// White = visible, Black = hidden.
/// Attach to any persistent GameObject (e.g. GameManager).
/// Call RevealAt() from MapUI whenever a new location is discovered.
/// </summary>
public class FogOfWarTexture : MonoBehaviour
{
    public static FogOfWarTexture Instance { get; private set; }

    [Header("Texture Settings")]
    [Tooltip("Resolution of the fog mask. 256 is fine for a small HUD map.")]
    public int resolution = 256;

    [Header("Reveal Settings")]
    [Range(0.05f, 0.4f)]
    [Tooltip("Radius of the revealed circle as a fraction of the map (0–1).")]
    public float revealRadius = 0.18f;

    [Range(0.01f, 0.15f)]
    [Tooltip("Soft falloff width at the edge of each revealed circle.")]
    public float edgeSoftness = 0.06f;

    // The fog texture other scripts can read
    public RenderTexture FogTexture { get; private set; }

    // Internal pixel buffer (CPU side for combining circles)
    private float[] _fogBuffer;   // 0 = hidden, 1 = fully revealed
    private Texture2D _cpuTex;

    // Tracks which UVs have been revealed so we can rebuild after scene load
    private readonly List<Vector2> _revealedPoints = new List<Vector2>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _fogBuffer = new float[resolution * resolution];
        _cpuTex    = new Texture2D(resolution, resolution, TextureFormat.RFloat, false);
        FogTexture = new RenderTexture(resolution, resolution, 0, RenderTextureFormat.RFloat);
        FogTexture.filterMode = FilterMode.Bilinear;
        FogTexture.wrapMode   = TextureWrapMode.Clamp;

        // Start fully hidden
        ClearFog();
    }

    void OnDestroy()
    {
        if (FogTexture != null) FogTexture.Release();
    }

    // ── Public API ──────────────────────────────────────────────────────

    /// <summary>Reveal a circular area at the given map UV (0-1 space).</summary>
    public void RevealAt(Vector2 uv)
    {
        _revealedPoints.Add(uv);
        PaintCircle(uv);
        UploadToGPU();
    }

    /// <summary>Reveal the player's current position each frame (small radius).</summary>
    public void RevealPlayerAt(Vector2 uv, float radiusOverride = -1f)
    {
        float r = radiusOverride > 0 ? radiusOverride : revealRadius * 0.6f;
        PaintCircle(uv, r);
        UploadToGPU();
    }

    /// <summary>Reset fog to fully hidden (call on new game).</summary>
    public void ClearFog()
    {
        for (int i = 0; i < _fogBuffer.Length; i++) _fogBuffer[i] = 0f;
        _revealedPoints.Clear();
        UploadToGPU();
    }

    // ── Internal ────────────────────────────────────────────────────────

    void PaintCircle(Vector2 uv, float radiusOverride = -1f)
    {
        float r = radiusOverride > 0 ? radiusOverride : revealRadius;
        int cx = Mathf.RoundToInt(uv.x * resolution);
        int cy = Mathf.RoundToInt(uv.y * resolution);
        int pixR = Mathf.RoundToInt(r * resolution);

        for (int py = cy - pixR; py <= cy + pixR; py++)
        {
            for (int px = cx - pixR; px <= cx + pixR; px++)
            {
                if (px < 0 || px >= resolution || py < 0 || py >= resolution) continue;

                float dist = Vector2.Distance(new Vector2(px, py), new Vector2(cx, cy));
                float normDist = dist / pixR;

                float value;
                float softPixels = edgeSoftness * resolution;
                if (dist <= pixR - softPixels)
                    value = 1f;
                else
                    value = Mathf.Lerp(1f, 0f, (dist - (pixR - softPixels)) / softPixels);

                int idx = py * resolution + px;
                _fogBuffer[idx] = Mathf.Max(_fogBuffer[idx], value);
            }
        }
    }

    void UploadToGPU()
    {
        // Write float buffer into Texture2D
        Color[] pixels = new Color[resolution * resolution];
        for (int i = 0; i < _fogBuffer.Length; i++)
            pixels[i] = new Color(_fogBuffer[i], 0f, 0f, 1f);

        _cpuTex.SetPixels(pixels);
        _cpuTex.Apply();

        // Blit to RenderTexture
        Graphics.Blit(_cpuTex, FogTexture);
    }
}

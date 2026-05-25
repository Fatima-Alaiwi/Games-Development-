using UnityEngine;

/// <summary>
/// Defines the rectangular region of the world that the HUD map covers.
/// Create ONE asset via Assets > Create > MansionMap > MapBounds.
/// Set worldMin to the bottom-left corner of your level and worldMax to the top-right.
/// </summary>
[CreateAssetMenu(menuName = "MansionMap/MapBounds", fileName = "MapBounds")]
public class MapBounds : ScriptableObject
{
    [Tooltip("World-space XZ coordinate of the bottom-left corner of the map area.")]
    public Vector2 worldMin = new Vector2(-50f, -50f);

    [Tooltip("World-space XZ coordinate of the top-right corner of the map area.")]
    public Vector2 worldMax = new Vector2(50f, 50f);

    /// <summary>
    /// Converts a world XZ position to a normalised UV coordinate (0-1, 0-1).
    /// </summary>
    public Vector2 WorldToMapUV(Vector3 worldPos)
    {
        float u = Mathf.InverseLerp(worldMin.x, worldMax.x, worldPos.x);
        float v = Mathf.InverseLerp(worldMin.y, worldMax.y, worldPos.z);  // Z → V
        return new Vector2(u, v);
    }

    /// <summary>
    /// Converts a world XZ Vector2 to a normalised UV coordinate.
    /// </summary>
    public Vector2 WorldToMapUV(Vector2 worldXZ)
    {
        float u = Mathf.InverseLerp(worldMin.x, worldMax.x, worldXZ.x);
        float v = Mathf.InverseLerp(worldMin.y, worldMax.y, worldXZ.y);
        return new Vector2(u, v);
    }
}

using UnityEngine;

/// <summary>
/// ScriptableObject that defines a named location on the HUD map.
/// Create one asset per location via Assets > Create > MansionMap > MapLocation.
/// </summary>
[CreateAssetMenu(menuName = "MansionMap/MapLocation", fileName = "NewMapLocation")]
public class MapLocation : ScriptableObject
{
    [Header("Identity")]
    public string locationName;

    [Header("World Position")]
    [Tooltip("Place an empty GameObject at this location in the scene and copy its world XZ position here.")]
    public Vector2 worldPosition;          // X = world X, Y = world Z

    [Header("Map Pin Appearance")]
    public Sprite pinIcon;                 // assign a Sprite (tree, skull, book, gem, house icon)
    public Color  pinColor = Color.white;

    [Header("Quest Link (optional)")]
    [Tooltip("Name of the Quest ScriptableObject that completes when this location is reached.")]
    public string linkedQuestName;

    [HideInInspector] public bool isReached;
    [HideInInspector] public bool isVisible;   // revealed through fog
}

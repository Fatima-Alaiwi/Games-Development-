using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("References")]
    public MapBounds mapBounds;
    public MapLocation[] locations;
    public Transform playerTransform;

    [Header("UI Elements")]
    public RawImage fogOverlay;
    public RectTransform pinsContainer;
    public RectTransform mapRect;
    public Image playerDot;
    public CanvasGroup mapRevealAnim;

    [Header("Pin Settings")]
    public GameObject pinPrefab;
    public Color pinDefaultColor = new Color(0.78f, 0.57f, 0.16f);
    public Color pinReachedColor = new Color(0.31f, 0.78f, 0.47f);

    [Header("Map Size (match your MapPanel RectTransform)")]
    public float mapWidth = 280f;
    public float mapHeight = 280f;

    [Header("Player Dot Settings")]
    public float playerPulsePeriod = 1.2f;
    public float playerPulseMin = 0.7f;
    public float playerPulseMax = 1.15f;

    [Header("Fog Settings")]
    public float playerRevealRadius = 0.08f;

    [Header("Map Pickup")]
    public string mapItemName = "Map";
    public float revealFadeDuration = 1.2f;

    [Header("Proximity")]
    public float autoReachDistance = 5f;

    // Private
    private bool _mapUnlocked = false;
    private readonly Dictionary<MapLocation, RectTransform> _pins
        = new Dictionary<MapLocation, RectTransform>();
    private FogOfWarTexture _fog;

    // ── Lifecycle ────────────────────────────────────────────────────────

    void Start()
    {
        Debug.Log("MapUI Start() called");
        _fog = FogOfWarTexture.Instance;

        if (fogOverlay != null && _fog != null)
            fogOverlay.texture = _fog.FogTexture;

        // Hide MapPanel's OWN CanvasGroup
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        // Also hide mapRevealAnim if assigned
        if (mapRevealAnim != null)
        {
            mapRevealAnim.alpha = 0f;
            mapRevealAnim.interactable = false;
            mapRevealAnim.blocksRaycasts = false;
        }

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        Debug.Log("Init() frame 1");
        yield return null;
        Debug.Log("Init() frame 2");
        yield return null;

        Debug.Log($"mapBounds null? {mapBounds == null}");
        Debug.Log($"locations null? {locations == null}, length: {locations?.Length}");
        Debug.Log($"pinPrefab null? {pinPrefab == null}");
        Debug.Log($"pinsContainer null? {pinsContainer == null}");

        foreach (var loc in locations)
        {
            Debug.Log($"Spawning pin for: {loc?.locationName}");
            SpawnPin(loc);
        }

        Debug.Log("Starting WatchForMap...");
        StartCoroutine(WatchForMap());
    }

    void Update()
    {
        if (!_mapUnlocked || playerTransform == null) return;
        UpdatePlayerDot();
        UpdateFogForPlayer();
        CheckLocationProximity();
    }

    // ── Map unlock ───────────────────────────────────────────────────────

    IEnumerator WatchForMap()
    {
        Debug.Log("WatchForMap started");
        while (!_mapUnlocked)
        {
            Debug.Log("Checking for map item...");
            if (InventoryManager.instance != null &&
                InventoryManager.instance.HasItem(mapItemName))
            {
                Debug.Log("MAP FOUND - unlocking!");
                UnlockMap(instant: false);
                yield break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    void UnlockMap(bool instant)
    {
        if (_mapUnlocked) return; // guard against double-call
        _mapUnlocked = true;

        foreach (var loc in locations)
        {
            if (loc.isReached)
            {
                _fog?.RevealAt(mapBounds.WorldToMapUV(loc.worldPosition));
                RefreshPin(loc);
            }
        }

        if (instant)
        {
            if (mapRevealAnim != null)
            {
                mapRevealAnim.alpha = 1f;
                mapRevealAnim.interactable = true;
                mapRevealAnim.blocksRaycasts = true;
            }
        }
        else
        {
            StartCoroutine(FadeInMap());
        }

        GetComponent<MapToggle>()?.SetUnlocked();
    }

    IEnumerator FadeInMap()
    {
        float t = 0f;
        CanvasGroup cg = GetComponent<CanvasGroup>(); // get MapPanel's OWN CanvasGroup
        if (cg != null)
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
        while (t < revealFadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / revealFadeDuration);
            if (cg != null) cg.alpha = alpha;
            if (mapRevealAnim != null) mapRevealAnim.alpha = alpha;
            yield return null;
        }
    }

    // Keep this empty — WatchForMap handles everything now
    public void OnMapPickedUp() { }

    // ── Player dot ───────────────────────────────────────────────────────

    void UpdatePlayerDot()
    {
        if (playerDot == null) return;

        Vector2 uv = mapBounds.WorldToMapUV(playerTransform.position);

        // Clamp so dot never leaves the map panel
        uv.x = Mathf.Clamp01(uv.x);
        uv.y = Mathf.Clamp01(uv.y);

        // UV 0,0 = bottom-left → anchoredPosition relative to panel centre
        Vector2 localPos = new Vector2(
            (uv.x - 0.5f) * mapWidth,
            (uv.y - 0.5f) * mapHeight
        );
        playerDot.rectTransform.anchoredPosition = localPos;

        // Pulse
        float pulse = Mathf.Lerp(playerPulseMin, playerPulseMax,
            (Mathf.Sin(Time.time * (2f * Mathf.PI / playerPulsePeriod)) + 1f) * 0.5f);
        playerDot.rectTransform.localScale = Vector3.one * pulse;

        // Point in movement direction
        float yRot = playerTransform.eulerAngles.y;
        playerDot.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -yRot);
    }

    void UpdateFogForPlayer()
    {
        Vector2 uv = mapBounds.WorldToMapUV(playerTransform.position);
        _fog?.RevealPlayerAt(uv, playerRevealRadius);
    }

    // ── Location proximity ───────────────────────────────────────────────

    void CheckLocationProximity()
    {
        foreach (var loc in locations)
        {
            if (loc.isReached) continue;
            Vector3 locWorld = new Vector3(
                loc.worldPosition.x,
                playerTransform.position.y,
                loc.worldPosition.y);
            if (Vector3.Distance(playerTransform.position, locWorld) <= autoReachDistance)
                MarkLocationReached(loc);
        }
    }

    public void MarkLocationReached(MapLocation loc)
    {
        if (loc.isReached) return;
        loc.isReached = true;
        _fog?.RevealAt(mapBounds.WorldToMapUV(loc.worldPosition));
        RefreshPin(loc);
    }

    public void RevealLocationPin(MapLocation loc)
    {
        loc.isVisible = true;
        RefreshPin(loc);
    }

    // ── Pins ─────────────────────────────────────────────────────────────

    void SpawnPin(MapLocation loc)
    {
        if (pinPrefab == null || pinsContainer == null) return;
        if (_pins.ContainsKey(loc)) return;

        GameObject go = Instantiate(pinPrefab, pinsContainer);
        RectTransform rt = go.GetComponent<RectTransform>();
        _pins[loc] = rt;

        Vector2 uv = mapBounds.WorldToMapUV(loc.worldPosition);
        Vector2 localPos = new Vector2(
            (uv.x - 0.5f) * mapWidth,
            (uv.y - 0.5f) * mapHeight
        );

        // ADD THESE LOGS
        Debug.Log($"Pin [{loc.locationName}] worldPos:{loc.worldPosition} UV:{uv} localPos:{localPos}");

        rt.anchoredPosition = localPos;

        Image img = go.GetComponent<Image>();
        if (img != null && loc.pinIcon != null) img.sprite = loc.pinIcon;

        RefreshPin(loc);
    }

    void RefreshPin(MapLocation loc)
    {
        if (!_pins.TryGetValue(loc, out RectTransform rt)) return;
        Image img = rt.GetComponent<Image>();
        if (img == null) return;

        img.color = Color.white;
        rt.gameObject.SetActive(true);

        // Scale up slightly when reached
        rt.localScale = loc.isReached ? Vector3.one * 3f : Vector3.one;
    }
}
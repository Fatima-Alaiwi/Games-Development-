using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Attach to the MapPanel root GameObject.
/// Player presses M (or whatever key you set) to toggle the map open/closed.
/// The map only responds if it has been unlocked (map item picked up).
///
/// This is separate from MapUI so concerns stay separated:
///   MapUI    = data/state (location tracking, fog, pins)
///   MapToggle = input/animation (open, close, scale anim)
/// </summary>
public class MapToggle : MonoBehaviour
{
    [Header("Input")]
    public KeyCode toggleKey = KeyCode.M;

    [Header("Animation")]
    public float openDuration  = 0.18f;
    public float closeDuration = 0.14f;
    public AnimationCurve openCurve  = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("References")]
    public CanvasGroup canvasGroup;    // for alpha fade
    public RectTransform mapPanel;     // for scale pop

    private bool _isOpen     = false;
    private bool _unlocked   = false;
    private Coroutine _anim;

    // Called by MapUI.UnlockMap() after map pickup
    public void SetUnlocked() => _unlocked = true;

    void Update()
    {
        if (!_unlocked) return;
        if (Input.GetKeyDown(toggleKey))
            Toggle();
    }

    public void Toggle()
    {
        if (_isOpen) Close(); else Open();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(AnimateMap(opening: true));
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(AnimateMap(opening: false));
    }

    IEnumerator AnimateMap(bool opening)
    {
        float dur = opening ? openDuration : closeDuration;
        AnimationCurve curve = opening ? openCurve : closeCurve;
        float t = 0f;

        CanvasGroup cg = GetComponent<CanvasGroup>();

        float fromAlpha = opening ? 0f : 1f;
        float toAlpha = opening ? 1f : 0f;
        Vector3 fromScale = opening ? Vector3.one * 0.85f : Vector3.one;
        Vector3 toScale = opening ? Vector3.one : Vector3.one * 0.85f;

        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float s = curve.Evaluate(Mathf.Clamp01(t / dur));
            if (mapPanel != null) mapPanel.localScale = Vector3.Lerp(fromScale, toScale, s);
            if (cg != null) cg.alpha = Mathf.Lerp(fromAlpha, toAlpha, s);
            yield return null;
        }

        // Never disable the GameObject — just set final alpha
        if (cg != null) cg.alpha = toAlpha;
        if (mapPanel != null) mapPanel.localScale = toScale;
    }
}

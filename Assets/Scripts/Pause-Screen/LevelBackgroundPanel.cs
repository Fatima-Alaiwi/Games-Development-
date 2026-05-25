using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelBackgroundPanel : MonoBehaviour
{
    [Serializable]
    private class LevelBackground
    {
        public SavedValue.LevelId level;
        public Sprite background = null;
    }

    private static readonly List<LevelBackgroundPanel> ActivePanels = new List<LevelBackgroundPanel>();

    [Header("UI")]
    [SerializeField] private Image panelImage;

    [Header("Level Backgrounds")]
    [SerializeField] private LevelBackground[] backgrounds =
    {
        new LevelBackground { level = SavedValue.LevelId.Lab },
        new LevelBackground { level = SavedValue.LevelId.Mansion },
        new LevelBackground { level = SavedValue.LevelId.Samurai },
        new LevelBackground { level = SavedValue.LevelId.Dungeon },
        new LevelBackground { level = SavedValue.LevelId.Kingdom },
        new LevelBackground { level = SavedValue.LevelId.Future }
    };

    private void Awake()
    {
        if (panelImage == null)
            panelImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (!ActivePanels.Contains(this))
            ActivePanels.Add(this);

        ApplyBackground();
    }

    private void OnDisable()
    {
        ActivePanels.Remove(this);
    }

    public void ApplyBackground()
    {
        if (panelImage == null)
            return;

        Sprite background = GetBackgroundForLevel(SavedValue.CurrentLevel);

        if (background == null)
        {
            Debug.LogWarning("No pause panel background assigned for level: " + SavedValue.CurrentLevel);
            return;
        }

        panelImage.sprite = background;
    }

    public static void SetCurrentLevel(SavedValue.LevelId level)
    {
        SavedValue.SetCurrentLevel(level);
    }

    public static void RefreshAllPanels()
    {
        for (int i = ActivePanels.Count - 1; i >= 0; i--)
        {
            if (ActivePanels[i] == null)
                ActivePanels.RemoveAt(i);
            else
                ActivePanels[i].ApplyBackground();
        }
    }

    public void SetCurrentLevelFromInspector(SavedValue.LevelId level)
    {
        SetCurrentLevel(level);
        ApplyBackground();
    }

    private Sprite GetBackgroundForLevel(SavedValue.LevelId level)
    {
        if (backgrounds == null)
            return null;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            if (backgrounds[i] != null && backgrounds[i].level == level)
                return backgrounds[i].background;
        }

        return null;
    }
}

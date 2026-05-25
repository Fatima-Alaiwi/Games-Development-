using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SavedValue : MonoBehaviour
{
    public enum LevelId
    {
        Lab,
        Mansion,
        Samurai,
        Dungeon,
        Kingdom,
        Future
    }

    private static SavedValue instance;
    private static LevelId currentLevel = LevelId.Kingdom;

    [SerializeField] private LevelId level = LevelId.Kingdom;

    public static LevelId CurrentLevel
    {
        get
        {
            if (instance != null)
                return instance.level;

            return currentLevel;
        }
    }

    private void Awake()
    {
        instance = this;
        currentLevel = level;
    }

    private void OnValidate()
    {
        currentLevel = level;

        if (Application.isPlaying)
            LevelBackgroundPanel.RefreshAllPanels();
    }

    public static void SetCurrentLevel(LevelId level)
    {
        currentLevel = level;

        if (instance != null)
            instance.level = level;

        LevelBackgroundPanel.RefreshAllPanels();
    }
}

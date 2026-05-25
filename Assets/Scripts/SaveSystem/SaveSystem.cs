using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    static string SavePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    static GameData _pendingLoad;

    // ── Public API ────────────────────────────────────────────────────────────

    public static bool HasSave() => File.Exists(SavePath);

    public static void Save()
    {
        GameData data = CollectData();
        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        Debug.Log($"[SaveSystem] Game saved → {SavePath}");
    }

    // Called by the main-menu Continue button.
    public static void ContinueGame()
    {
        _pendingLoad = LoadFromDisk();
        if (_pendingLoad == null)
        {
            Debug.LogWarning("[SaveSystem] No save file found.");
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoadedApply;
        SceneManager.LoadScene(_pendingLoad.sceneName);
    }

    // Applies the save file to the scene that is already open — for testing only.
    public static void ApplyToCurrentScene()
    {
        GameData data = LoadFromDisk();
        if (data == null)
        {
            Debug.LogWarning("[SaveSystem] No save file found — nothing to apply.");
            return;
        }
        ApplyData(data);
        Debug.Log("[SaveSystem] Save data applied to current scene.");
    }

    // Called by SaveApplyHelper after waiting one frame.
    public static void ApplyToCurrentScene(GameData data)
    {
        ApplyData(data);
        Debug.Log("[SaveSystem] Save data applied to current scene.");
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        Debug.Log("[SaveSystem] Save file deleted.");
    }

    // ── Internal ──────────────────────────────────────────────────────────────

    static void OnSceneLoadedApply(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedApply;

        // Use a temporary MonoBehaviour to wait one frame so all Start() methods
        // (HealthBar, InventorySlot) finish before we apply save data over them.
        GameObject helper = new GameObject("SaveApplyHelper");
        helper.AddComponent<SaveApplyHelper>().data = _pendingLoad;
        _pendingLoad = null;
    }

    static GameData CollectData()
    {
        GameData data = new GameData();
        data.sceneName = SceneManager.GetActiveScene().name;
        data.currentLevel = SavedValue.CurrentLevel.ToString();

        Actor player = FindPlayer();
        if (player != null)
        {
            data.playerHealth = player.currentHealth;
            data.playerMaxHealth = player.maxHealth;
            Transform root = player.transform.root;
            data.hasPositionData = true;
            data.playerPosition = root.position;
            data.playerEulerAngles = root.eulerAngles;
        }

        if (InventoryManager.instance != null)
        {
            data.inventory = new List<InventorySaveData>();
            foreach (var item in InventoryManager.instance.items)
                data.inventory.Add(new InventorySaveData { itemName = item.itemName, count = item.count });
        }

        if (QuestManager.Instance != null)
        {
            data.activeQuests = new List<QuestSaveData>();
            data.completedQuestNames = new List<string>();

            foreach (var q in QuestManager.Instance.activeQuests)
                data.activeQuests.Add(new QuestSaveData
                {
                    questName = q.questName,
                    currentAmount = q.currentAmount,
                    isCompleted = q.isCompleted
                });

            foreach (var q in QuestManager.Instance.completedQuests)
                data.completedQuestNames.Add(q.questName);
        }

        data.collectedObjectIds = CollectibleTracker.GetAll();

        data.openedDoorIds = new List<string>();
        foreach (var d in Object.FindObjectsByType<SaveableDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (d.isOpened && !string.IsNullOrEmpty(d.doorId))
                data.openedDoorIds.Add(d.doorId);

        data.triggeredSpawnerIds = new List<string>();
        foreach (var s in Object.FindObjectsByType<SaveableSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (s.hasTriggered && !string.IsNullOrEmpty(s.spawnerId))
                data.triggeredSpawnerIds.Add(s.spawnerId);

        return data;
    }

    static void ApplyData(GameData data)
    {
        if (data == null) return;

        // Restore current level tracker
        if (System.Enum.TryParse(data.currentLevel, out SavedValue.LevelId lid))
            SavedValue.SetCurrentLevel(lid);

        // Restore player position and health
        Actor player = FindPlayer();
        if (player != null)
        {
            if (data.hasPositionData)
            {
                Transform root = player.transform.root;
                CharacterController cc = root.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;
                root.position = data.playerPosition;
                root.eulerAngles = data.playerEulerAngles;
                if (cc != null) cc.enabled = true;
            }

            player.RestoreHealth(data.playerHealth, data.playerMaxHealth);
        }

        // Restore inventory
        if (InventoryManager.instance != null && data.inventory != null)
            InventoryManager.instance.LoadFromSave(data.inventory);

        // Restore quests
        if (QuestManager.Instance != null)
            QuestManager.Instance.LoadFromSave(data.activeQuests, data.completedQuestNames);

        // Hide already-collected objects
        CollectibleTracker.LoadFrom(data.collectedObjectIds);
        foreach (var c in Object.FindObjectsByType<Collectible>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (CollectibleTracker.IsCollected(c.collectibleId))
                c.gameObject.SetActive(false);

        // Reopen doors
        if (data.openedDoorIds != null)
        {
            var openedSet = new System.Collections.Generic.HashSet<string>(data.openedDoorIds);
            foreach (var d in Object.FindObjectsByType<SaveableDoor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (openedSet.Contains(d.doorId))
                    d.RestoreOpen();
        }

        // Retrigger spawners
        if (data.triggeredSpawnerIds != null)
        {
            var triggeredSet = new System.Collections.Generic.HashSet<string>(data.triggeredSpawnerIds);
            foreach (var s in Object.FindObjectsByType<SaveableSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (triggeredSet.Contains(s.spawnerId))
                    s.RestoreTriggered();
        }
    }

    static GameData LoadFromDisk()
    {
        if (!HasSave()) return null;
        return JsonUtility.FromJson<GameData>(File.ReadAllText(SavePath));
    }

    static Actor FindPlayer()
    {
        foreach (var a in Object.FindObjectsByType<Actor>(FindObjectsSortMode.None))
            if (a.isPlayer) return a;
        return null;
    }
}

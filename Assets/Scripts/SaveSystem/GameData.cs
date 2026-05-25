using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public string sceneName;
    public string currentLevel;
    public int playerHealth;
    public int playerMaxHealth;
    public bool hasPositionData;
    public Vector3 playerPosition;
    public Vector3 playerEulerAngles;
    public List<InventorySaveData> inventory = new List<InventorySaveData>();
    public List<QuestSaveData> activeQuests = new List<QuestSaveData>();
    public List<string> completedQuestNames = new List<string>();
    public List<string> collectedObjectIds = new List<string>();
    public List<string> openedDoorIds = new List<string>();
    public List<string> triggeredSpawnerIds = new List<string>();
}

[Serializable]
public class InventorySaveData
{
    public string itemName;
    public int count;
}

[Serializable]
public class QuestSaveData
{
    public string questName;
    public int currentAmount;
    public bool isCompleted;
}

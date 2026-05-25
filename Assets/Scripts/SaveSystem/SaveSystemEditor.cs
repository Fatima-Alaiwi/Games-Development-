#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class SaveSystemEditor
{
    [MenuItem("Tools/Save System/Open Save Folder")]
    static void OpenSaveFolder()
    {
        string path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
        Debug.Log("[SaveSystem] Save folder: " + path);
    }

    [MenuItem("Tools/Save System/Delete Save File")]
    static void DeleteSave()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("[SaveSystem] Save file deleted: " + path);
        }
        else
        {
            Debug.Log("[SaveSystem] No save file found at: " + path);
        }
    }

    [MenuItem("Tools/Save System/Print Save Path")]
    static void PrintSavePath()
    {
        Debug.Log("[SaveSystem] Save path: " + Path.Combine(Application.persistentDataPath, "savegame.json"));
    }
}
#endif

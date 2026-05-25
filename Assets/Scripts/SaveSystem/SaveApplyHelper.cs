using System.Collections;
using UnityEngine;

// Spawned by SaveSystem after a scene load. Waits one frame so all Start()
// methods finish before applying save data, then destroys itself.
public class SaveApplyHelper : MonoBehaviour
{
    public GameData data;

    IEnumerator Start()
    {
        yield return null;
        SaveSystem.ApplyToCurrentScene(data);
        Destroy(gameObject);
    }
}

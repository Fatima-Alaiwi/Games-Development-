using UnityEngine;

public class WinSoundPlayer : MonoBehaviour
{
    public AudioClip winSound;

    void Start()
    {
        if (winSound != null)
            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
    }
}

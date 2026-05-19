using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    public GameObject laserPrefab;
    public Transform muzzlePoint;
    public AudioSource shootAudio; // Sound of the gun
    public float fireRate = 2.0f;
    private float nextFire;

    void Start() {
        this.enabled = false;
    }

    void Update() {
        if (PauseMenuManager.isPaused) return;

        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot() {
        Instantiate(laserPrefab, muzzlePoint.position, muzzlePoint.rotation);
        if (shootAudio != null) shootAudio.Play();
    }
}
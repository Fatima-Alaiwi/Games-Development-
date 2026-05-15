using UnityEngine;

public class EnemyAI : MonoBehaviour 
{
    public enum Weapon { Assault, Pistol, Sword }
    public Weapon myWeapon;
    public Animator anim;

    void Start() 
    {
        // Randomize weapon type upon spawning from EnemySpawner
        myWeapon = (Weapon)Random.Range(0, 3);
        anim.SetInteger("WeaponType", (int)myWeapon);
        
        // Logic to activate the correct 3D model for the weapon goes here
    }

    void Update() 
    {
        // Maintain your existing 'isPaused' logic
        // This ensures robots stop moving and animating when the game is paused
        if (PauseMenuManager.isPaused) 
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;

        // If weapon is Pistol and it's time to attack, 
        // the animator stays in 'Pistol Idle' while your code fires the raycast
    }
}

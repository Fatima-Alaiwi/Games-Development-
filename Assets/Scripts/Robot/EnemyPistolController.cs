using UnityEngine;

public class EnemyPistolController : MonoBehaviour 
{
    public Animator anim;
    public GameObject pistolModel; // The 3D model of the pistol in the robot's hand

    void Start() 
    {
        // Ensure the pistol is visible upon spawning
        if (pistolModel != null) pistolModel.SetActive(true);
        
        // Trigger the Pistol state in your Animator
        // Set this to whatever index or trigger you assigned for Pistol
        anim.SetInteger("WeaponType", 1); 
    }

    void Update() 
    {
        // Use your Pause Manager to freeze animations
        if (PauseMenuManager.isPaused) 
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;
        
        // Attack logic will go here
    }
}
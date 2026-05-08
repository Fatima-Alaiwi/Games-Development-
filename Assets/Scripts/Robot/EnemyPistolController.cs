using UnityEngine;

public class EnemyPistolController : MonoBehaviour 
{
    public Animator anim;
    public GameObject pistolModel; 

    void Start() 
    {
        if (pistolModel != null) pistolModel.SetActive(true);
        anim.SetInteger("WeaponType", 1); 
    }

    void Update() 
    {
        if (PauseMenuManager.isPaused) 
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;
        
        // Attack logic will go here
    }
}
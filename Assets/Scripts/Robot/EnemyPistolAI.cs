using UnityEngine;

public class EnemyPistolAI : MonoBehaviour 
{
    public Animator anim;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    void Update() 
    {
        // Global Pause Check
        if (PauseMenuManager.isPaused) 
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;

        // Simple Attack Logic using the Pistol Idle pose
        if (Time.time - lastAttackTime > attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void PerformAttack()
    {
        // Trigger your muzzle flash/SFX here
        Debug.Log("Robot fired pistol!");
        // The robot stays in 'Pistol Idle' so the aim remains steady
    }
}
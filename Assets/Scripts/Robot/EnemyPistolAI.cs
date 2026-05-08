using UnityEngine;

public class EnemyPistolAI : MonoBehaviour 
{
    public Animator anim;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    void Update() 
    {
        if (PauseMenuManager.isPaused) 
        {
            anim.speed = 0;
            return;
        }
        anim.speed = 1;

        if (Time.time - lastAttackTime > attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    void PerformAttack()
    {
        Debug.Log("Robot fired pistol!");
    }
}
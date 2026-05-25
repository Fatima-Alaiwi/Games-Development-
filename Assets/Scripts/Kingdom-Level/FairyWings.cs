using UnityEngine;

public class FairyWings : MonoBehaviour
{
    Animator anim;

    [Header("Wing settings")]
    public float flapSpeedFlying = 1.5f;
    public float flapSpeedIdle = 0.4f;

    void Awake() => anim = GetComponent<Animator>();

    public void SetFlying(bool flying)
    {
        anim.SetBool("isFlying", flying);
        anim.SetFloat("wingSpeed",
            flying ? flapSpeedFlying : flapSpeedIdle);
    }

    void Update()
    {
        // Example: press Space to toggle
        if (Input.GetKeyDown(KeyCode.Space))
            SetFlying(!anim.GetBool("isFlying"));
    }
}
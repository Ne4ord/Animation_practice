using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.11f;  // 5 км/ч
    public float runSpeed = 8.33f;   // 30 км/ч
    public float animationSmooth = 10f;

    [Header("Health")]
    public int maxHealth = 5;

    [Header("Attack timing")]
    [Range(0f, 1f)] public float hitStartPercent = 0.35f;
    [Range(0f, 1f)] public float hitEndPercent = 0.65f;

    [Header("Camera")]
    public Transform cameraTransform;

    private Animator anim;
    public WeaponHitbox weaponHitbox;
    private bool isAttacking = false;
    private bool isJumpAttacking = false;
    private bool isDead = false;
    public int currentHealth;
    private float attackDuration;
    private float jumpAttackDuration;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (weaponHitbox == null)
            weaponHitbox = GetComponentInChildren<WeaponHitbox>();
        if (weaponHitbox == null)
            weaponHitbox = gameObject.AddComponent<WeaponHitbox>();

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.ToLower().Contains("jump"))
                jumpAttackDuration = clip.length;
            else if (clip.name.ToLower().Contains("attack"))
                attackDuration = clip.length;
        }
    }

    void Update()
    {
        if (isDead) return;
        if (Time.timeScale == 0f) return;

        if (isAttacking || isJumpAttacking)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0, v).normalized;

        if (input.magnitude > 0.1f)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * input.z + right * input.x;

            bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float targetSpeed = isRunning ? runSpeed : walkSpeed;

            transform.position += moveDirection * targetSpeed * Time.deltaTime;
            transform.forward = moveDirection;

            
            float currentAnimSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.Lerp(currentAnimSpeed, targetSpeed, Time.deltaTime * animationSmooth));
        }
        else
        {
            
            float currentAnimSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.Lerp(currentAnimSpeed, 0f, Time.deltaTime * animationSmooth));
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking && !isJumpAttacking)
            StartAttack();

        if (Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isJumpAttacking)
            StartJumpAttack();
    }

    void StartAttack()
    {
        isAttacking = true;

        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Attack");

        if (weaponHitbox != null)
        {
            weaponHitbox.ResetHitbox();
            weaponHitbox.canHit = false;
        }

        StartCoroutine(AttackRoutine("Attack", attackDuration, false));
    }

    void StartJumpAttack()
    {
        isJumpAttacking = true;

        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("JumpAttack");

        if (weaponHitbox != null)
        {
            weaponHitbox.ResetHitbox();
            weaponHitbox.canHit = false;
        }

        StartCoroutine(AttackRoutine("JumpAttack", jumpAttackDuration, true));
    }

    IEnumerator AttackRoutine(string stateName, float fallbackLength, bool jump)
    {
        float length = 0f;
        float waited = 0f;

        while (length <= 0f && waited < 0.5f)
        {
            AnimatorStateInfo next = anim.GetNextAnimatorStateInfo(0);
            AnimatorStateInfo current = anim.GetCurrentAnimatorStateInfo(0);

            if (next.IsName(stateName))
                length = next.length;
            else if (current.IsName(stateName))
                length = current.length;
            else
            {
                waited += Time.deltaTime;
                yield return null;
            }
        }

        if (length <= 0f)
            length = fallbackLength > 0f ? fallbackLength : 1f;

        yield return new WaitForSeconds(length * hitStartPercent);
        EnableHit();
        yield return new WaitForSeconds(length * (hitEndPercent - hitStartPercent));
        DisableHit();
        yield return new WaitForSeconds(length * (1f - hitEndPercent));

        if (jump)
            FinishJumpAttack();
        else
            FinishAttack();
    }

    void EnableHit()
    {
        if (weaponHitbox != null) weaponHitbox.canHit = true;
    }

    void DisableHit()
    {
        if (weaponHitbox != null) weaponHitbox.canHit = false;
    }

    void FinishAttack()
    {
        isAttacking = false;
        if (weaponHitbox != null)
            weaponHitbox.canHit = false;
    }

    void FinishJumpAttack()
    {
        isJumpAttacking = false;
        if (weaponHitbox != null) weaponHitbox.canHit = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Die");

        float dieDuration = 2f;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.ToLower().Contains("die"))
            {
                dieDuration = clip.length;
                break;
            }
        }

        Invoke(nameof(TriggerGameOver), dieDuration);
    }

    void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
    }
}

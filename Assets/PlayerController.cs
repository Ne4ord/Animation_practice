using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 1.11f;  // 5 км/ч
    public float runSpeed = 8.33f;   // 30 км/ч
    public float animationSmooth = 10f;

    [Header("Health")]
    public int maxHealth = 5;

    [Header("Weapon")]
    public GameObject sword;
    public GameObject swordOnBack;

    [Header("Camera")]
    public Transform cameraTransform;

    private Animator anim;
    private WeaponHitbox weaponHitbox;
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

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.ToLower().Contains("jump"))
                jumpAttackDuration = clip.length;
            else if (clip.name.ToLower().Contains("attack"))
                attackDuration = clip.length;
        }

        if (attackDuration <= 0) attackDuration = 3.34f;
        if (jumpAttackDuration <= 0) jumpAttackDuration = 2f;

        if (sword != null)
            weaponHitbox = sword.GetComponent<WeaponHitbox>();

        if (sword != null)
            sword.SetActive(true);
        if (swordOnBack != null)
            swordOnBack.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

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

            // Плавно меняем Speed в Animator
            float currentAnimSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.Lerp(currentAnimSpeed, targetSpeed, Time.deltaTime * animationSmooth));

            if (sword != null)
                sword.SetActive(false);
            if (swordOnBack != null)
                swordOnBack.SetActive(true);
        }
        else
        {
            // Плавно сбрасываем Speed до 0
            float currentAnimSpeed = anim.GetFloat("Speed");
            anim.SetFloat("Speed", Mathf.Lerp(currentAnimSpeed, 0f, Time.deltaTime * animationSmooth));

            if (sword != null)
                sword.SetActive(true);
            if (swordOnBack != null)
                swordOnBack.SetActive(false);
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

        if (sword != null)
            sword.SetActive(true);
        if (swordOnBack != null)
            swordOnBack.SetActive(false);

        if (weaponHitbox != null)
        {
            weaponHitbox.ResetHitbox();
            weaponHitbox.canHit = true;
        }

        Invoke(nameof(FinishAttack), attackDuration);
    }

    void FinishAttack()
    {
        isAttacking = false;
        if (weaponHitbox != null)
            weaponHitbox.canHit = false;
    }

    void StartJumpAttack()
    {
        isJumpAttacking = true;

        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("JumpAttack");

        if (sword != null) sword.SetActive(true);
        if (swordOnBack != null) swordOnBack.SetActive(false);

        if (weaponHitbox != null)
        {
            weaponHitbox.ResetHitbox();
            weaponHitbox.canHit = true;
        }

        Invoke(nameof(FinishJumpAttack), jumpAttackDuration);
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

        if (sword != null)
            sword.SetActive(false);
        if (swordOnBack != null)
            swordOnBack.SetActive(false);

        if (weaponHitbox != null)
            weaponHitbox.canHit = false;

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
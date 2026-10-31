using UnityEngine;
using System.Collections;
using System;



public class PlayerController : MonoBehaviour
{
    public event Action<int> OnHealthChanged;
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
    private bool isDead = false;
    public int currentHealth;
    private float attackDuration;

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (weaponHitbox == null)
            weaponHitbox = GetComponentInChildren<WeaponHitbox>();

       
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.ToLower().Contains("attack"))
            {
                attackDuration = clip.length;
                break;
            }
        }
    }

    void Update()
    {
        if (isDead) return; 
        if (Time.timeScale == 0f) return;
        if (isAttacking) return;

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

        if (Input.GetMouseButtonDown(0) && !isAttacking)
            StartAttack();
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

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(attackDuration * hitStartPercent);
        if (weaponHitbox != null) weaponHitbox.canHit = true;
        yield return new WaitForSeconds(attackDuration * (hitEndPercent - hitStartPercent));
        if (weaponHitbox != null) weaponHitbox.canHit = false;
        yield return new WaitForSeconds(attackDuration * (1f - hitEndPercent));
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        anim.SetFloat("Speed", 0f);

       

       
        if (weaponHitbox != null)
            weaponHitbox.canHit = false;
        TriggerGameOver();
    }

    void TriggerGameOver()
    {
        GameManager.Instance.GameOver();
    }
}
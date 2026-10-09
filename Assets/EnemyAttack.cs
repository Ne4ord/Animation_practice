using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 2f;
    public float attackRange = 2.5f;

    private Transform player;
    private float lastAttackTime;
    private Enemy enemy;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        if (enemy != null && enemy.IsDead()) return;
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(damage);
            Debug.Log("Enemy attacked! Player HP: ");
        }
    }
}
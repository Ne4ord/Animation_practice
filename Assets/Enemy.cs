using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 1;
    public float moveSpeed = 1.5f;
    public float stopDistance = 2f;

    private Transform player;
    private bool isDead = false;
    private Collider enemyCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        enemyCollider = GetComponent<Collider>();
        if (enemyCollider != null)
            enemyCollider.enabled = true;

        isDead = false;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > stopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            transform.position += direction * moveSpeed * Time.deltaTime;

            if (direction.magnitude > 0.1f)
                transform.forward = direction;
        }
    }

    public void TakeDamage()
    {
        if (isDead) return;

        isDead = true;
        Die();
    }

    void Die()
    {
        if (enemyCollider != null)
            enemyCollider.enabled = false;

        GameManager.Instance?.AddScore(1);
        EnemySpawner.Instance?.OnEnemyKilled();
        Destroy(gameObject, 0.3f);
    }

    public bool IsDead()
    {
        return isDead;
    }
}
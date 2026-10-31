using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    [Header("Stats")]
    public int health = 1;
    public float moveSpeed = 1.5f;
    public float stopDistance = 2f;

    [Header("Audio & VFX")]
    public AudioClip deathSound;      
    public GameObject explosionPrefab; 

    private Transform player;
    private bool isDead = false;
    private Collider enemyCollider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
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
            agent.isStopped = false;
            agent.SetDestination(player.position);


        }
        else
        {
            agent.isStopped = true;
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

       
        PlaySound2D(deathSound, 1f);

       
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        
        GameManager.Instance?.AddScore(1);
        EnemySpawner.Instance?.OnEnemyKilled();

       
        Destroy(gameObject, 0.3f);
    }

    public bool IsDead()
    {
        return isDead;
    }

   
    void PlaySound2D(AudioClip clip, float volume)
    {
        if (clip == null) return;

        
        GameObject tempAudioObject = new GameObject("TempAudio_" + clip.name);
        tempAudioObject.transform.position = transform.position;

        
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;

        
        audioSource.spatialBlend = 0f;

        
        audioSource.Play();

        
        Destroy(tempAudioObject, clip.length);
    }
}
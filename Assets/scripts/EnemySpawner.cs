using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public GameObject enemyPrefab;
    public int maxEnemies = 3;
    public float spawnDistanceMin = 5f;
    public float spawnDistanceMax = 10f;

    private int currentEnemies = 0;
    private Transform player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SpawnInitialEnemies();
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            SpawnEnemy();
        }
    }

    public void OnEnemyKilled()
    {
        currentEnemies--;
        if (currentEnemies < maxEnemies && enemyPrefab != null)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null) return;

        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(spawnDistanceMin, spawnDistanceMax);
        Vector3 randomPos = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        randomPos.y = 1;

        GameObject enemyObj = Instantiate(enemyPrefab, randomPos, Quaternion.identity);

        
        enemyObj.SetActive(true);
        foreach (MonoBehaviour mb in enemyObj.GetComponents<MonoBehaviour>())
        {
            if (mb is Enemy || mb is EnemyAttack)
                mb.enabled = true;
        }

        currentEnemies++;
    }
}
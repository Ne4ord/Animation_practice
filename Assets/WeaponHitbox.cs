using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    public Collider hitboxCollider;
    public bool canHit = false;

    private List<Enemy> hitEnemies = new List<Enemy>();

    void Start()
    {
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    void Update()
    {
        if (hitboxCollider != null)
            hitboxCollider.enabled = canHit;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            hitEnemies.Add(enemy);
            enemy.TakeDamage();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!canHit) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            hitEnemies.Add(enemy);
            enemy.TakeDamage();
        }
    }

    public void ResetHitbox()
    {
        hitEnemies.Clear();
    }
}
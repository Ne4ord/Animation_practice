using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    [System.NonSerialized] public bool canHit = false;

    [Header("Hit Area")]
    public float hitRadius = 1.5f;
    public float forwardOffset = 1.0f;
    public float heightOffset = 1.0f;

    private List<Enemy> hitEnemies = new List<Enemy>();

    void Start()
    {
        canHit = false;
        hitEnemies.Clear();
    }

    void Update()
    {
        if (!canHit) return;
        Vector3 center = transform.position + transform.forward * forwardOffset+ Vector3.up * heightOffset;

        Collider[] hits = Physics.OverlapSphere(center, hitRadius);
        foreach (Collider other in hits)
            TryHit(other);
    }

    void TryHit(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * forwardOffset + Vector3.up * heightOffset, hitRadius);
    }
}
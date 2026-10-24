using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    public Collider hitboxCollider;
    [System.NonSerialized] public bool canHit = false;

    public float hitRadius = 1.5f;
    public float forwardOffset = 1.0f;
    public float heightOffset = 1.0f;

    private List<Enemy> hitEnemies = new List<Enemy>();

    void Start()
    {
        canHit = false;
        hitEnemies.Clear();

        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    void Update()
    {
        if (hitboxCollider != null)
            hitboxCollider.enabled = canHit;

        if (!canHit) return;

        Vector3 center = transform.position
                         + transform.forward * forwardOffset
                         + Vector3.up * heightOffset;

        Collider[] hits = Physics.OverlapSphere(center, hitRadius);
        foreach (Collider other in hits)
            TryHit(other);

        if (hitboxCollider != null)
        {
            Bounds b = hitboxCollider.bounds;
            Collider[] swordHits = Physics.OverlapBox(b.center, b.extents, hitboxCollider.transform.rotation);
            foreach (Collider other in swordHits)
                TryHit(other);
        }
    }

    void OnTriggerEnter(Collider other) { if (canHit) TryHit(other); }
    void OnTriggerStay(Collider other)  { if (canHit) TryHit(other); }

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

using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    Collider2D col;
    readonly HashSet<int> hitThisSwing = new();
    public int damage = 10;    // set by EnemyMovement during Start()

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;   // disabled by default
    }

    public void BeginSwing() => hitThisSwing.Clear();
    public void Enable() => col.enabled = true;
    public void Disable() => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only damage the player (or anything with IDamageable)
        if (hitThisSwing.Add(other.gameObject.GetInstanceID()))
        {
            if (other.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage, transform.root.gameObject); // source = enemy
        }
    }
}
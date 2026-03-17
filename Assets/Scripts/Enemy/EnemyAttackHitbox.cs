using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    private int damage;
    private Collider2D col;
    private readonly HashSet<int> hitThisSwing = new();
    private EnemyShell enemy;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        enemy = GetComponentInParent<EnemyShell>();
    }

    public void BeginSwing()
    {
        hitThisSwing.Clear();
        if (enemy != null && enemy.currentData != null)
            damage = enemy.currentData.attackDamage;
    }

    public void Enable() => col.enabled = true;
    public void Disable() => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitThisSwing.Add(other.gameObject.GetInstanceID()))
        {
            if (other.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damage, enemy.gameObject);
        }
    }
}
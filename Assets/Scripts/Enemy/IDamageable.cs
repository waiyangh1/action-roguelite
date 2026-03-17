using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, GameObject source);
    void Die();
}
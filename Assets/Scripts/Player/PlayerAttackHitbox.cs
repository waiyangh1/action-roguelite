using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    Collider2D col;
    readonly HashSet<int> hitThisSwing = new();
    PlayerController player;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
        player = GetComponentInParent<PlayerController>();
    }

    public void BeginSwing() => hitThisSwing.Clear();
    public void Enable()     => col.enabled = true;
    public void Disable()    => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitThisSwing.Add(other.gameObject.GetInstanceID()))
        {
            if (other.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(player.Data.attackDamage, player.gameObject);
        }
    }
}

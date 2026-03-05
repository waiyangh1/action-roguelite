using System.Collections.Generic;
using UnityEngine;

// Place on a child GameObject with any Collider2D (circle, box, polygon, etc.).
// Collider starts disabled; animation events on the player root call
// PlayerController.EnableHitbox / DisableHitbox which forward here.
// BeginSwing clears the hit cache so each attack swing hits each enemy only once.
public class PlayerAttackHitbox : MonoBehaviour
{
    Collider2D col;
    readonly HashSet<int> hitThisSwing = new();

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void BeginSwing() => hitThisSwing.Clear();
    public void Enable()     => col.enabled = true;
    public void Disable()    => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hitThisSwing.Add(other.gameObject.GetInstanceID()))
            Debug.Log($"[Attack] Hit: {other.name}");
        // TODO: other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
    }
}

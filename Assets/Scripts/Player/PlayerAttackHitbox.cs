using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] float offsetDistance = 0.8f;

    Collider2D col;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    public void SetDirection(Vector2 dir)
    {
        transform.localPosition = (Vector3)(dir * offsetDistance);
    }

    public void Enable()  => col.enabled = true;
    public void Disable() => col.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hook up to enemy health here later
        Debug.Log($"[Attack] Hit: {other.name}");
    }
}

using UnityEngine;

public struct DamageEvent : IEvent
{
    public GameObject target;
    public int damage;
    public GameObject source;

    public DamageEvent(GameObject target, int damage, GameObject source)
    {
        this.target = target;
        this.damage = damage;
        this.source = source;
    }
}


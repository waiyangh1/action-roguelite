using UnityEngine;

public struct DeathEvent : IEvent
{
    public GameObject target;
    public DeathEvent(GameObject target) => this.target = target;
}
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSO", menuName = "Data/Player")]
public class PlayerSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.14f;
    public float dashCooldown = 0.5f;

    [Header("Attack")]
    public float lingerDuration = 0.5f;
    public float attackMoveSpeed = 1.5f;
    public float attackBufferDuration = 0.2f;
}

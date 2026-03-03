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
    public float[] hitOpenNorm  = { 0.30f, 0.28f, 0.30f };
    public float[] hitCloseNorm = { 0.88f, 0.88f, 0.98f };
}

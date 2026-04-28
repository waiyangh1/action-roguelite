using UnityEngine;

public class PlayerTransformAnchorAssigner : MonoBehaviour
{
    [SerializeField] private TransformAnchor playerAnchor;

    private void OnEnable()
    {
        if (playerAnchor != null)
            playerAnchor.Value = transform;
    }

    private void OnDisable()
    {
        if (playerAnchor != null && playerAnchor.Value == transform)
            playerAnchor.Value = null;
    }
}
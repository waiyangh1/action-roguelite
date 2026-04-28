using UnityEngine;

[CreateAssetMenu(fileName = "PlayerTransformAnchor", menuName = "Data/TransformAnchor")]
public class TransformAnchor : ScriptableObject
{
    public Transform Value;
}
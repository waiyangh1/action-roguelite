using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/SoundData")]
public class SoundDataSO : ScriptableObject
{
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1f;
    [Range(0.5f, 1.5f)] public float pitch = 1f;
    public bool isMusic; // If true, it uses the BGM channel
}
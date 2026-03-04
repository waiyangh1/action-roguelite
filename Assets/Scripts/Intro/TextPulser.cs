using UnityEngine;
using TMPro; // If using TextMeshPro

public class TextPulser : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 1f;

    void Update()
    {
        // Smoothly oscillate the alpha between min and max values
        float lerpTime = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, lerpTime);
    }
}
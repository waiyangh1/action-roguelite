using UnityEngine;
using System.Collections;

public class LoadingOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // For smoother feel

    public IEnumerator FadeIn()
    {
        canvasGroup.blocksRaycasts = true;
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Use the curve to evaluate the alpha
            canvasGroup.alpha = fadeCurve.Evaluate(timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeOut()
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Evaluate the curve in reverse for fading out
            canvasGroup.alpha = 1 - fadeCurve.Evaluate(timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
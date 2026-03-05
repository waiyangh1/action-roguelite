using System.Collections;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] float interval = 0.04f;
    [SerializeField] float ghostLifetime = 0.25f;
    [SerializeField] Color ghostColor = new Color(0.4f, 0.8f, 1f, 0.55f);

    SpriteRenderer spriteRenderer;
    Coroutine trailCoroutine;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Play()
    {
        if (trailCoroutine != null)
            StopCoroutine(trailCoroutine);

        trailCoroutine = StartCoroutine(SpawnLoop());
    }

    public void Stop()
    {
        if (trailCoroutine == null) return;

        StopCoroutine(trailCoroutine);
        trailCoroutine = null;
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnGhost();
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnGhost()
    {
        if (spriteRenderer.sprite == null) return;

        var ghost = new GameObject("Ghost");
        ghost.transform.SetPositionAndRotation(transform.position, transform.rotation);
        ghost.transform.localScale = transform.localScale;

        var sr = ghost.AddComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.flipX = spriteRenderer.flipX;
        sr.flipY = spriteRenderer.flipY;
        sr.sortingLayerID = spriteRenderer.sortingLayerID;
        sr.sortingOrder = spriteRenderer.sortingOrder - 1;
        sr.color = ghostColor;

        StartCoroutine(FadeOut(sr, ghost));
    }

    IEnumerator FadeOut(SpriteRenderer sr, GameObject ghost)
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < ghostLifetime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / ghostLifetime);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(ghost);
    }
}

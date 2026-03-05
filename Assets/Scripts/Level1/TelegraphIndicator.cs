using UnityEngine;

public class TelegraphIndicator : MonoBehaviour
{
    private SpriteRenderer sr;
    private float duration;
    private float timer;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    public void SetupLine(float time, Vector2 direction)
    {
        duration = time;
        timer = 0;

        // Point the long thin sprite at the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Ensure the line scales or positions correctly from the fire point
        gameObject.SetActive(true);
    }

    public void SetupCurve(float time, Vector2 direction)
    {
        duration = time;
        timer = 0;

        // Rotate the arc to face the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        gameObject.SetActive(true);
    }

    void Update()
    {
        timer += Time.deltaTime;
        float progress = timer / duration;

        // Visual feedback: gets brighter/redder as attack approaches
        sr.color = new Color(1, 0, 0, Mathf.Lerp(0.1f, 0.7f, progress));

        if (timer >= duration) gameObject.SetActive(false);
    }
}
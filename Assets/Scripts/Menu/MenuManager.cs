using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private SoundDataSO menuMusic;
    [SerializeField] private SoundDataSO panelSFX;

    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    

    private void Start()
    {
        if (menuMusic != null) EventBus.Publish(new PlaySoundEvent(menuMusic));

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }
    public void StartGame()
    {
        // Make sure "EquipmentScene" is added to your Build Settings!
        EventBus.Publish(new ChangeSceneEvent("Player"));
    }
    public void OpenSettings()
    {
        if (panelSFX != null) EventBus.Publish(new PlaySoundEvent(panelSFX));

        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);

        // 2. Start Pop Animation
        StopAllCoroutines();
        StartCoroutine(AnimatePanelPop(settingsPanel.transform));
    }
    public void CloseSettings()
    {
        if (panelSFX != null) EventBus.Publish(new PlaySoundEvent(panelSFX));

        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // 2. Start Pop Animation
        StopAllCoroutines();
        StartCoroutine(AnimatePanelPop(mainMenuPanel.transform));
    }
    private IEnumerator AnimatePanelPop(Transform panel)
    {
        // PS2 Pop Logic: Starts at 0, grows to 1.1, settles at 1.0
        panel.localScale = Vector3.zero;
        float timer = 0;
        float duration = 0.2f; // Very snappy for PS2 feel

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            // Simple bounce math
            float scaleValue = Mathf.Lerp(0, 1.1f, progress);
            if (progress > 0.8f) scaleValue = Mathf.Lerp(1.1f, 1.0f, (progress - 0.8f) * 5f);

            panel.localScale = new Vector3(scaleValue, scaleValue, 1);
            yield return null;
        }
        panel.localScale = Vector3.one;
    }

    public void QuitGame() => Application.Quit();
}
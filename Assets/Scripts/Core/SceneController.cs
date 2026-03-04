using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LoadingOverlay overlay;
    [SerializeField] private SoundDataSO introMusic;
    [SerializeField] private SoundDataSO transitionSFX; // Add this for the "Whoosh" or fade sound

    private string _currentContentScene = "";

    private void OnEnable() => EventBus.Subscribe<ChangeSceneEvent>(OnSceneChangeRequest);
    private void OnDisable() => EventBus.Unsubscribe<ChangeSceneEvent>(OnSceneChangeRequest);

    private void Start()
    {
        if (introMusic != null)
            EventBus.Publish(new PlaySoundEvent(introMusic));

        StartCoroutine(TransitionRoutine("introScene"));
    }

    private void OnSceneChangeRequest(IEvent e)
    {
        var eventData = (ChangeSceneEvent)e;
        StartCoroutine(TransitionRoutine(eventData.SceneName));
    }

    private IEnumerator TransitionRoutine(string newSceneName)
    {
        // 1. Play the transition sound effect immediately
        if (transitionSFX != null)
            EventBus.Publish(new PlaySoundEvent(transitionSFX));

        // 2. Stop music and start visual fade
        EventBus.Publish(new StopMusicEvent());

        if (overlay != null) yield return overlay.FadeIn();

        if (!string.IsNullOrEmpty(_currentContentScene))
            yield return SceneManager.UnloadSceneAsync(_currentContentScene);

        AsyncOperation op = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        while (!op.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newSceneName));
        _currentContentScene = newSceneName;

        // 3. Fade out the black overlay
        if (overlay != null) yield return overlay.FadeOut();
    }
}
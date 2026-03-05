using UnityEngine;

public class IntroInputHandler : MonoBehaviour
{
    [SerializeField] private string targetScene = "MenuScene";

    // This is the new part: Drag your SoundDataSO asset here in the Inspector
    [SerializeField] private SoundDataSO clickSfx;

    public void HandlePressStart()
    {
        Debug.Log("<color=green>SUCCESS:</color> Press Start Detected!");

        // 1. Tell the AudioManager (in CoreScene) to play the click sound
        if (clickSfx != null)
        {
            EventBus.Publish(new PlaySoundEvent(clickSfx));
        }

        // 2. Tell the SceneController (in CoreScene) to change the scene
        EventBus.Publish(new ChangeSceneEvent(targetScene));
    }
}
using UnityEngine;
using UnityEngine.UI;

public class GlobalUISoundHandler : MonoBehaviour
{
    [SerializeField] private SoundDataSO clickSound;

    void Start()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button b in allButtons)
        {
            b.onClick.AddListener(() => {
                if (clickSound != null) EventBus.Publish(new PlaySoundEvent(clickSound));
            });
        }
    }
}
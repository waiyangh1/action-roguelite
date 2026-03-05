using UnityEngine;
using UnityEngine.UI; // Fixes Slider error

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void OnEnable()
    {
        // Sync sliders to the saved data
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxSlider.value = PlayerPrefs.GetFloat("SfxVolume", 0.7f);
    }

    public void OnMusicSliderChanged()
    {
        EventBus.Publish(new VolumeChangedEvent(SoundType.Music, musicSlider.value));
    }

    public void OnSfxSliderChanged()
    {
        EventBus.Publish(new VolumeChangedEvent(SoundType.SFX, sfxSlider.value));
    }
}
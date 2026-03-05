using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int sfxPoolSize = 6;
    [SerializeField] private float musicFadeSpeed = 2f;

    private AudioSource _bgmSource;
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    // Memory for volumes
    private float _musicMasterVol = 1f;
    private float _sfxMasterVol = 1f;

    private void Awake()
    {
        // LOAD SAVED DATA
        _musicMasterVol = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        _sfxMasterVol = PlayerPrefs.GetFloat("SfxVolume", 0.7f);

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;
        _bgmSource.volume = 0; // Start at 0 for the first fade-in

        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            _sfxPool.Add(source);
        }
    }

    private void OnEnable()
    {
        EventBus.Subscribe<PlaySoundEvent>(OnPlaySound);
        EventBus.Subscribe<StopMusicEvent>(e => StartCoroutine(FadeOutMusic()));
        EventBus.Subscribe<VolumeChangedEvent>(OnVolumeChanged);
    }

    private void OnVolumeChanged(IEvent e)
    {
        var data = (VolumeChangedEvent)e;
        if (data.Type == SoundType.Music)
        {
            _musicMasterVol = data.Volume;
            _bgmSource.volume = _musicMasterVol;
            PlayerPrefs.SetFloat("MusicVolume", _musicMasterVol);
        }
        else
        {
            _sfxMasterVol = data.Volume;
            PlayerPrefs.SetFloat("SfxVolume", _sfxMasterVol);
        }
    }

    private void OnPlaySound(IEvent e)
    {
        var data = ((PlaySoundEvent)e).Data;
        if (data.isMusic) StartCoroutine(FadeInMusic(data));
        else PlaySfx(data);
    }

    private void PlaySfx(SoundDataSO data)
    {
        foreach (var source in _sfxPool)
        {
            if (!source.isPlaying)
            {
                source.clip = data.clip;
                source.volume = data.volume * _sfxMasterVol; // Applies the master slider
                source.pitch = data.pitch;
                source.Play();
                return;
            }
        }
    }

    private IEnumerator FadeInMusic(SoundDataSO data)
    {
        if (_bgmSource.clip == data.clip && _bgmSource.isPlaying) yield break;
        yield return StartCoroutine(FadeOutMusic());

        _bgmSource.clip = data.clip;
        _bgmSource.Play();

        float target = data.volume * _musicMasterVol;
        while (_bgmSource.volume < target)
        {
            _bgmSource.volume += Time.deltaTime * musicFadeSpeed;
            yield return null;
        }
    }

    private IEnumerator FadeOutMusic()
    {
        while (_bgmSource.volume > 0)
        {
            _bgmSource.volume -= Time.deltaTime * musicFadeSpeed;
            yield return null;
        }
        _bgmSource.Stop();
    }
}
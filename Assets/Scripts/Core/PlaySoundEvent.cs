public class PlaySoundEvent : IEvent
{
    public SoundDataSO Data;
    public PlaySoundEvent(SoundDataSO data) => Data = data;
}

public class StopMusicEvent : IEvent { }

public enum SoundType { Music, SFX }

public class VolumeChangedEvent : IEvent
{
    public SoundType Type;
    public float Volume;
    public VolumeChangedEvent(SoundType type, float vol)
    {
        Type = type;
        Volume = vol;
    }
}
using Blade.SoundSystem;
using PaperFlower.Core;
using UnityEngine;

public static class SoundEvents
{
    public static PlaySFXEvent PlaySFXEvent = new PlaySFXEvent();
    public static StopSoundEvent StopSoundEvent = new StopSoundEvent();
}

public class PlaySFXEvent : GameEvent
{
    public SoundSO clip;
    public Vector3 position;
    public bool withPosition = false;
    public int channel;

    public PlaySFXEvent Initialize(SoundSO clip, Vector3 position, int channel = 0)
    {
        this.clip = clip;
        this.position = position;
        this.channel = channel;
        withPosition = true;
        return this;
    }
    
    public PlaySFXEvent Initialize(SoundSO clip, int channel = 0)
    {
        this.clip = clip;
        this.channel = channel;
        withPosition = false;
        return this;
    }
}

public class StopSoundEvent : GameEvent
{
    public int channel;

    public StopSoundEvent Initialize(int channel = 0)
    {
        this.channel = channel;
        return this;
    }
}
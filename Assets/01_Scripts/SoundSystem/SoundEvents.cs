using Blade.SoundSystem;
using PaperFlower.Core;

public static class SoundEvents
{
    public static PlaySFXEvent PlaySFXEvent = new PlaySFXEvent();
    public static StopSoundEvent StopSoundEvent = new StopSoundEvent();
}

public class PlaySFXEvent : GameEvent
{
    public SoundSO clip;
    public int channel;

    public PlaySFXEvent Initialize(SoundSO clip, int channel = 0)
    {
        this.clip = clip;
        this.channel = channel;
            
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
using System;

public class CuePoint
{
    private CuePointStatus cuePointStatus;
    public CuePointStatus CuePointStatus { get => cuePointStatus; }

    public bool CueActivate = false;

    private TimeSpan cuePointTime;
    public TimeSpan CuePointTime { get => cuePointTime; }

    public CuePoint()
    {
        cuePointStatus = CuePointStatus.Empty;
    }

    public void SetCuePointTime(TimeSpan time)
    {
        cuePointTime = time;
        cuePointStatus = CuePointStatus.Created;
    }

    public void ResetCuePoint()
    {
        cuePointTime = TimeSpan.FromMilliseconds(0);
        cuePointStatus = CuePointStatus.Empty;
    }
}
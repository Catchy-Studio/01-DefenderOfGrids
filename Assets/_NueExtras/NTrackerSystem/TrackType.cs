namespace _NueExtras.NTrackerSystem
{
    public enum TrackType
    {
        TotalRunCount =0,
        WinCount = 1,
        LoseCount =2,
        AbandonCount = 3,
        TotalRunTime = 4,
        LongestRunTime = 5,
        FastestRunTime = 6,
    }
    
    public static class TrackTypeExtensions
    {
        public static string GetKey(this TrackType trackType)
        {
            return trackType.ToString();
        }
    }
}
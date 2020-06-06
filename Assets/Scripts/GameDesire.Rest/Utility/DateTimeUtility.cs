using System;

namespace GameDesire.Rest.Utility
{
    public static class DateTimeUtility
    {
        public static readonly DateTime EpochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan GetTimeSinceEpoch()
        {
            return DateTime.UtcNow - EpochDate;
        }

        public static double GetCurrentPosixMiliseconds()
        {
            var miliseconds = GetTimeSinceEpoch().TotalMilliseconds;
            var absoluteMiliseconds = miliseconds - miliseconds % 1;
            return absoluteMiliseconds;
        }
    }
}
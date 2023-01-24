using CheckInProject.CheckInCore.Models;

namespace CheckInProject.CheckInCore.Utils
{
    public static class TimeConverter
    {
        public readonly static TimeOnly Morning = TimeOnly.MinValue;
        public readonly static TimeOnly Afternoon = TimeOnly.Parse("12:00");
        public readonly static TimeOnly Evening = TimeOnly.Parse("18:00");
        public static TimeEnum ConvertTimeToDescription(TimeOnly sourceTime)
        {
            if (sourceTime >= Morning && sourceTime < Afternoon) return TimeEnum.Morning;
            if (sourceTime >= Afternoon && sourceTime < Evening) return TimeEnum.Afternoon;
            else return TimeEnum.Evening;
        }
    }
}

using System;

namespace WebRequestsAutomater.Common
{
    public static class Helpers
    {
        public static int GetRandomTimeSpanInMiliseconds()
        {
            int min;
            int max;
            var isWeekend = DateTime.Today.DayOfWeek == DayOfWeek.Saturday ||
                DateTime.Today.DayOfWeek == DayOfWeek.Sunday;
            min = isWeekend
                ? 960000
                : 420000;
            max = isWeekend
                ? 4080000
                : 1440000;
            var randomMs = new Random();

            return randomMs.Next(min, max);
        }

        public static string FormatMilisecondsToMinutes(int miliseconds)
        {
            var totalSeconds = Math.Floor(Convert.ToDouble(miliseconds / 1000 / 60));
            var totalSecondsAsMiliseconds = totalSeconds * 60 * 1000;
            var span = (miliseconds - totalSecondsAsMiliseconds) / 1000;
            return $"{totalSeconds} minutes {span} seconds";
        }
    }
}

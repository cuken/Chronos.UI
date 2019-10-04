using System;

namespace Chronos.UI.Models
{
    public class Activity
    {
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Comment { get; set; }

        internal TimeSpan ComputeDuration()
        {
            Duration = StopTime - StartTime;
            return Duration;
        }
    }
}

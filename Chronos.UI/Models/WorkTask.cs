using System;
using System.Collections.Generic;
using System.Text;

namespace Chronos.UI.Models
{
    public class WorkTask
    {
        public string Name { get; set; }
        public TimeSpan TotalDuration { get; set; }
        public List<Activity> Activites { get; set; } = new List<Activity>();
        public string Guid { get; set; }

        internal void ComputeTotalDuration(int indexOfAcitivity)
        {
            TotalDuration += Activites[indexOfAcitivity].ComputeDuration();
        }
    }
}

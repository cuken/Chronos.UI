using System.Collections.Generic;

namespace Chronos.UI.Models
{
    public class TimeSheet
    {
        List<WorkTask> Tasks { get; set; }
        WorkTask ActiveTask { get; set; }
    }
}

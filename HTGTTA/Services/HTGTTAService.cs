using HTGTTA.Enums;
using System;

namespace HTGTTA.Services
{
// method of access global variables

    public class HTGTTAService
    { 
        public TimerTypeEnum TimerType { get; set; } = TimerTypeEnum.Normal; //type of timer

        public TimeSpan MaxTime { get; set; } = new TimeSpan(0, 15, 0); //maximum amount of time
    }
}

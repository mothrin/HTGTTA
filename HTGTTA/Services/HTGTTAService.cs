
using HTGTTA.Enums;
using System;

namespace HTGTTA.Services
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A service for accessing htgttas information. </summary>
    ///
    /// <remarks>   Moth, 16/03/2024. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class HTGTTAService
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the type of the timer. </summary>
        ///
        /// <value> The type of the timer. </value>
        ///-------------------------------------------------------------------------------------------------

        public TimerTypeEnum TimerType { get; set; } = TimerTypeEnum.Normal;

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the maximum time. </summary>
        ///
        /// <value> The maximum time of the. </value>
        ///-------------------------------------------------------------------------------------------------

        public TimeSpan MaxTime { get; set; } = new TimeSpan(0, 15, 0);
    }
}

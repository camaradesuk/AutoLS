using System;
using MassTransit.Scheduling;

namespace SyRF.BiorxivParser.Endpoint.Activities
{
    class BiorxivCovidSearchSchedule : DefaultRecurringSchedule
    {
        public BiorxivCovidSearchSchedule(int days)
        {
            StartTime = DateTime.Now;
            CronExpression = "0 0 12 1/1 * ? *"; // In every 12 hours.
            MisfirePolicy = MissedEventPolicy.Default;
            EndTime = null;
        }
    }
}
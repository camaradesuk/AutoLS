using System;
using MassTransit.Scheduling;

namespace SyRF.LivingSearch.Endpoint.Activities
{
    public class LivingSearchSchedule : DefaultRecurringSchedule
    {
        public LivingSearchSchedule(int days)
        {
            StartTime = DateTime.Now;
            CronExpression = $"{StartTime.Second + 5} {StartTime.Minute} {StartTime.Hour} */{days} * ? *";
            MisfirePolicy = MissedEventPolicy.Default;
            EndTime = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TermPricing
{

    public struct DateTimeSpan
    {
        private readonly int years;
        private readonly int months;
        private readonly int days;
        private readonly int hours;
        private readonly int minutes;
        private readonly int seconds;
        private readonly int milliseconds;

        public DateTimeSpan(int years, int months, int days, int hours, int minutes, int seconds, int milliseconds)
        {
            this.years = years;
            this.months = months;
            this.days = days;
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
            this.milliseconds = milliseconds;
        }

        public int Years { get { return years; } }
        public int Months { get { return months; } }
        public int Days { get { return days; } }
        public int Hours { get { return hours; } }
        public int Minutes { get { return minutes; } }
        public int Seconds { get { return seconds; } }
        public int Milliseconds { get { return milliseconds; } }

        enum Phase { Years, Months, Days, Done }

        public static DateTimeSpan CompareDates(DateTime date1, DateTime date2)
        {
            if (date2 < date1)
            {
                var sub = date1;
                date1 = date2;
                date2 = sub;
            }

            DateTime current = date1;
            int years = 0;
            int months = 0;
            int days = 0;

            Phase phase = Phase.Years;
            DateTimeSpan span = new DateTimeSpan();

            while (phase != Phase.Done)
            {
                switch (phase)
                {
                    case Phase.Years:
                        if (current.AddYears(years + 1) > date2)
                        {
                            phase = Phase.Months;
                            current = current.AddYears(years);
                        }
                        else
                        {
                            years++;
                        }
                        break;
                    case Phase.Months:
                        if (current.AddMonths(months + 1) > date2)
                        {
                            phase = Phase.Days;
                            current = current.AddMonths(months);
                        }
                        else
                        {
                            months++;
                        }
                        break;
                    case Phase.Days:
                        if (current.AddDays(days + 1) > date2)
                        {
                            current = current.AddDays(days);
                            var timespan = date2 - current;
                            span = new DateTimeSpan(years, months, days, timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
                            phase = Phase.Done;
                        }
                        else
                        {
                            days++;
                        }
                        break;
                }
            }

            return span;
        }

        public static TimeSpan retrieveOffset(IOrganizationService service)
        {
            var currentUserSettings = service.RetrieveMultiple(
            new QueryExpression("usersettings")
            {
                ColumnSet = new ColumnSet("localeid", "timezonecode"),
                Criteria = new FilterExpression
                {
                    Conditions =
               {
            new ConditionExpression("systemuserid", ConditionOperator.EqualUserId)
               }
                }
            }).Entities[0].ToEntity<Entity>();
            int? userzonecode = (int?)currentUserSettings.Attributes["timezonecode"];

            if (userzonecode != null)
            {
                var qe = new QueryExpression("timezonedefinition");
                qe.ColumnSet = new ColumnSet("standardname");
                qe.Criteria.AddCondition("timezonecode", ConditionOperator.Equal, userzonecode);
                EntityCollection _timeZoneDef = service.RetrieveMultiple(qe);

                if (_timeZoneDef.Entities.Count == 1)
                {
                    String timezonename = _timeZoneDef.Entities[0]["standardname"].ToString();
                    TimeZoneInfo userZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezonename);
                    TimeSpan ts = userZoneInfo.GetUtcOffset(DateTime.Now);
                    return ts;
                }
            }
            
            return new TimeSpan(0,0,0);
        }

        public static DateTime DateTimeLocal(DateTime dateTimeUTC, IOrganizationService service)
        {
            TimeSpan ts = DateTimeSpan.retrieveOffset(service);
            DateTime ret = dateTimeUTC.AddHours(ts.Hours);
            return ret.AddMinutes(ts.Minutes);
        }
    }


}

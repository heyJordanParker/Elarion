using System;
using System.Collections.Generic;
using System.Linq;

namespace Elarion.Extensions {
    public static class DateTimeExtensions {

        // Time in minutes
        private const double Minute = 1;
        private const double Hour = 60 * Minute;
        private const double Day = 24 * Hour;
        private const double Month = 30 * Day;
        private const double Year = 365 * Day;
        
        private static readonly SortedList<double, Func<TimeSpan, string>> RelativeTimeOffsets =
            new SortedList<double, Func<TimeSpan, string>> {
                {Minute * 0.75, t => "less than a minute"},
                {Minute * 1.5, t => "about a minute"},
                {Minute * 45, t => $"{t.TotalMinutes:F0} minutes"},
                {Hour * 1.5, t => "about an hour"},
                {Day, t => $"{t.TotalHours:F0} hours"},
                {Day * 2, t => "about a day"},
                {Month, t => $"{t.TotalDays:F0} days"},
                {Month * 2, t => "about a month"},
                {Year, t => $"{t.TotalDays / 30:F0} months"},
                {Year * 2, t => "about a year"},
                {double.MaxValue, t => $"{t.TotalDays / 365:F0} years"}
            };

        // TODO an optional parameter that accepts a Localizer parameter (check Translator class) that localizes the strings and the suffixes
        // TODO add localizable prefixes
        public static string ToRelative(this DateTime value) {
            return ToRelative(value, DateTime.Now);
        }
        
        public static string ToUTCRelative(this DateTime value) {
            return ToRelative(value, DateTime.UtcNow);
        }

        public static string ToRelative(this DateTime value, DateTime now) {
            var delta = now - value;
            var suffix = delta.TotalMinutes > 0 ? " ago" : " from now";
            
            delta = new TimeSpan(Math.Abs(delta.Ticks));
            return RelativeTimeOffsets.First(n => delta.TotalMinutes < n.Key).Value(delta) + suffix;
        }
    }
}
using System;

namespace Svyaznoy.Core
{
    using System.Linq;

    public static class DateExtensions
    {
        public static DateTime MinSqlDateTime = new DateTime(1753,1,1);

        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999);
        }

        public static string ToLongDateTimeStringWithShift(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy HH:mm:ss zzz");
        }

        public static bool LeapYear(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, 12, 31).DayOfYear == 366;
        }

        public static int GetDifferenceInDays(this DateTime firstDate, DateTime secondDate, bool considerTimeSpan = true)
        {
            DateTime startDate;
            DateTime endDate;

            if (firstDate > secondDate)
            {
                startDate = secondDate;
                endDate = firstDate;
            }
            else
            {
                startDate = firstDate;
                endDate = secondDate;
            }

            int countWeekednDaysStart = WeekendDateTimes.Count(d => d.Year == startDate.Year && d <= startDate);
            int startDateWorkDays = startDate.DayOfYear - countWeekednDaysStart;

            int countWeekednDaysEnd = WeekendDateTimes.Count(d => d.Year == endDate.Year && d <= endDate);
            int endDateWorkDays = endDate.DayOfYear - countWeekednDaysEnd;

            int timeCorrection = 0;
            if (considerTimeSpan)
            {
                TimeSpan difTime = endDate.TimeOfDay - startDate.TimeOfDay;
                timeCorrection = difTime < TimeSpan.Zero ? 1 : 0;
            }

            if (WeekendDateTimes.Contains(startDate.Date))
            {
                timeCorrection += 1;
            }

            int result;

            if (startDate.Year == endDate.Year)
                result = endDateWorkDays - startDateWorkDays;
            else
            {
                int countDays = 0;
                DateTime tempTime = startDate;
                while (tempTime.Year < endDate.Year)
                {
                    countDays += tempTime.LeapYear() ? 366 : 365 - WeekendDateTimes.Count(w => w.Year == tempTime.Year);
                    tempTime = tempTime.AddYears(1);
                }

                result = (countDays + endDateWorkDays) - startDateWorkDays;
            }

            return result == 0 ? result : result - timeCorrection;
        }

        /// <summary>
        /// Получить дату подходящуюю под диапазон SQL
        /// </summary>
        /// <param name="date">дата</param>
        public static DateTime GetSqlDate(this DateTime date)
        {
            return date == DateTime.MinValue ? MinSqlDateTime : date;
        }

        /// <summary>
        /// Получить дату подходящуюю под диапазон SQL
        /// </summary>
        /// <param name="date">дата</param>
        public static DateTime? GetSqlDate(this DateTime? date)
        {
            if (date.HasValue)
            {
                return date.Value == DateTime.MinValue ? null : date;
            }
            
            return date;
        }

        public static string[] MonthRussianNamesInGenetive = { "январь", "февраль", "март", "апрель", "май", "июнь", "июль", "август", "сентябрь", "октябрь", "ноябрь", "декабрь" };
        public static string[] MonthRussianNamesInParent = { "января", "февраля", "марта", "апреля", "мая", "июня", "июля", "августа", "сентября", "октября", "ноября", "декабря" }; 

        public static DateTime[] WeekendDateTimes = {
                                                        DateTime.Parse("2015-01-01"),
                                                        DateTime.Parse("2015-01-02"),
                                                        DateTime.Parse("2015-01-03"),
                                                        DateTime.Parse("2015-01-04"),
                                                        DateTime.Parse("2015-01-05"),
                                                        DateTime.Parse("2015-01-06"),
                                                        DateTime.Parse("2015-01-07"),
                                                        DateTime.Parse("2015-01-08"),
                                                        DateTime.Parse("2015-01-09"),
                                                        DateTime.Parse("2015-01-10"),
                                                        DateTime.Parse("2015-01-11"),
                                                        DateTime.Parse("2015-01-17"),
                                                        DateTime.Parse("2015-01-18"),
                                                        DateTime.Parse("2015-01-24"),
                                                        DateTime.Parse("2015-01-25"),
                                                        DateTime.Parse("2015-01-31"),
                                                        DateTime.Parse("2015-02-01"),
                                                        DateTime.Parse("2015-02-07"),
                                                        DateTime.Parse("2015-02-08"),
                                                        DateTime.Parse("2015-02-14"),
                                                        DateTime.Parse("2015-02-15"),
                                                        DateTime.Parse("2015-02-21"),
                                                        DateTime.Parse("2015-02-22"),
                                                        DateTime.Parse("2015-02-23"),
                                                        DateTime.Parse("2015-02-28"),
                                                        DateTime.Parse("2015-03-01"),
                                                        DateTime.Parse("2015-03-07"),
                                                        DateTime.Parse("2015-03-08"),
                                                        DateTime.Parse("2015-03-09"),
                                                        DateTime.Parse("2015-03-14"),
                                                        DateTime.Parse("2015-03-15"),
                                                        DateTime.Parse("2015-03-21"),
                                                        DateTime.Parse("2015-03-22"),
                                                        DateTime.Parse("2015-03-28"),
                                                        DateTime.Parse("2015-03-29"),
                                                        DateTime.Parse("2015-04-04"),
                                                        DateTime.Parse("2015-04-05"),
                                                        DateTime.Parse("2015-04-11"),
                                                        DateTime.Parse("2015-04-12"),
                                                        DateTime.Parse("2015-04-18"),
                                                        DateTime.Parse("2015-04-19"),
                                                        DateTime.Parse("2015-04-25"),
                                                        DateTime.Parse("2015-04-26"),
                                                        DateTime.Parse("2015-05-01"),
                                                        DateTime.Parse("2015-05-02"),
                                                        DateTime.Parse("2015-05-03"),
                                                        DateTime.Parse("2015-05-04"),
                                                        DateTime.Parse("2015-05-09"),
                                                        DateTime.Parse("2015-05-10"),
                                                        DateTime.Parse("2015-05-11"),
                                                        DateTime.Parse("2015-05-16"),
                                                        DateTime.Parse("2015-05-17"),
                                                        DateTime.Parse("2015-05-23"),
                                                        DateTime.Parse("2015-05-24"),
                                                        DateTime.Parse("2015-05-30"),
                                                        DateTime.Parse("2015-05-31"),
                                                        DateTime.Parse("2015-06-06"),
                                                        DateTime.Parse("2015-06-07"),
                                                        DateTime.Parse("2015-06-12"),
                                                        DateTime.Parse("2015-06-13"),
                                                        DateTime.Parse("2015-06-14"),
                                                        DateTime.Parse("2015-06-20"),
                                                        DateTime.Parse("2015-06-21"),
                                                        DateTime.Parse("2015-06-27"),
                                                        DateTime.Parse("2015-06-28"),
                                                        DateTime.Parse("2015-07-04"),
                                                        DateTime.Parse("2015-07-05"),
                                                        DateTime.Parse("2015-07-11"),
                                                        DateTime.Parse("2015-07-12"),
                                                        DateTime.Parse("2015-07-18"),
                                                        DateTime.Parse("2015-07-19"),
                                                        DateTime.Parse("2015-07-25"),
                                                        DateTime.Parse("2015-07-26"),
                                                        DateTime.Parse("2015-08-01"),
                                                        DateTime.Parse("2015-08-02"),
                                                        DateTime.Parse("2015-08-08"),
                                                        DateTime.Parse("2015-08-09"),
                                                        DateTime.Parse("2015-08-15"),
                                                        DateTime.Parse("2015-08-16"),
                                                        DateTime.Parse("2015-08-22"),
                                                        DateTime.Parse("2015-08-23"),
                                                        DateTime.Parse("2015-08-29"),
                                                        DateTime.Parse("2015-08-30"),
                                                        DateTime.Parse("2015-09-05"),
                                                        DateTime.Parse("2015-09-05"),
                                                        DateTime.Parse("2015-09-06"),
                                                        DateTime.Parse("2015-09-12"),
                                                        DateTime.Parse("2015-09-13"),
                                                        DateTime.Parse("2015-09-19"),
                                                        DateTime.Parse("2015-09-20"),
                                                        DateTime.Parse("2015-09-26"),
                                                        DateTime.Parse("2015-09-27"),
                                                        DateTime.Parse("2015-10-03"),
                                                        DateTime.Parse("2015-10-04"),
                                                        DateTime.Parse("2015-10-10"),
                                                        DateTime.Parse("2015-10-11"),
                                                        DateTime.Parse("2015-10-17"),
                                                        DateTime.Parse("2015-10-18"),
                                                        DateTime.Parse("2015-10-24"),
                                                        DateTime.Parse("2015-10-25"),
                                                        DateTime.Parse("2015-10-31"),
                                                        DateTime.Parse("2015-11-01"),
                                                        DateTime.Parse("2015-11-04"),
                                                        DateTime.Parse("2015-11-07"),
                                                        DateTime.Parse("2015-11-08"),
                                                        DateTime.Parse("2015-11-14"),
                                                        DateTime.Parse("2015-11-15"),
                                                        DateTime.Parse("2015-11-21"),
                                                        DateTime.Parse("2015-11-22"),
                                                        DateTime.Parse("2015-11-28"),
                                                        DateTime.Parse("2015-11-29"),
                                                        DateTime.Parse("2015-12-05"),
                                                        DateTime.Parse("2015-12-06"),
                                                        DateTime.Parse("2015-12-12"),
                                                        DateTime.Parse("2015-12-13"),
                                                        DateTime.Parse("2015-12-19"),
                                                        DateTime.Parse("2015-12-20"),
                                                        DateTime.Parse("2015-12-26"),
                                                        DateTime.Parse("2015-12-27")
                                                    };
    }
}

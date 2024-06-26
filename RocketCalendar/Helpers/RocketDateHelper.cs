﻿using RocketCalendar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocketCalendar.Helpers
{
    public class RocketDateHelper
    {
        public int GetFirstDayNameIndexOfCurrentYear(RocketCalendarModel calendar, int currentMonthIndex, int currentYear)
        {
            int indexResult = 0;

            //First compare base year to current year
            bool isBaseDateInPastOrCurrentMonth = IsBaseDateInPastOrCurrentMonth(calendar, currentMonthIndex, currentYear);

            //Next get the total difference in days between the 1st of the month and the base date

            int dayCountDiff = GetDifferenceInDays(calendar, 1, currentMonthIndex, currentYear);

            //Find the day index difference
            int dayIndexDiff = dayCountDiff % calendar.DayNameCollection.Count;

            //Determine shift direction
                //Specifically, determine if the base date in the past or future compared to the current date
            bool isBaseDateInFuture = true;
            if (isBaseDateInPastOrCurrentMonth)
            {
                if(calendar.BaseDate.DateMonth == currentMonthIndex)
                {
                    isBaseDateInFuture = true;
                }
                else
                {
                    isBaseDateInFuture = false;
                }
            }

            //If the base date is in the future:
            if(isBaseDateInFuture)
            {
                indexResult = calendar.BaseDate.DayIndex - dayIndexDiff;
            }
            //If the base date is in the past:
            else
            {
                indexResult = calendar.BaseDate.DayIndex + dayIndexDiff;
                if (indexResult > calendar.DayNameCollection.Count - 1)
                {
                    indexResult -= calendar.DayNameCollection.Count;
                }
            }

            if(indexResult < 0)
            {
                indexResult += calendar.DayNameCollection.Count;
            }

            return indexResult;
        }

        public bool DoesRepeatingEventApplyToDate(RocketCalendarModel calendar, RocketEvent rEvent, RocketDate targetDate)
        {
            //This function assumes rEvent.EventDate.DateDay == targetDate.DateDay for performance reasons
            int yearDiff = Math.Abs(targetDate.DateYear - rEvent.EventDate.DateYear);
            int monthDiff = 0;

            if(rEvent.YearRepeatInterval != 0)
            {
                //repeats over years
                if(yearDiff % rEvent.YearRepeatInterval == 0 && rEvent.EventDate.DateMonth == targetDate.DateMonth && rEvent.EventDate.DateDay == targetDate.DateDay)
                {
                    return true;
                }
            }

            if(rEvent.MonthRepeatInterval != 0)
            {
                //repeats over months
                //Determine which date came first
                if(targetDate.DateYear > rEvent.EventDate.DateYear)
                {
                    monthDiff += calendar.MonthCollection.Count * (yearDiff - 1);
                    monthDiff += targetDate.DateMonth;
                    monthDiff += calendar.MonthCollection.Count - rEvent.EventDate.DateMonth;
                } 
                else if(targetDate.DateYear < rEvent.EventDate.DateYear)
                {
                    monthDiff += calendar.MonthCollection.Count * (yearDiff - 1);
                    monthDiff += rEvent.EventDate.DateMonth;
                    monthDiff += calendar.MonthCollection.Count - targetDate.DateMonth;
                }
                else if(targetDate.DateYear == rEvent.EventDate.DateYear)
                {
                    monthDiff = Math.Abs(rEvent.EventDate.DateMonth -  targetDate.DateMonth);
                }
                
                if(monthDiff % rEvent.MonthRepeatInterval == 0)
                {
                    return true;
                }
            }
            return false;
        }

        #region Resources

        private bool IsBaseDateInPastOrCurrentMonth(RocketCalendarModel calendar, int currentMonthIndex, int currentYear)
        {
            if (currentYear > calendar.BaseDate.DateYear)
            {
                //if base date year is in a past year
                return true;
            }
            else if (currentYear < calendar.BaseDate.DateYear)
            {
                //if base date year is in a future year
                return false;
            }
            else if (currentYear == calendar.BaseDate.DateYear)
            {
                //if base date year is in the same year, compare base month to current month

                if (currentMonthIndex > calendar.BaseDate.DateMonth)
                {
                    //if base date month is in a past month
                    return true;
                }
                else if (currentMonthIndex < calendar.BaseDate.DateMonth)
                {
                    //if base date month is in a future month
                    return false;
                }
                else if (currentMonthIndex == calendar.BaseDate.DateMonth)
                {
                    //if base date month is in the same month
                    return true;
                }
            }
            //Replace with exception flag
            return true;
        }

        private int DaysInYear(RocketCalendarModel calendar, int requestedYear) 
        {
            int numOfDaysInYear = 0;

            for (int a = 0; a < calendar.MonthCollection.Count; a++)
            {
                numOfDaysInYear += calendar.MonthCollection[a].NumOfDays;

                //Check for leap days
                if (calendar.MonthCollection[a].LeapYearInterval != null)
                {
                    //If requested year is a leap year for a particular month, add an additional day
                    if (requestedYear % calendar.MonthCollection[a].LeapYearInterval == 0)
                    {
                        numOfDaysInYear++;
                    }
                }
            }
            return numOfDaysInYear;
        }

        private int GetRemainingDaysInYear(RocketCalendarModel calendar, int currentDay, int currentMonth, int currentYear)
        {
            int result = DaysInYear(calendar, currentYear);
            result -= GetNumberOfDayWithRespectToCurrentYear(calendar, currentDay, currentMonth, currentYear);
            return result;
        }

        private int GetNumberOfDayWithRespectToCurrentYear(RocketCalendarModel calendar, int currentDay, int currentMonth, int currentYear)
        {
            int result = 0;
            if (currentMonth > 0)
            {
                for (int i = 0; i < currentMonth; i++)
                {
                    result += calendar.MonthCollection[i].NumOfDays;

                    //If current year is a leap year for a particular month, add an additional day
                    if (currentYear % calendar.MonthCollection[i].LeapYearInterval == 0)
                    {
                        result++;
                    }
                }
            }
            result += currentDay;

            return result;
        }

        private int GetDifferenceInDays(RocketCalendarModel calendar, int targetDay, int targetMonth, int targetYear)
        {
            int result = 0;

            //determine which date came first
            bool isBaseDateBeforeTargetMonth = IsBaseDateBeforeTargetMonth(calendar, targetMonth, targetYear);

            if(isBaseDateBeforeTargetMonth)
            {
                //If year difference is greater than 1, sum full years
                if (targetYear > calendar.BaseDate.DateYear + 1)
                {
                    for (int i = calendar.BaseDate.DateYear + 1;  i < targetYear; i++)
                    {
                        //DaysInYear handles leap years
                        result += DaysInYear(calendar, i);
                    }
                }

                //If year difference is greater than 0, add remainders of partial years
                if (targetYear > calendar.BaseDate.DateYear)
                {
                    result += GetRemainingDaysInYear(calendar, calendar.BaseDate.DateDay, calendar.BaseDate.DateMonth, calendar.BaseDate.DateYear);
                    result += GetNumberOfDayWithRespectToCurrentYear(calendar, targetDay, targetMonth, targetYear);
                }

                //If the base date is before the target date but in the same year, add target's wrt year # and subtract base's wrt year # ? check this*********************
                if(targetYear ==  calendar.BaseDate.DateYear)
                {
                    result += GetNumberOfDayWithRespectToCurrentYear(calendar, targetDay, targetMonth, targetYear);
                    result -= GetNumberOfDayWithRespectToCurrentYear(calendar, calendar.BaseDate.DateDay, calendar.BaseDate.DateMonth, calendar.BaseDate.DateYear);
                }
            }
            
            else if (!isBaseDateBeforeTargetMonth)
            {
                //If year difference is greater than 1, summ full years
                if (calendar.BaseDate.DateYear > targetYear + 1)
                {
                    for (int i = targetYear + 1; i < calendar.BaseDate.DateYear; i++)
                    {
                        //DaysInYear handles leap years
                        result += DaysInYear(calendar, i);
                    }
                }

                //If year difference is greater than 0, add remainders of partial years
                if(calendar.BaseDate.DateYear > targetYear)
                {
                    result += GetNumberOfDayWithRespectToCurrentYear(calendar, calendar.BaseDate.DateDay, calendar.BaseDate.DateMonth, calendar.BaseDate.DateYear);
                    result += GetRemainingDaysInYear(calendar, targetDay, targetMonth, targetYear);
                }

                //If the base date is before the target date but in the same year, add base's wrt year # and subtract target's wrt year # ? check this*********************
                if (targetYear == calendar.BaseDate.DateYear)
                {
                    result += GetNumberOfDayWithRespectToCurrentYear(calendar, calendar.BaseDate.DateDay, calendar.BaseDate.DateMonth, calendar.BaseDate.DateYear);
                    result -= GetNumberOfDayWithRespectToCurrentYear(calendar, targetDay, targetMonth, targetYear);
                    
                }
            }
            return result;
        }

        private bool IsBaseDateBeforeTargetMonth(RocketCalendarModel calendar, int targetMonth, int targetYear)
        {
            bool result = false;
            if(calendar.BaseDate.DateYear < targetYear)
            {
                result = true;
            } else if (calendar.BaseDate.DateYear == targetYear)
            {
                if(calendar.BaseDate.DateMonth < targetMonth)
                {
                    result = true;
                }
            }
            return result;
        }

        #endregion
    }
}

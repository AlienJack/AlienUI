using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DEMO
{

    public static class Utility
    {
        public static string GetDayOfWeekStr(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => "星期日",
                DayOfWeek.Monday => "星期一",
                DayOfWeek.Tuesday => "星期二",
                DayOfWeek.Wednesday => "星期三",
                DayOfWeek.Thursday => "星期四",
                DayOfWeek.Friday => "星期五",
                DayOfWeek.Saturday => "星期六",
                _ => throw new Exception("not possible"),
            };
        }
    }
}

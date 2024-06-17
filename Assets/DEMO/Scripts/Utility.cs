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
                DayOfWeek.Sunday => "������",
                DayOfWeek.Monday => "����һ",
                DayOfWeek.Tuesday => "���ڶ�",
                DayOfWeek.Wednesday => "������",
                DayOfWeek.Thursday => "������",
                DayOfWeek.Friday => "������",
                DayOfWeek.Saturday => "������",
                _ => throw new Exception("not possible"),
            };
        }
    }
}

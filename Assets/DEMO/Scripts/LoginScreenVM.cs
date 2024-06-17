using AlienUI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace DEMO
{
    public class LoginScreenVM : ViewModel
    {
        public string CurrentTime
        {
            get
            {
                var dateNow = DateTime.Now;
                return $"{dateNow.Hour}:{dateNow.Minute}";
            }
        }

        public string CurrentDate
        {
            get
            {
                var dateNow = DateTime.Now;
                return $"{dateNow.Month}ÔÂ{dateNow.Day:00}ºÅ,{dateNow.DayOfWeek.GetDayOfWeekStr()}";
            }
        }

        public LoginScreenVM()
        {
            Timer timer = new Timer(1);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RaisePropertyChanged("CurrentData");
            RaisePropertyChanged("CurrentTime");
        }
    }
}
